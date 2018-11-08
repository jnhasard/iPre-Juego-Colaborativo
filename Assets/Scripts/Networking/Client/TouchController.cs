using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Hola Esto es un comentario

public class TouchController : MonoBehaviour {

    #region Events

    public void OnClick()
    {
        GameObject client = GameObject.Find("ClientObject");
        ClientNetworkDiscovery listen = client.GetComponent<ClientNetworkDiscovery>();
        GameObject.Find("ConnectText").GetComponent<Text>().text = "Conectar";
        listen.InitializeListening();
    }

    #endregion
}
