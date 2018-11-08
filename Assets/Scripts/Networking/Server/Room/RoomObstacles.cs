using System.Collections.Generic;

public class RoomObstacles
{

    #region Attributes

    HashSet<string> destroyedObstacles;

    #endregion

    #region Costructor

    public RoomObstacles()
    {
        destroyedObstacles = new HashSet<string>();
    }

    #endregion

    #region Common

    public void AddObstacle(string name)
    {
        if (!destroyedObstacles.Contains(name))
        {
            destroyedObstacles.Add(name);
        }
    }

    public void Reset()
    {
        destroyedObstacles = new HashSet<string>();
    }

    public List<string> GetObstaclesMessages()

    {
        List<string> messages = new List<string>();
        foreach (string obstacle in destroyedObstacles)
        {
            string message = "DestroyObject/" + obstacle;
            messages.Add(message);
        }
        return messages;
    }

    #endregion

}
