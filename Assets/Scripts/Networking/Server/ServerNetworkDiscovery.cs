using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerNetworkDiscovery : NetworkDiscovery
{

    private int port;
    private int socketId;

    private static int minPort = 7777;
    private static int maxPort = 8888;

    public void Start()
    {
        Initialize();
    }

    public void InitializeBroadcast()
    {
        if (StartAsServer())
        {
            Debug.Log("Server started broadcasting locally");
        }
    }

    public void ResetServer()
    {
        StopBroadcast();
        StartAsServer();
    }

    public int[] CreateServer(HostTopology topology)
    {
        int[] connectionData = new int[2] { -1, -1 };

        for (int portNumber = minPort; portNumber <= maxPort; portNumber++)
        {
            socketId = NetworkTransport.AddHost(topology, portNumber);

            if (socketId != -1)
            {
                port = portNumber;

                connectionData[0] = port;
                connectionData[1] = socketId;

                broadcastData = "port/" + port;
               
                break;
            }

        }

        return connectionData;
    }

}