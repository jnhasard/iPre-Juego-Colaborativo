using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInCircuitMovementController : MonoBehaviour
{

    public bool move;
    private Vector2[] targets;
    private Transform myTransform;
    private float moveSpeed;
    private float timeToWait;
    private int arrayNumber;
    public bool playerHasReturned;

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            transform.position = Vector2.MoveTowards(transform.position, targets[arrayNumber], moveSpeed * Time.deltaTime);
        }
    }

    public void InitializeCyclicMovements(Vector2[] incomingTargets, float _moveSpeed, float _timeToWait, int _arrayNumber)
    {
        string name = gameObject.name;
        moveSpeed = _moveSpeed;
        timeToWait = _timeToWait;
        arrayNumber = _arrayNumber;
        targets = incomingTargets;
        move = true;
        if(gameObject.activeInHierarchy)
        {
            StartCoroutine(StartMoving());
        }
    }

    protected virtual IEnumerator StartMoving()
    {
        while (true)
        {
            if (transform.position.x == targets[arrayNumber].x && transform.position.y == targets[arrayNumber].y)
            {
                arrayNumber++;
                if (arrayNumber == targets.Length)
                {
                    arrayNumber = 0;
                }
                if (playerHasReturned)
                {
                    SendObjectInCircuitDataToServer();
                    playerHasReturned = false;
                }
            }
            yield return new WaitForSeconds(timeToWait);
        }

    }

    protected void OnCollisionEnter2D(Collision2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            other.transform.parent = transform;
        }
    }

    protected void OnCollisionExit2D(Collision2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            other.transform.parent = null;
        }
    }

    protected bool GameObjectIsPlayer(GameObject other)
    {
        return other.GetComponent<PlayerController>();
    }

    protected void SendObjectInCircuitDataToServer()
    {
        Transform transform = gameObject.GetComponent<Transform>();
        int id = gameObject.GetInstanceID();

        string message = "CoordinateObjectInCircuit/" + id + "/" + transform.position.x + "/" + transform.position.y + "/" + arrayNumber;

        SendMessageToServer(message, true);
    }

    private void SendMessageToServer(string message, bool secure)
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, secure);
        }
    }

    public void HandleMovingInCircuitObjectData(string[] msg)
    {
        myTransform.position = new Vector2(float.Parse(msg[2]), float.Parse(msg[3]));
        arrayNumber = int.Parse(msg[4]);
        StartMoving();
    }

}

