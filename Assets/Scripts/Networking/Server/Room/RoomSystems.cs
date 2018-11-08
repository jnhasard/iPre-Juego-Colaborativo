using System.Collections.Generic;

public class RoomSystems
{

    #region Attributes

    HashSet<string> activatedSystems;

    #endregion

    #region Costructor

    public RoomSystems()
    {
        activatedSystems = new HashSet<string>();
    }

    #endregion

    #region Common

    public void AddSystem(string name)
    {
        activatedSystems.Add(name);
    }

    public void Reset()
    {
        activatedSystems = new HashSet<string>();
    }

    public List<string> GetSystemsMessages()
    {
        List<string> messages = new List<string>();

        foreach (string system in activatedSystems)
        {
            string message = "ActivateSystem/" + system;
            messages.Add(message);
        }

        return messages;
    }

    #endregion

}
