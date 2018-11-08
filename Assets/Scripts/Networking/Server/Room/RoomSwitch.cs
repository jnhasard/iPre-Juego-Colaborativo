
public class RoomSwitch
{

    #region Attributes

    public bool on;
    public int groupId;
    public int individualId;
    public Room room;

    #endregion

    #region Constructor

    public RoomSwitch(int groupId, int individualId, Room room)
    {
        this.groupId = groupId;
        this.individualId = individualId;
        this.room = room;
    }

    #endregion

    #region Common

    public string GetReconnectData()
    {
        return "ChangeSwitchStatus/" + groupId + "/" + individualId + "/" + on;
    }

    #endregion

}
