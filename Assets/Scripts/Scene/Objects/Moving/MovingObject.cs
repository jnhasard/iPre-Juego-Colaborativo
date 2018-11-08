using UnityEngine;
using System.Collections;

public class MovingObject : MonoBehaviour
{

    #region Attributes

    private Vector2 startPoint;
    public Vector2 endPoint;

    public bool hasParasite;
    public string parasiteName;
    public bool dontCollideWithPlayers;
    public float moveSpeed;
    public float timeToWait;

    public bool isWorking;
    public bool playerHasReturned;
    private Vector2 currentTarget;
    private GameObject parasite;
    private Transform pTransform;



    #endregion

    #region Start & Update

    protected virtual void Start()
    {
        IgnoreCollisionWithDestroyables();
        CheckVariables();

        if (hasParasite)
        {
            parasite = (GameObject)Instantiate(Resources.Load("Prefabs/Powerables/" + parasiteName));
            pTransform = parasite.transform;
            PowerableObject pObject = gameObject.GetComponent<PowerableObject>();
            pObject.powers[0].particles[0] = parasite;
        }


        isWorking = true;

        if (dontCollideWithPlayers)
        {
            IgnoreCollisionWithPlayers(true);
        }

        if (endPoint != null)
        {
            currentTarget = endPoint;
        }

    }

    void Update()
    {
        if (currentTarget.Equals(default(Vector2)))
        {
            return;
        }

        if (endPoint.Equals(default(Vector2)))
        {
            return;
        }

        if (Vector2.Distance(transform.position, endPoint) <= 0f)
        {
            StartCoroutine(WaitForPlayer(timeToWait, startPoint));
        }

        if (Vector2.Distance(transform.position, startPoint) <= 0f)
        {
            StartCoroutine(WaitForPlayer(timeToWait, endPoint));
        }

        transform.position = Vector2.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime);
        if (hasParasite)
        {
            if (parasite != null)
            {
                pTransform.position = gameObject.transform.position;
            }
        }

        if (playerHasReturned)
        {
            CoordinateMovingObject();
        }
    }

    #endregion

    #region Common

    private void CoordinateMovingObject()
    {
        string x = transform.position.x.ToString();
        string y = transform.position.y.ToString();
        string targetX = currentTarget.x.ToString();
        string targetY = currentTarget.y.ToString();

        string message = "CoordinateMovingObject/" + name + "/" + x + "/" + y + "/" + targetX + "/" +  targetY;
        SendMessageToServer(message, true);
        playerHasReturned = false;
    }


    public void SetData(Vector2 start, Vector2 end, float speed, float timeIllWait, bool ignoreCollisionWithPlayers)
    {
        startPoint = start;
        endPoint = end;
        currentTarget = end;
        moveSpeed = speed;
        timeToWait = timeIllWait;
        IgnoreCollisionWithPlayers(ignoreCollisionWithPlayers);
    }

    public void SetData(Vector2 start, Vector2 end, float speed)
    {
        SetData(start, end);
        moveSpeed = speed;
    }

    public void SetData(Vector2 start, Vector2 end)
    {
        startPoint = start;
        endPoint = end;
    }

    public void HandlePlayerReturned(string[] msg)
    {
        float _x = float.Parse(msg[2]);
        float _y = float.Parse(msg[3]);

        float targetX = float.Parse(msg[4]);
        float targetY = float.Parse(msg[5]);

        gameObject.transform.position = new Vector3(_x, _y, transform.position.z);
        currentTarget = new Vector2(targetX, targetY);
    }

    #endregion

    #region Utils

    private void IgnoreCollisionWithPlayers(bool ignore)
    {
        Collider2D collider = GetComponent<Collider2D>();

        GameObject player1 = GameObject.Find("Verde");
        GameObject player2 = GameObject.Find("Rojo");
        GameObject player3 = GameObject.Find("Amarillo");
        Physics2D.IgnoreCollision(collider, player1.GetComponent<Collider2D>(), ignore);
        Physics2D.IgnoreCollision(collider, player2.GetComponent<Collider2D>(), ignore);
        Physics2D.IgnoreCollision(collider, player3.GetComponent<Collider2D>(), ignore);
    }

    private void IgnoreCollisionWithDestroyables()
    {
        Collider2D myCollider = gameObject.GetComponent<Collider2D>();
        DestroyableObject[] destroyables = FindObjectsOfType<DestroyableObject>();
        foreach (DestroyableObject destroyable in destroyables)
        {
            Physics2D.IgnoreCollision(myCollider, destroyable.GetComponent<Collider2D>(), true);
        }
    }

    protected bool GameObjectIsPlayer(GameObject other)
    {
        return other.GetComponent<PlayerController>();
    }

    #endregion

    #region Events

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

    protected void CheckVariables()
    {
        startPoint = gameObject.transform.position;

        if (endPoint == new Vector2(0f, 0f))
        {
            Debug.LogError("The Moving object: " + gameObject.name + " has no end point");
        }

        if (moveSpeed == 0)
        {
            Debug.LogError("The Moving object: " + gameObject.name + " has no movementSpeed");
        }

        if (timeToWait == 0)
        {
            Debug.LogError("The Moving object: " + gameObject.name + " has no waiting Time");
        }

        if (hasParasite)
        {
            if (parasiteName.Equals(default(string)))
            {
                Debug.LogError("The Moving Object: " + gameObject.name + "is Set for Parasite but has no parasite name set");
            }
        }
    }

    protected IEnumerator WaitForPlayer(float timeToWait, Vector2 newTarget)
    {
        yield return new WaitForSeconds(timeToWait);

        currentTarget = newTarget;

    }



    private void SendMessageToServer(string message, bool secure)
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, secure);
        }
    }
    #endregion

}
