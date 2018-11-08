using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitBuilder : MonoBehaviour {


    public GameObject[] objectsToRotate;
    public float _moveSpeed;
    public float _timeToWait;
    protected Vector2[] objectPositions;


    // Use this for initialization

    void Start() {
        CheckParameters();
        SetStartPositionsInCircuit();
        InitializeMovementInEachObject();
    }

    private void InitializeMovementInEachObject()
    {
        for (int i = 0; i < objectsToRotate.Length; i++)
        {
            ObjectInCircuitMovementController cmController = objectsToRotate[i].GetComponent<ObjectInCircuitMovementController>();
            cmController.InitializeCyclicMovements(objectPositions, _moveSpeed, _timeToWait, i);
        }

    }

    private void SetStartPositionsInCircuit()
    {
        objectPositions = new Vector2[objectsToRotate.Length];

        for (int i = 0; i < objectsToRotate.Length; i++)
        {
            Transform actualObject = objectsToRotate[i].GetComponent<Transform>();
            objectPositions[i] = actualObject.transform.position;
        }
    }

    private void CheckParameters()
    {
        if (objectsToRotate.Equals(default(GameObject[])))
        {
            Debug.LogError("Platform Rotator named: " + gameObject.name + " needs Objects to Start");
            return;
        }

        if (_moveSpeed.Equals(default(float)))
        {
            Debug.LogError("Platform Rotator named: " + gameObject.name + " needs a moveSpeed to Start");
            return;
        }
    }
    private void ChangeObjectsPositionsOrderInArray()
    {
        Vector2 savedPosition = objectPositions[0];

        for (int i = 0; i<objectPositions.Length; i++)
        {
            int j = i + 1;
            if (j < objectPositions.Length)
            {
                objectPositions[i] = objectPositions[j];
            }
            else if (j >= objectPositions.Length)
            {
                objectPositions[i] = savedPosition;
            }
        }
    }
}
