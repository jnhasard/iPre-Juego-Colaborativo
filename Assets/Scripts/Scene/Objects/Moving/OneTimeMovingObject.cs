using UnityEngine;

public class OneTimeMovingObject : MonoBehaviour
{

    #region Attributes

    public Vector3 target;

    public float moveSpeed;
    public bool move;
    public bool diesAtTheEnd;
    public bool isPlatform;
    public float timeToWait = 0;
    public bool needsParticles;
    protected bool noMorePlayers;



    private GameObject[] myParasiteParticles;
    private PlayerController[] playerControllers;

    #endregion

    private void Start()
    {
        noMorePlayers = false;
        playerControllers = new PlayerController[3];
    }

    #region Update

    void Update()
    {
        if (move)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, target, moveSpeed * Time.deltaTime);

            if (needsParticles)
            {
                foreach (GameObject parasiteParticle in myParasiteParticles)
                {
                    if (parasiteParticle != null)
                    {
                        parasiteParticle.transform.position = gameObject.transform.position;
                    }
                }

            }

            if (diesAtTheEnd)
            {
                if (gameObject.transform.position == target)
                {
                    if (isPlatform)
                    {
                        for (int i = 0; i < playerControllers.Length; i++)
                        {
                            if (playerControllers[i] != null)
                            {
                                PlayerController playerToRelease = playerControllers[i];
                                playerToRelease.ResetTransform();
                            }
                        }
                        noMorePlayers = true;
                        //Destroy(gameObject, timeToWait);
                        gameObject.SetActive(false);
                        return;
                    }
                    if (needsParticles)
                    {
                        for (int i = 0; i  <myParasiteParticles.Length;i++ )
                        {
                            Destroy(myParasiteParticles[i]);
                            myParasiteParticles[i] = null; 
                        }
                    }
                    Destroy(gameObject);
                }
            }

            if (moveSpeed == 0)
            {
                Debug.Log("la velocidad de " + gameObject.name + " es 0, no se va a mover");
            }

        }

    }

    #endregion

    protected void OnCollisionEnter2D(Collision2D other)
    {
        if (isPlatform)
        {
            if (noMorePlayers)
            {
                return;
            }
            if (GameObjectIsPlayer(other.gameObject))
            {
                PlayerController player = other.gameObject.GetComponent<PlayerController>();
                int i = player.playerId;
                playerControllers[i] = player;

                playerControllers[i].parent = gameObject;
                other.transform.parent = transform;
            }
        }
    }

    protected void OnCollisionExit2D(Collision2D other)
    {
        if (isPlatform)
        {
            if (GameObjectIsPlayer(other.gameObject))
            {
                other.transform.parent = null;
            }
        }
    }

    protected bool GameObjectIsPlayer(GameObject other)
    {
        return other.GetComponent<PlayerController>();
    }

    public void SetParasiteParticles(GameObject[] parasiteParticles)
    {
        myParasiteParticles = new GameObject[3] { parasiteParticles[0], parasiteParticles[1], parasiteParticles[2] };
    }

    public void DestroyParasiteParticles()
    {
        foreach (GameObject particle in myParasiteParticles)
        {
            if (particle != null)
            {
                Destroy(particle, .1f);
            }
        }
    }

}
