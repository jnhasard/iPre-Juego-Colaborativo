using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Client : MonoBehaviour
{

    #region Attributes

    public static Client instance;

    private ClientMessageHandler handler;
    private HostTopology topology;

    private static int maxConnections = 12;

    private int unreliableChannelId;
    private int reliableBigChannelId;
    private int reliableLittleChannelId;
    private int connectionId;
    private string serverIp;
    private int socketId; // Host ID
    private int port;

    public bool debug;

    #endregion

    #region Start & Update

    void Start()
    {

        DontDestroyOnLoad(this);
        instance = this;

        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();

        unreliableChannelId = config.AddChannel(QosType.Unreliable);
        reliableBigChannelId = config.AddChannel(QosType.ReliableFragmented);
        reliableLittleChannelId = config.AddChannel(QosType.Reliable);

        topology = new HostTopology(config, maxConnections);

        handler = new ClientMessageHandler(this);
    }

    void LateUpdate()
    {
        int recSocketId;
        int recConnectionId; // Reconoce la ID del jugador
        int recChannelId;
        byte[] recBuffer = new byte[NetworkConsts.bufferSize];
        int dataSize;
        byte error;
        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recSocketId, out recConnectionId, out recChannelId, recBuffer, NetworkConsts.bufferSize, out dataSize, out error);
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
                if (NotInClientScene())
                {
                    if (GetLocalPlayer())
                    {
                        LevelManager lm = FindObjectOfType<LevelManager>();
                        lm.ShowReconnectingMessage(false);
                        GetLocalPlayer().Connect(true);
                    }
                }
                Debug.Log("Connection succesfull");
                break;

            case NetworkEventType.DataEvent:
                Stream stream = new MemoryStream(recBuffer);
                BinaryFormatter formatter = new BinaryFormatter();
                string message = formatter.Deserialize(stream) as string;

                if (recChannelId == unreliableChannelId || recChannelId == reliableLittleChannelId)
                {
                    handler.HandleMessage(message);
                }

                if (recChannelId == reliableBigChannelId)
                {
                    ReceiveMessageFromPlanner(message, recConnectionId);
                }

                if (debug)
                {
                    Debug.Log(HoraMinuto() + " - " + message);
                }

                break;

            case NetworkEventType.DisconnectEvent:
                if (connectionId == recConnectionId) // Detectamos que fuimos nosotros los que nos desconectamos
                {
                    Reconnecting();
                }

                Debug.Log("Disconnected from server");
                break;
        }
    }

    #endregion

    #region Common

    public void Connect()
    {
        try
        {
            byte error;
            connectionId = NetworkTransport.Connect(socketId, serverIp, port, 0, out error);
        }
        catch
        {
            Debug.Log("Connection to server failed");
        }
    }

    public bool Connect(string ip, int port)
    {
        try
        {
            this.port = port;
            socketId = NetworkTransport.AddHost(topology, port);

            byte error;
            serverIp = ip;
            connectionId = NetworkTransport.Connect(socketId, ip, port, 0, out error);
            return true;
        }
        catch
        {
            Debug.Log("Connection to server failed");
            return false;
        }

    }

    private void Reconnecting() //REVISAR ESTE CóDIGO
    {
        if (NotInClientScene())
        {
            LevelManager lm = GameObject.FindObjectOfType<LevelManager>();
            lm.ShowReconnectingMessage(true);
            GetLocalPlayer().Connect(false);
        }

        Connect();
    }

    public void StartFirstPlan()
    {
        Planner planner = FindObjectOfType<Planner>();
        if (planner)
        {
            planner.FirstPlan();
        }
    }

    #endregion

    #region Messaging

    public void SendMessageToServer(string message, bool secure)
    {
        byte error;
        byte[] buffer = new byte[NetworkConsts.bufferSize];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);
        int channel = unreliableChannelId;
        if (secure)
        {
            channel = reliableLittleChannelId;
        }
        NetworkTransport.Send(socketId, connectionId, channel, buffer, NetworkConsts.bufferSize, out error);
    }

    public void SendMessageToPlanner(string message)
    {
        byte error;
        byte[] buffer = new byte[NetworkConsts.bigBufferSize];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);
        NetworkTransport.Send(socketId, connectionId, reliableBigChannelId, buffer, NetworkConsts.bigBufferSize, out error);
    }

    private void ReceiveMessageFromPlanner(string message, int connectionId)
    {
        Planner planner = FindObjectOfType<Planner>();
        planner.SetPlanFromServer(message);
    }

    public void SendNewChatMessageToServer(string newChatMessage)
    {
        SendMessageToServer("NewChatMessage/" + newChatMessage, false);
    }

    public void RequestPlayerIdToServer()
    {
        SendMessageToServer("PlayerRequestId", true);
    }


    #endregion

    #region Utils

    public bool NotInClientScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        return currentScene.name != "ClientScene";
    }

    public PlayerController GetPlayerController(int playerId)
    {

        PlayerController script;
        GameObject player;

        switch (playerId)
        {
            case 0:
                player = GameObject.Find("Verde");
                script = player.GetComponent<MageController>();
                break;
            case 1:
                player = GameObject.Find("Rojo");
                script = player.GetComponent<WarriorController>();
                break;
            case 2:
                player = GameObject.Find("Amarillo");
                script = player.GetComponent<EngineerController>();
                break;
            default:
                player = null;
                script = null;
                break;
        }
        return script;
    }

    public EnemyController GetEnemy(int enemyId)
    {
        EnemyController[] enemies = GameObject.FindObjectsOfType<EnemyController>();

        foreach (EnemyController enemy in enemies)
        {
            if (enemy.GetEnemyId() == enemyId)
            {
                return enemy;
            }
        }

        Debug.Log("Enemy with id " + enemyId + " does not exists");
        return null;
    }

    public PlayerController GetById(int playerId)
    {
        if (playerId == 0)
        {
            return GetMage();
        }

        else if (playerId == 1)
        {
            return GetWarrior();
        }

        else
        {
            return GetEngineer();
        }
    }

    public PlayerController GetLocalPlayer()
    {
        GameObject player1 = GameObject.Find("Verde");
        GameObject player2 = GameObject.Find("Rojo");
        GameObject player3 = GameObject.Find("Amarillo");

        if (player1)
        {
            MageController player1Controller = player1.GetComponent<MageController>();
            if (player1Controller.localPlayer)
            {
                return player1Controller;
            }
        }

        if (player2)
        {
            WarriorController player2Controller = player2.GetComponent<WarriorController>();
            if (player2Controller.localPlayer)
            {
                return player2Controller;
            }
        }

        if (player3)
        {
            EngineerController player3Controller = player3.GetComponent<EngineerController>();
            if (player3Controller.localPlayer)
            {
                return player3Controller;
            }
        }

        return null;

    }

    public MageController GetMage()
    {
        GameObject player = GameObject.Find("Verde");
        MageController magecontroller = player.GetComponent<MageController>();
        return magecontroller;
    }

    public WarriorController GetWarrior()
    {
        GameObject player = GameObject.Find("Rojo");
        WarriorController script = player.GetComponent<WarriorController>();
        return script;
    }

    public EngineerController GetEngineer()
    {
        GameObject player = GameObject.Find("Amarillo");
        EngineerController script = player.GetComponent<EngineerController>();
        return script;
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

    #endregion

}
