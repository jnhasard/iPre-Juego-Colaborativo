using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Threading;

public class Server : MonoBehaviour
{

    #region Attributes

    public int port;

    public int maxConnections;

    private int socketId;
    private int channelId;
    private int bigChannelId;
    private int secureChannel;
    private bool listening;

    public List<Room> rooms;
    public Room lastDestroyedRoom; //Almacena una referencia al ultimo room que se ha destruido para quue el siguiente room en crearse use el mismo logger que el anterior.
    private RoomManager rm;

    private ServerNetworkDiscovery serverNetworkDiscovery;
    public ServerMessageHandler messageHandler;
    public static Server instance;
    public int maxPlayers;
    public string sceneToLoad;
    public string NPCsLastMessage;
    public bool debug;
    private int roomCounter = 0;
    //Planner Thread
    Thread planner;
    //Cola de mensajes a procesar
    private List<string> messageStack = new List<string>();
    //Cola de conections id del output del planner
    private List<int> connectionIdStack = new List<int>();
    //Cola de mensajes procesados por el planner
    private List<string> outputStack = new List<string>();
    //Ruta y nombre del template del archivo problema
    public string templateFileName;
    //Ruta y nombre del archivo problema nuevo
    public string problemFileName;
    //Ruta y nombre del archivo batch para planificar
    public string batchFileName;
    //Ruta y nombre del archivo output de la planificacion
    public string outputFileName;
    //Donde se debe escribir para cada template
    public List<int> startLinePerLevel;
    //Cache de planes
    private Dictionary<string, string> cacheOutput = new Dictionary<string, string>();

    #endregion

    #region Start

    void Start()
    {

        NPCsLastMessage = "";
        maxPlayers = 3;
        instance = this;

        NetworkTransport.Init();

        serverNetworkDiscovery = GetComponent<ServerNetworkDiscovery>();

        ConnectionConfig config = new ConnectionConfig();

        rm = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
        channelId = config.AddChannel(QosType.Unreliable);
        bigChannelId = config.AddChannel(QosType.ReliableFragmented);
        secureChannel = config.AddChannel(QosType.Reliable);
        HostTopology topology = new HostTopology(config, maxConnections);

        int[] connectionData = serverNetworkDiscovery.CreateServer(topology);

        if (connectionData[0] != -1 && connectionData[1] != -1)
        {
            port = connectionData[0];
            socketId = connectionData[1];

            listening = true;
            UnityEngine.Debug.Log("Server listening on port " + port);
        }
        else
        {
            throw new Exception("Server didn't found a free port");
        }

        rooms = new List<Room>();
        messageHandler = new ServerMessageHandler(this);

        planner = new Thread(new ThreadStart(this.Plan));
        planner.Start();

        this.sceneToLoad = "Escena1";
    }

    #endregion

    #region Reset

    public void Reset()
    {
        if (!listening)
        {
            UnityEngine.Debug.Log("Server is not listening yet");
            return;
        }

        serverNetworkDiscovery.ResetServer();
    }

    #endregion

    #region Update

    void LateUpdate()
    {
        if (connectionIdStack.Count > 0)
        {
            int connectionId = connectionIdStack[0];
            string output = outputStack[0];
            SendPlannerInfoToClient(connectionId, output);
            connectionIdStack.RemoveAt(0);
            outputStack.RemoveAt(0);
        }
        int recSocketId;
        int recConnectionId; // Reconoce la ID del jugador
        int recChannelId;
        byte[] recBuffer = new byte[NetworkConsts.bufferSize];
        int dataSize;
        byte error;
        string recAddress;
        byte recError;
        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recSocketId, out recConnectionId, out recChannelId, recBuffer, NetworkConsts.bufferSize, out dataSize, out error);
        UnityEngine.Networking.Types.NetworkID recNetId;
        UnityEngine.Networking.Types.NodeID recNodeId;
        NetworkTransport.GetConnectionInfo(socketId, recConnectionId, out recAddress, out port, out recNetId, out recNodeId, out recError);
        NetworkError Error = (NetworkError)error;
        if (Error == NetworkError.MessageToLong)
        {
            //Trata de capturar el mensaje denuevo, pero asumiendo buffer más grande.
            recBuffer = new byte[NetworkConsts.bigBufferSize];
            recNetworkEvent = NetworkTransport.Receive(out recSocketId, out recConnectionId, out recChannelId, recBuffer, NetworkConsts.bigBufferSize, out dataSize, out error);
        }
        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                AddConnection(recConnectionId);
                UnityEngine.Debug.Log("Client " + recAddress + " connected");
                break;
            case NetworkEventType.DataEvent:
                Stream stream = new MemoryStream(recBuffer);
                BinaryFormatter formatter = new BinaryFormatter();
                string message = formatter.Deserialize(stream) as string;

                if (recChannelId == channelId || recChannelId == secureChannel)
                {
                    //Mensaje corto normal
                    messageHandler.HandleMessage(message, recAddress);

                }
                if (recChannelId == bigChannelId)
                {
                    //Mensaje largo. Planner
                    SendMessagToPlanner(message, recConnectionId);
                }

                if (debug)
                {
                    UnityEngine.Debug.Log(HoraMinuto() + " - from(" + recAddress + "): " + message);
                }

                break;
            case NetworkEventType.DisconnectEvent:
                DeleteConnection(recConnectionId);
                UnityEngine.Debug.Log("Client " + recAddress + " disconnected");
                break;
        }
    }

    #endregion

    #region Common

    public void InitializeBroadcast()
    {
        if (!listening)
        {
            UnityEngine.Debug.Log("Server is not listening yet");
            return;
        }

        serverNetworkDiscovery.InitializeBroadcast();
    }

    private void AddConnection(int connectionId)
    {
        int port;
        byte recError;
        string recAddress;
        UnityEngine.Networking.Types.NodeID recNodeId;
        UnityEngine.Networking.Types.NetworkID recNetId;

        NetworkTransport.GetConnectionInfo(socketId, connectionId, out recAddress, out port, out recNetId, out recNodeId, out recError);
        NetworkPlayer player = GetPlayer(recAddress);

        // Jugador existía y se reconecta.
        if (player != null)
        {
            player.connected = true;
            player.connectionId = connectionId;
            SendMessageToClient(connectionId, "ChangeScene/" + player.room.sceneToLoad, true);
            List <NetworkPlayer> players = player.room.players;
			foreach (NetworkPlayer connectedPlayer in players)
			{
				if (connectedPlayer.controlOverEnemies) 
				{
                    SendMessageToClient(connectionId, "PlayerHasReturned/", true);
					break;
				}	
			}
            messageHandler.SendAllData(recAddress, player.room);
            player.room.SendControlEnemiesToClient(player, false);
            player.SendDataToRoomBoxManager();
            UnityEngine.Debug.Log("Client " + recAddress + " reconnected");
            return;
        }

        //Jugador no existía y se crea uno nuevo.
        Room room = SearchRoom();
        if (room == null)
        {
            roomCounter++;
            RoomLogger logger = null;
            if(lastDestroyedRoom != null)
            {
                logger = lastDestroyedRoom.log;
                lastDestroyedRoom = null;
            }
            room = new Room(roomCounter, this, messageHandler, maxPlayers,logger);
            RoomManager rm = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
            if (!rm)
            {
                UnityEngine.Debug.LogError("No se encontró RoomManager en ServerScene. uwu 1");
            }
            rm.AddNewRoom(room);
            rooms.Add(room);
        }

        room.AddPlayer(connectionId, recAddress);
    }

    private void DeleteConnection(int connectionId)
    {

        NetworkPlayer player = GetPlayer(connectionId);

        if (player != null)
        {
            player.connected = false;

            string msg;
            if (player.id == 0)
            {
                msg = "Verde se ha Desconectado";
            }
            else if (player.id == 1)
            {
                msg = "Rojo se ha Desconectado";
            }
            else
            {
                msg = "Amarillo se ha Desconectado";
            }

            if (player.controlOverEnemies == true)
            {
                player.room.ChangeControlEnemies();
            }

            player.room.SendMessageToAllPlayers("NewChatMessage/" + msg, false);
            player.room.SendMessageToAllPlayers("PlayerDisconnected/" + player.id, false);

            RoomManager rm = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
            if (rm)
            {
                rm.DeletePlayerFromRoom(connectionId, GetPlayer(connectionId).room);
            }
            else
            {
                UnityEngine.Debug.LogError("No se encontró RoomManager en ServerScene. uwu");
            }
        }

    }

    #endregion

    #region Utils

    public NetworkPlayer GetPlayer(string address)
    {
        foreach (Room room in rooms)
        {
            NetworkPlayer player = room.FindPlayerInRoom(address);
            if (player != null)
            {
                return player;
            }
        }
        return null;
    }

    public NetworkPlayer GetPlayer(int id)
    {
        foreach (Room room in rooms)
        {
            NetworkPlayer player = room.FindPlayerInRoom(id);
            if (player != null)
            {
                return player;
            }
        }
        return null;
    }

    //Retorna la sala con la mayor cantidad de jugadores que no esté llena.
    private Room SearchRoom()
    {
        Room selectedRoom = null;
        int selectedMaxPlayers = 0;
        foreach (Room room in rooms)
        {
            if (!room.IsFull())
            {
                if (selectedMaxPlayers <= room.numPlayers)
                {
                    selectedRoom = room;
                    selectedMaxPlayers = room.numPlayers;
                }
            }
        }
        return selectedRoom;
    }

    private string HoraMinuto()
    {
        DateTime now = DateTime.Now;

        string hora = now.Hour.ToString();
        string minutos = now.Minute.ToString();
        string segundos = now.Second.ToString();


        if (minutos.Length == 1)
        {
            minutos = "0" + minutos;
        }

        if (segundos.Length == 1)
        {
            segundos = "0" + segundos;
        }

        string tiempo = " " + hora + ":" + minutos + ":" + segundos;
        return tiempo;
    }

    public void DestroyRoom(Room room)
    {
        lastDestroyedRoom = room;
        rooms.Remove(room);
    }

    public void ChangeRoomScene(Room room, string scene)
    {
        room.sender.SendChangeScene(scene, room);
    }

    #endregion

    #region Messaging

    public void SendMessageToClient(int clientId, string message, bool secure)
    {
        byte error;
        //int bytes = System.Text.ASCIIEncoding.ASCII.GetByteCount(message);
        byte[] buffer = new byte[NetworkConsts.bufferSize];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);
        int channel = channelId;
        if (secure)
        {
            channel = secureChannel;
        }
        NetworkTransport.Send(socketId, clientId, channel, buffer, NetworkConsts.bufferSize, out error);
    }


    public void SendPlannerInfoToClient(int clientId, string message)
    {
        byte error;
        //int bytes = System.Text.ASCIIEncoding.ASCII.GetByteCount(message);
        byte[] buffer = new byte[NetworkConsts.bigBufferSize];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);
        NetworkTransport.Send(socketId, clientId, bigChannelId, buffer, NetworkConsts.bigBufferSize, out error);
    }

    public void SendMessageToClient(NetworkPlayer player, string message, bool secure)
    {
        SendMessageToClient(player.connectionId, message, secure);
    }

    private void SendMessagToPlanner(string message, int connectionId)
    {
        messageStack.Add(connectionId + "/" + message);
    }

    #endregion

    #region Planner

    private void Plan()
    {
        while (true)
        {
            if (messageStack != null && messageStack.Count > 0)
            {

                UnityEngine.Debug.Log("stack enter count:" + messageStack.Count);

                string message = messageStack[0];
                messageStack.RemoveAt(0);

                List<string> parameters = new List<string>(message.Split('/'));
                int connectionId = int.Parse(parameters[0]);
                parameters.RemoveAt(0);

                int level = int.Parse(parameters[0]);
                parameters.RemoveAt(0);

                string def = parameters[0];
                string init = parameters[1];
                string goal = parameters[2];

                if (!cacheOutput.ContainsKey(init))
                {
                    List<string> data = new List<string>();
                    data.Add(def);
                    data.Add(")");
                    data.Add("(:init");
                    data.Add(init);
                    data.Add(")");
                    data.Add("(:goal (and");
                    data.Add(goal);

                    string tempFileName = templateFileName + level + ".txt";
                    string probFileName = problemFileName + level + ".pddl";
                    string batFileName = batchFileName + level + ".bat";
                    string outFileName = outputFileName + level + ".txt";

                    List<string> lines = new List<string>(System.IO.File.ReadAllLines(tempFileName));
                    lines.InsertRange(startLinePerLevel[level - 1], data);

                    using (StreamWriter writer = new StreamWriter(probFileName, false))
                    {
                        foreach (string line in lines)
                        {
                            writer.WriteLine(line);
                        }
                    }

                    Process batchProcess = new Process();
                    batchProcess.StartInfo.UseShellExecute = false;
                    batchProcess.StartInfo.RedirectStandardOutput = true;
                    batchProcess.StartInfo.CreateNoWindow = true;
                    batchProcess.StartInfo.FileName = batFileName;

                    string output = null;

                    try
                    {
                        batchProcess.Start();
                        UnityEngine.Debug.Log("batch star");
                        output = batchProcess.StandardOutput.ReadToEnd();
                        UnityEngine.Debug.Log(output);
                        batchProcess.WaitForExit();
                        UnityEngine.Debug.Log("batch ended");
                        batchProcess.Close();
                        UnityEngine.Debug.Log("batch close");
                        List<string> linesOutput = new List<string>(System.IO.File.ReadAllLines(outFileName));
                        if (linesOutput.Count > 0)
                        {
                            output = linesOutput[0];
                            linesOutput.RemoveAt(0);
                            linesOutput.RemoveAt(linesOutput.Count - 1);
                            foreach (string line in linesOutput)
                            {
                                output += "/" + line;
                            }

                        }
                    }

                    catch (FileNotFoundException e)
                    {
                        output = "ERROR";
                        UnityEngine.Debug.Log(output);
                    }

                    catch (Exception e)
                    {
                        output = e.ToString();
                        UnityEngine.Debug.Log(output);
                    }

                    //Send output
                    connectionIdStack.Add(connectionId);
                    outputStack.Add(output);
                    cacheOutput.Add(init, output);
                }
                else
                {
                    connectionIdStack.Add(connectionId);
                    outputStack.Add(cacheOutput[init]);
                }
            }
        }
    }
    #endregion
}
