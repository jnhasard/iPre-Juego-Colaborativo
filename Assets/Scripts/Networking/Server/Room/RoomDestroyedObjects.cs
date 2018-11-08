using System.Collections.Generic;

public class RoomDestroyedObjects
{

    #region Attributes

    HashSet<string> destroyedObjects;

    #endregion

    #region Costructor

    public RoomDestroyedObjects()
    {
        destroyedObjects = new HashSet<string>();
    }

    #endregion

    #region Common

    public void AddObjectDestroyed(string name)
    {
        if (!destroyedObjects.Contains(name))
        {
            destroyedObjects.Add(name);
        }
    }

    public void Reset()
    {
        destroyedObjects = new HashSet<string>();
    }

    public List<string> GetObjectMessages()

    {
        List<string> messages = new List<string>();
        foreach (string objectDestroyed in destroyedObjects)
        {
            string message = "ObjectDestroyed/" + objectDestroyed;
            messages.Add(message);
        }
        return messages;
    }

#endregion
}