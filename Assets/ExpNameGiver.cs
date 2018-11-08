using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpNameGiver : MonoBehaviour
{

    public int lastId;

    void Start()
    {

        lastId = 0;
        PickUpExp[] exps = FindObjectsOfType<PickUpExp>();

        for (int i = 0; i < exps.Length; i++)
        {
            GameObject exp = exps[i].gameObject;
            string name = exp.name;
            name = name + i.ToString();
            exp.name = name;
            lastId = i;
        }
    }
}
