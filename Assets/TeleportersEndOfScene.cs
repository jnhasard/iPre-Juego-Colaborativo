using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportersEndOfScene : MonoBehaviour
{
    [Serializable]

    public struct InstantiatedPortal
    {
        public Vector2 portalPosition;
        public Vector2 portalDestination;
        public string portalType;
    }

    public InstantiatedPortal[] portals;
    public int gearsNeeded;
    private int currentGears;

    // Use this for initialization
    void Start()
    {
        CheckParameters();
        currentGears = 0;
    }

    private void CheckParameters()
    {
        if (gearsNeeded == 0)
        {
            Debug.LogError("teleporters end of scene named: " + gameObject.name + " needs the number of gearn needed");
        }
    }


    public void GearActivation()
    {
        currentGears++;
        Debug.LogError("Number of gears who activated is: " + currentGears);
        if (currentGears == gearsNeeded)
        {
            InstantiateEndOfScenePortals();
            currentGears = 0;
        }
        else if (currentGears>gearsNeeded)
        {
            currentGears = 0;
        }
    }

    private void InstantiateEndOfScenePortals()
    {
        LevelManager lManager = FindObjectOfType<LevelManager>();
        if (lManager)
        {
            for (int i = 0; i<portals.Length; i++)
            {
                lManager.InstantiatePortal(portals[i].portalType, portals[i].portalPosition, portals[i].portalDestination);
            }
        }
    }

}
