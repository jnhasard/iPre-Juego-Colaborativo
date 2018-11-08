using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour {
    List<RoomBox> roomBoxs;
    public int maxRooms = 15;
    public enum Player { None, Mage, Warrior, Engineer };
    int activeRooms;
    private bool[] occupiedRooms;
    public float firstRoomRight = 0;
    public float firstRoomLeft = 0;
    public float firstRoomTop = 0;
    public float firstRoomBottom = 0;
    public float deltaX = 0;
    public float deltaY = 0;
    // Use this for initialization
    void Start() {
        roomBoxs = new List<RoomBox>();
        activeRooms = 0;
        occupiedRooms = new bool[maxRooms];
    }

    //Return -1 si encuentra error.
    private int GetFreePosition()
    {
        for(int i=0; i < maxRooms; i++)
        {
            if(occupiedRooms[i] == false)
            {
                occupiedRooms[i] = true;
                return i;
            }
        }
        return -1; 
    }

    public void AddNewRoom(Room room)
    {
        if (activeRooms > maxRooms)
        {
            Debug.LogError("No es posible mostrar mas de 15 rooms en la interfaz. Se intentó agregar room numero " + activeRooms);
            //TODO: LLevar la cuenta de los rooms a nivel de servidor para que no puedan existir más de maxNumber.
        }
        int id = room.id;
        activeRooms++;
        RoomBox newRoom = new RoomBox(id, GetFreePosition(), room);
        roomBoxs.Add(newRoom);
        UpdateGUI(newRoom);
    }

    public bool AddNewPlayerToRoom(RoomBox.PlayersID player, int connectionId, Room room)
    {
        RoomBox roomBox = GetRoomBoxFromRoom(room);
        if (roomBox != null)
        {
            roomBox.AddPlayer(player,connectionId);
            UpdateGUI(roomBox);
            return true;
        }
        return false;
    }

    public bool DeletePlayerFromRoom(int connectionId, Room room)
    {
        RoomBox roomBox = GetRoomBoxFromRoom(room);
        if (roomBox != null)
        {
            //occupiedRooms[roomBox.boxId] = false;
            roomBox.DeletePlayer(connectionId);
            UpdateGUI(roomBox);
            return true;
        }
        return false;
    }

    private RoomBox GetRoomBoxFromRoom(Room room)
    {
        foreach(RoomBox box in roomBoxs)
        {
            if(box.roomId == room.id)
            {
                return box;
            }
        }
        return null;
    }

    public Room GetRoomFromRoomBox(int roomboxId)
    {
        foreach(RoomBox roombox in roomBoxs)
        {
            if(roombox.boxId == roomboxId)
            {
                return roombox.room;
            }
        }
        return null;
    }

    public void FreeSpace(int boxId)
    {
        occupiedRooms[boxId] = false;
        SetBlankGUI(boxId);
    }


    private void SetBlankGUI(int boxId)
    {
        GameObject box = GameObject.Find("RoomResetButton" + boxId);
        Text boxText = box.GetComponentInChildren<Text>();
        boxText.text = "Empty";
    }

    private void UpdateGUI(RoomBox room)
    {
        GameObject box = GameObject.Find("ResetButtonText" + room.boxId);
        Text boxText = box.GetComponent<Text>();
        boxText.text = "RESET" + room.boxId +"\n";
        foreach(RoomBox.PlayersID player in room.currentPlayers.Values)
        {
            boxText.text += player.ToString()[0] + " ";
        }
        //TODO: Hacer que se muestre el room
    }

}
