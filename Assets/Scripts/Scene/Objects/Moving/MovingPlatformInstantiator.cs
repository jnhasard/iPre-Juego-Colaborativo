using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformInstantiator : MonoBehaviour {


    GameObject[] platforms;
    public Vector2 initialPosition;
    public Vector2 targetPosition;
    public float instantiationRate;
    public float moveSpeed;
    public float timeBeforeKill;
    public bool onStart;

    public bool isWorking;
    public bool playerHasReturned;



    public string objectName;

    // Use this for initialization
    void Start()
    {
        playerHasReturned = false;
        initialPosition = gameObject.transform.position;
        CheckParameters();
        if (onStart)
        {
            StartTheCoRoutine();
        }
    }

    public void StartTheCoRoutine()
    {
        isWorking = true;
        StartCoroutine(InstantiatePlatform());
    }
    // Update is called once per frame
    private IEnumerator InstantiatePlatform()
    {
        while (true)
        {
            GameObject movingPlatform = (GameObject)Instantiate(Resources.Load("Prefabs/" + objectName));
            if (movingPlatform != null)
            {
                movingPlatform.transform.position = initialPosition;

                if (movingPlatform.GetComponent<OneTimeMovingObject>())
                {
                    StartMovingPlatform(movingPlatform);

                    if (playerHasReturned)
                    {
                        SendInstantiatorData();
                        playerHasReturned = false;
                    }
                }

                yield return new WaitForSeconds(instantiationRate);
            }
        }
    }

    private void StartMovingPlatform(GameObject movingPlatform)
    {
        OneTimeMovingObject movingVariables = movingPlatform.GetComponent<OneTimeMovingObject>();
        movingVariables.target = targetPosition;
        movingVariables.moveSpeed = moveSpeed;
        movingVariables.diesAtTheEnd = true;
        movingVariables.isPlatform = true;
        movingVariables.timeToWait = timeBeforeKill;
        StartCoroutine(WaitBeforeMove(timeBeforeKill, movingVariables));
    }

    private void CheckParameters()
    {
        if (initialPosition == new Vector2(0f, 0f))
        {
            Debug.LogError("MovingPlatformInstantiator: " + gameObject.name + " needs an Initial Position");
        } 

        if (targetPosition == new Vector2(0f, 0f))
        {
            Debug.LogError("MovingPlatformInstantiator: " + gameObject.name + " needs a Target Position");
        }

        if (timeBeforeKill == 0f) 
        {
            Debug.LogError("MovingPlatformInstantiator: " + gameObject.name + " needs has 0 waitBeforKill");
        }

        if (instantiationRate == 0f)
        {
            Debug.LogError("MovingPlatformInstantiator: " + gameObject.name + " needs an Instantiatio Rate");
        }

        if (moveSpeed == 0f)
        {
            Debug.LogError("MovingPlatformInstantiator: " + gameObject.name + " needs a MoveSpeed");
        }
    }

    private void SendMessageToServer(string message, bool secure)
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, secure);
        }
    }

    protected void SendInstantiatorData()
    {
        string message = "CoordinateInstantiators/" + name;
        SendMessageToServer(message, true);
    }

    public void HandleInstantiationSynch(string[] msg)
    {
        StartTheCoRoutine();
    }

    private IEnumerator WaitBeforeMove(float timeToWait, OneTimeMovingObject movingVariables)
    {
        yield return new WaitForSeconds(timeToWait);

        movingVariables.move = true;
    }
}
