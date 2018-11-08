using System;
using UnityEngine;

public class NetworkPlayer
{

    #region Attributes

    public string[] inventory = new string[8];
    public Room room;

    public bool controlOverEnemies;
    public int connectionId;
    public string ipAddress;
    public float positionX;
    public float positionY;
    public bool isGrounded;
    public int directionX;
    public int directionY;
    public bool connected;
    public float speedX;
    public int id;
    public bool power;

    public bool pressingJump;
    public bool pressingRight;
    public bool pressingLeft;
    public bool attacking;

    #endregion

    #region Constructor

    public NetworkPlayer(int connectionId, int playerId, Room room, string address)
    {
        this.connectionId = connectionId;
        this.ipAddress = address;
        this.id = playerId;
        this.room = room;

        controlOverEnemies = false;
        isGrounded = false;
        attacking = false;
        connected = true;
        power = false;
        pressingJump = false;
        pressingRight = false;
        pressingLeft = false;
        positionX = room.GetStartPosition()[0];  
        positionY = room.GetStartPosition()[1];
        speedX = 0;
        directionX = 1;
        directionY = 1;
        SendDataToRoomBoxManager();
    }

    public void SendDataToRoomBoxManager()
    {
        GameObject roomManager = GameObject.FindGameObjectWithTag("RoomManager");
        roomManager.GetComponent<RoomManager>().AddNewPlayerToRoom(getPlayerEnum(), connectionId, room);
    }
    #endregion

    #region Common
    public RoomBox.PlayersID getPlayerEnum()
    {
        switch (id)
        {
            case 0:
                return RoomBox.PlayersID.Mage;
            case 1:
                return RoomBox.PlayersID.Warrior;
            default:
                return RoomBox.PlayersID.Engineer;
        }
    }
    public void InventoryUpdate(string message)
    {
        char[] separator = new char[1];
        separator[0] = '/';
        string[] msg = message.Split(separator);
        int index = Int32.Parse(msg[2]);

        if (msg[1] == "Add")
        {
            AddItemToInventory(index, msg[3]);
        }
        else
        {
            RemoveItemFromInventory(index);
        }
    }

    public string GetReconnectData()
    {
        return "PlayerChangePosition/" +
           id + "/" +
           positionX + "/" +
           positionY + "/" +
           directionX + "/" +
           directionY + "/" +
           speedX + "/" +
           isGrounded + "/" +
           false + "/" +
           false + "/" +
           false;
    }

    #endregion

    #region Utils

    private void AddItemToInventory(int index, string spriteName)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (i == index)
            {
                inventory[i] = spriteName;
                return;
            }
        }
    }

    private void RemoveItemFromInventory(int index)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (i == index)
            {
                inventory[i] = null;
                return;
            }
        }
    }

    #endregion

}
