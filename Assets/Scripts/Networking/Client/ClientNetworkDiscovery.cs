using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ClientNetworkDiscovery : NetworkDiscovery
{

    #region Attributes

    private Client client;

    #endregion

    #region Start   

    void Start()
    {
        Initialize();
        client = GetComponent<Client>();
    }

    #endregion

    #region Common

    public void InitializeListening()
    {

        if (StartAsClient())
        {
            Debug.Log("Client started listenting locally");
            GameObject.Find("ConnectText").GetComponent<Text>().text = "Conectando...";
        }

    }

    #endregion

    #region Events

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        char[] separator = new char[1] { '/' };

        string[] parsedData = data.Split(separator);
        string serverIp = fromAddress;
        int port = -1;

        if (parsedData.Length > 0 && parsedData[0] == "port" && parsedData[1] != null)
        {
            port = int.Parse(parsedData[1]);
            Debug.Log("Client received broadcast from " + serverIp + ":" + port);
        }

        if (port != -1)
        {
            bool connected = false;

            while (!connected)
            {
                connected = client.Connect(serverIp, port);
            }

            GameObject.Find("ConnectText").GetComponent<Text>().text = "Esperando...";
            StopBroadcast();
        }

    }

    #endregion

}
