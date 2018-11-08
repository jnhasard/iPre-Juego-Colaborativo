using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableNameGiver : MonoBehaviour {

    public int lastId;
	// Use this for initialization
	void Start () {

        lastId = 0;
        DestroyableObject[] destroyables = FindObjectsOfType<DestroyableObject>();

        for (int i = 0; i < destroyables.Length; i++)
        {
            GameObject exp = destroyables[i].gameObject;
            string name = exp.name;
            name = name + i.ToString();
            exp.name = name;
            lastId = i;
        }
    }
}
