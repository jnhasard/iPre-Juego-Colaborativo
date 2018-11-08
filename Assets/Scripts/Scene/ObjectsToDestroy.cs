using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsToDestroy : MonoBehaviour {

    public GameObject[] objectsToBurn;
	// Use this for initialization
	void Start () {
		if (objectsToBurn.Length == 0)
        {
            Debug.Log("ObjectBurner named: " + gameObject.name + " needs objects toDestroy, duh");
        }
	}
	
    public void BurnAllThisStuff()
    {
        foreach (GameObject gObject in objectsToBurn)
        {
            if (gObject.GetComponent<DestroyableObject>())
            {
                gObject.GetComponent<DestroyableObject>().DestroyMe(true);
            }
        }
    }
}
