using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchForLoopDestroyer : MonoBehaviour {

    public string[] switchesNeeded;
    public string objectToFind;
    private int switchesActivated;


    // Use this for initialization
    void Start ()
    {
        switchesActivated = 0;
    }

    public void SwitchReady(string incomingSwitch)
    {
        // Este código se llama desde los SwitchActions de la escena. 

        for (int i = 0; i<switchesNeeded.Length;i++)
        {
            if (switchesNeeded[i] == incomingSwitch)
            {   
                switchesNeeded[i] = null;
                switchesActivated++;
            }
        }

        if (switchesActivated == switchesNeeded.Length)
        {
            ForLoopDestroyer fDestroyer = GameObject.Find(objectToFind).GetComponent<ForLoopDestroyer>();
            fDestroyer.DestroyOneMoreObject();
        }
    }
}
