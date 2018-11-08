using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorTeleportAssigner : MonoBehaviour {


    public string[] destinies; 
    public Dictionary<string, Vector2> teleporterDestiny;

	// Use this for initialization
	void Start () {

        teleporterDestiny = new Dictionary<string, Vector2>();

        for (int i = 0; i < destinies.Length; i++)
        {
            GameObject destiny = GameObject.Find(destinies[i]);
            if (destiny != null)
            {
                AssignVectorValue(destinies[i]);
            }
        }
    }
	
	// Update is called once per frame

    public void AssignVectorValue(string nameOfDestiny)
    {
        if (GameObject.Find(nameOfDestiny))
        {
            if (teleporterDestiny.ContainsKey(nameOfDestiny))
            {
                return;
            }

            else
            {
                GameObject destination = GameObject.Find(nameOfDestiny);
                Vector2 vector = destination.GetComponent<Transform>().position;
                teleporterDestiny.Add(nameOfDestiny, vector);
            }
        }
    }

    public Vector2 WhereAmIGoing(string teleportDestination)
    {
        Vector2 v2 = teleporterDestiny[teleportDestination];
        return v2;
    }
}
