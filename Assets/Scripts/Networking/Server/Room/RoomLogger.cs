using System.IO;
using System;

public class RoomLogger
{

    #region Attributes

    int roomId;
    DateTime lastPOIP; //One for each player
    #endregion

    #region Constructor

    public RoomLogger(int id)
    {
        this.roomId = id;
        lastPOIP = GetTimeAsDateTime();
    }

    #endregion

    #region Common

    public void WriteAttack(int playerId)
    {
        StreamWriter writer = GetWriter();
        writer.WriteLine(GetTime() + " Player " + playerId + " attacked\n");
        writer.Close();
    }

    public void WritePower(int playerId, bool powerState)
    {
        StreamWriter writer = GetWriter();
        if (powerState)
        {
            writer.WriteLine(GetTime() + " Player " + playerId + " used his power\n");
        }
        else
        {
            writer.WriteLine(GetTime() + " Player " + playerId + " stopped using his power\n");
        }
        writer.Close();
    }

    public void WriteEnterPOI(int playerId, string poiId)
    {
        StreamWriter writer = GetWriter();
        writer.WriteLine(GetTime() + " Poi number: " + poiId + " was reached by Player" + playerId);
        writer.Close();
    }

    public void WriteEnterPOIButDontCare(int playerId, string poiId)
    {
        StreamWriter writer = GetWriter();
        writer.WriteLine(GetTime() + " Player: " + playerId +  "reached Poi number: " + poiId + ". Not relevant or Already Entered to Poi");
        writer.Close();
    }

    public void WriteTotalExp(int exp)
    {
        StreamWriter writer = GetWriter();
        writer.WriteLine(GetTime() + " CHANGING SCENE!  The players gathered " + exp + " experience points");
        writer.Close();
    }

    public void WritePlayerIsCharging(int playerId)
    {
        StreamWriter writer = GetWriter();
        writer.WriteLine(GetTime() + "Player " + playerId + " is Charging");
        writer.Close();
    }

    public void WritePlayernotCharging(int playerId)
    {
        StreamWriter writer = GetWriter();
        writer.WriteLine(GetTime() + "Player " + playerId + " Stoped Charging");
        writer.Close();
    }

    public void WritePoiIsReady(int playerID, string poiID)
    {
        StreamWriter writer = GetWriter();
        TimeSpan spans = GetTimeAsDateTime().Subtract(lastPOIP);
        writer.WriteLine(GetTime() +" Poi number: " + poiID + " was reached by all the necessary players. Seconds spent reaching this POI from the last one was: " + spans.TotalSeconds);
        lastPOIP = GetTimeAsDateTime();
        writer.Close();
    }

    public void WritePlayersAreDead()
    {
        StreamWriter writer = GetWriter();
        writer.WriteLine(GetTime() + " PLAYERS ARE DEAD!!");
        writer.Close();
    }

    public void WriteChangeScene(string sceneName)
    {
        StreamWriter writer = GetWriter();
        writer.WriteLine(GetTime() + " PLAYERS ARE STARTING SCENE: " + sceneName);
        writer.Close();
    }
    public void WriteNewLine()
    {
        StreamWriter writer = GetWriter();
        writer.WriteLine("");
        writer.Close();
    }


    //Modificar si se cambia el sistema de inventario
    public void WriteInventory(int playerId, string message)
    {
        char[] separator = new char[1];
        separator[0] = '/';
        string[] msg = message.Split(separator);
        int index = Int32.Parse(msg[2]);

        StreamWriter writer = GetWriter();

        if (msg[1] == "Add")
        {
            writer.WriteLine(GetTime() + " Player " + playerId + " picked stored " + msg[3] + " in the slot " + index +"\n");
        }
        else
        {
            writer.WriteLine(GetTime() + " Player " + playerId + " tossed item in slot " + index + "\n");
        }
        writer.Close();

    }


    public void WriteNewPosition(int playerId, float positionX, float positionY, bool pressingJump, bool pressingLeft, bool pressingRight)
    {
        StreamWriter writer = GetWriter();
        string line = "";
        if (pressingJump)
        {
            line = GetTime() + " Player " + playerId + " jumped from (" + positionX + "," + positionY + ")\n";
        }
        else if(pressingLeft && !pressingRight)
        {
            line = GetTime() + " Player " + playerId + " is going left from (" + positionX + "," + positionY + ")\n";
        }
        else if (!pressingLeft && pressingRight)
        {
            line = GetTime() + " Player " + playerId + " is going right from (" + positionX + "," + positionY + ")\n";
        }
        else if(pressingRight && pressingLeft)
        {
            line = GetTime() + " Player " + playerId + " is pressing right AND left while standing in (" + positionX + "," + positionY + ")\n";
        }
        else
        {
            writer.Close();
            return;
        }
        writer.WriteLine(line);
        writer.Close();
    }

    #endregion

    #region Utils

    private string GetTime(){
      return DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);        
    }

    private DateTime GetTimeAsDateTime()
    {
        return DateTime.Now;
    }

    private StreamWriter GetWriter()
    {
        return new StreamWriter(File.Open("Log_room_" + roomId + ".txt", FileMode.Append));
    }

    #endregion

}
