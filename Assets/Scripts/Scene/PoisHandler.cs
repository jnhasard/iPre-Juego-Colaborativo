using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisHandler
{

    #region Attributes

    HashSet<string> poisReady;

    #endregion

    #region Costructor

    public PoisHandler()
    {
        poisReady = new HashSet<string>();
    }

    #endregion

    #region Common

    public void AddPoiReady(string name)
    {
        if (!poisReady.Contains(name))
        {
            poisReady.Add(name);
        }
    }

    public void Reset()
    {
        poisReady = new HashSet<string>();
    }

    public List<string> GetPoiMessages()

    {
        List<string> messages = new List<string>();
        foreach (string poiReady in poisReady)
        {
            string message = "ReadyPoi/" + poiReady;
            messages.Add(message);
        }
        return messages;
    }

    #endregion
}

