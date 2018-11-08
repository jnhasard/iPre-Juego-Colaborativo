using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class MovableTriggerInstantiator : MonoBehaviour {

	[Serializable]
	 
	public struct ObjectToInstantiate
	{
		public Vector2 position;
		public string name;
	}

	public ObjectToInstantiate[] instantiateObjects;
	public GameObject objectNeeded;
    public bool jobDone;

	private LevelManager levelManager; 

	void Start () {
	
		levelManager = FindObjectOfType<LevelManager>();
		if (instantiateObjects.Length == 0)
		{
			Debug.LogError (gameObject.name + " doesnt have instantiate values");
		}
	}
	
	// Update is called once per frame

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject == objectNeeded) 
		{
            InstantiateObjects(other.gameObject);
		}		
	}

    public void InstantiateObjects(GameObject other)
    {
        Destroy(other);
        foreach (ObjectToInstantiate instObject in instantiateObjects)
        {
            levelManager.InstantiatePrefab(instObject.name, instObject.position);
        }
        jobDone = true; 
    }
}
