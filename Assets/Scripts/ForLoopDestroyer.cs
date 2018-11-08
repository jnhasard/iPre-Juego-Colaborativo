using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForLoopDestroyer : MonoBehaviour {

    public GameObject[] objectsToDestroy;
    public int objectsDestroyed;
	// Use this for initialization


	private void Start () {
        CheckParameters();
        objectsDestroyed = 0;
	}

    public void DestroyOneMoreObject()
    {
        for (int i = objectsDestroyed; i<objectsToDestroy.Length; i++)
        {
            if (objectsToDestroy[i] != null)
            {
                objectsDestroyed++;
                Destroy(objectsToDestroy[i]);
                break;
            } 
        }
    }

    private void CheckParameters()
    {
        if (objectsToDestroy.Length == 0)
        {
            Debug.LogError("The ForLoopDestroyer named: " + gameObject.name + "has no objects to destroy");
        }
    }


}
