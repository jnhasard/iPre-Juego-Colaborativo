using System.Collections.Generic;
using UnityEngine;

public class RoomBox
{
    public enum PlayersID { None, Mage, Warrior, Engineer };
    public int roomId;
    public int boxId;
    public Room room;
    public Dictionary<int, PlayersID> currentPlayers;
    public RoomBox(int id, int boxId, Room room)
    {
        this.room = room;
        roomId = id;
        this.boxId = boxId;
        currentPlayers = new Dictionary<int, PlayersID>();
    }

    public bool AddPlayer(PlayersID player, int connectionId)
    {
        if (currentPlayers.ContainsValue(player))
        {
            Debug.LogError("CRITICAL ERROR ENCONTRAMOS EL BUG: Se intentó agregar más de una vez un " + player + " en el room " + roomId);
        }
        if (!currentPlayers.ContainsKey(connectionId))
        {
            currentPlayers.Add(connectionId, player);
            return true;
        }
        else
        {
            Debug.LogError("Se intentó agregar dos veces al jugador con ip " + connectionId +" al room" + roomId);
            return false;
        }
    }

    public void DeletePlayer(int connectionId)
    {
        currentPlayers.Remove(connectionId);
    }
}