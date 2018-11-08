using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPanel : MonoBehaviour {

    #region Buttons

    public void ChatButton()
    {
        List<Room> rooms = Server.instance.rooms;
        foreach (Room room in rooms)
        {
            room.CreateTextChat();
        }
    }

    public void ServerButton()
    {
        Server.instance.InitializeBroadcast();
    }

    public void ResetServer()
    {
        Server.instance.Reset();
    }

    public void MaxPlayerRoomButton()
    {
        Text inputText = GameObject.Find("InputPlayerText").GetComponent<Text>();
        int number = Int32.Parse(inputText.text);
        Server.instance.maxPlayers = number;
    }

    public void SceneToLoadButton()
    {
        Text inputText = GameObject.Find("InputSceneText").GetComponent<Text>();
        Server.instance.sceneToLoad = "Escena" + inputText.text;
    }

    public void KillRoom(int boxId)
    {
        RoomManager roomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
        if (!roomManager)
        {
            UnityEngine.Debug.LogError("No se encontró RoomManager en ServerScene. uwu 3");
        }
        Server.instance.DestroyRoom(roomManager.GetRoomFromRoomBox(boxId));
        roomManager.FreeSpace(boxId);
    }

    public void ChangeSceneInRoom(int boxId)
    {
        Text inputText = GameObject.Find("InputText" + boxId).GetComponent<Text>();
        string sceneName = "Escena" + inputText.text;

        RoomManager roomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
        if (!roomManager)
        {
            UnityEngine.Debug.LogError("No se encontró RoomManager en ServerScene. uwu 2");
        }
        Server.instance.ChangeRoomScene(roomManager.GetRoomFromRoomBox(boxId), sceneName);
    }
    #endregion
}