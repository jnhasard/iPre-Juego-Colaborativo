using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    #region Attributes

    public Vector2[] patrollingPoints;
    public CircleCollider2D alertZone;
    public GameObject[] particles;


    public float patrollingSpeed;
    public bool patrolling;
    public bool fromEditor;
    public int directionX;  // 1 = right, -1 = left
    public float poweredTime;
    public bool maged;
    public Vector2 strenght;

    protected Dictionary<string, bool> ignoresCollisions;
    protected Vector2 currentPatrolPoint;
    protected LevelManager levelManager;
    protected SceneAnimator sceneAnimator;
    protected Rigidbody2D rb2d;
    protected Vector2 force;
    protected bool playerHasReturned;

    protected int currentPatrolPointCount;
    protected float maxHp = 100f;
    protected int debuger = 0;
    protected int damage = 0;
    protected int enemyId;
    protected float hp;
    protected float timeForAttack = 1.5f;
    protected float timeMaged;
    protected bool deathIsComing;

    protected static float alertDistanceFactor = 1.5f;
    protected static float maxXSpeed = .5f;
    protected static float WaitToDie = 1f;
    protected static float maxYSpeed = 0f;

    #endregion

    #region Start & Update  

    protected virtual void Start()
    {
        sceneAnimator = FindObjectOfType<SceneAnimator>();
        levelManager = FindObjectOfType<LevelManager>();
        rb2d = GetComponent<Rigidbody2D>();
        InitializeParticles();
        maged = false;
        deathIsComing = false;
        ignoresCollisions = new Dictionary<string, bool> { { "Verde", false }, { "Rojo", false }, { "Amarillo", false } };

        currentPatrolPointCount = 0;

        if (patrollingSpeed.Equals(default(float)))
        {
            Debug.LogError("This Enemy has no speed set");
        }

        patrollingSpeed = patrollingSpeed * 0.04f;
        directionX = -1;
        hp = maxHp;

        if (patrollingPoints != null && patrollingPoints.Length > 0)
        {
            NextPatrollingPoint();
        }

    }

    protected virtual void Update()
    {
        if (maged)
        {
            timeMaged++;
            Debug.Log(timeMaged);
            if (timeMaged == poweredTime)
            {
                UnmageThisEnemy();
            }
        }
    }

    #endregion

    #region Common

    protected void Attack(GameObject player)
    {
        sceneAnimator.StartAnimation("Attacking", this.gameObject);
        DealDamage(player);
    }

    protected virtual void Patroll()
    {
        if (!rb2d)
        {
            Debug.Log("If your are planning to move an enemy it should have a RigidBody2D");
            return;
        }

        if (Vector2.Distance(transform.position, currentPatrolPoint) < .1f)
        {
            NextPatrollingPoint();
        }

        if (playerHasReturned)
        {
            if (LocalPlayerHasControl())
            {
                SendPatrollingPoint();
                playerHasReturned = false;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, currentPatrolPoint, patrollingSpeed);
    }

    public virtual void TakeDamage(float damage)
    {
        sceneAnimator.StartAnimation("TakingDamage", gameObject);
        hp -= damage;
        Debug.Log(name + " took " + damage + " damage -> " + hp + "/" + maxHp);

        if (hp <= 0)
        {
            if (LocalPlayerHasControl())
            {
                SendEnemyDiedToServer(damage);
            }

            Die();
        }

    }

    public virtual void GetThisEnemyMaged()
    {
        if (maged)
        {
            timeMaged = 0;
            return;
        }
        if (!maged)
        {
            maged = true;

            ToggleParticles(true, 0);
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = false;
            }
        }
    }

    public void UnmageThisEnemy()
    {
        ToggleParticles(false, 0);
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = true;
        }
        maged = false;
        timeMaged = 0;
    }

    public virtual void UpdateCollisionsWithPlayer(GameObject player, bool ignores)
    {
        foreach (Collider2D collider in GetComponents<Collider2D>())
        {
            if (!collider.isTrigger)
            {
                Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), collider, ignores);
            }
        }

        ignoresCollisions[player.name] = ignores;
        if (LocalPlayerHasControl())
        {
            SendIgnoreCollisionDataToServer(player, ignores);
        }
    }

    public void ThePlayerReturned(bool thePlayerHasReturned)
    {
        playerHasReturned = thePlayerHasReturned;
    }

    protected virtual void DealDamage(GameObject player)
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        PlayerController playerController = player.GetComponent<PlayerController>();
        MageController mage = levelManager.GetMage();

        Vector2 playerPosition = player.transform.position;
        Vector2 attackForce = force;


        // Don't hit protected players
        if (mage.ProtectedByShield(player))
        {
            if (!ignoresCollisions[player.name])
            {
                UpdateCollisionsWithPlayer(player, true);
            }
            return;
        }
        else
        {
            if (ignoresCollisions[player.name])
            {
                UpdateCollisionsWithPlayer(player, false);
            }
        }

        // Only hit local players
        if (!playerController.localPlayer)
        {
            return;
        }

        // If player is at the left side of the enemy push it to the left
        if (playerPosition.x < transform.position.x)
        {
            attackForce.x *= -1;
        }


        playerController.TakeDamage(damage, attackForce);
    }


    public void Die()
    {
        if (deathIsComing)
        {
            return;
        }
        sceneAnimator.StartAnimation("Dying", gameObject);
        deathIsComing = true;
        StartCoroutine(WaitDying());
    }

    #endregion

    #region Setters & Getters

    public void Register(int enemyId)
    {
        this.enemyId = enemyId;

        string message = "EnemyRegisterId/" +
            gameObject.GetInstanceID() + "/" +
            enemyId + "/" +
            maxHp + "/" +
            directionX + "/" +
            transform.position.x + "/" +
            transform.position.y;

        if (patrollingPoints != null && patrollingPoints.Length > 0)
        {
            message += ("/" + patrollingPoints[0].x + "/" + patrollingPoints[0].y);
        }

        SendMessageToServer(message, true);
    }

    public void Initialize(int enemyId, int directionX, float posX, float posY)
    {
        this.enemyId = enemyId;
        SetPosition(directionX, posX, posY);
        Debug.Log("Enemy " + enemyId + " starting position " + posX + "," + posY);
    }

    public int GetEnemyId()
    {
        return enemyId;
    }

    public virtual void StartPatrolling()
    {
        Debug.Log("Enemy " + enemyId + " patrolling: " + transform.position.x + "," + transform.position.y + " to " + currentPatrolPoint.x + "," + currentPatrolPoint.y);
        patrolling = true;
    }

    public void SetPosition(int directionX, float positionX, float positionY)
    {
        this.directionX = directionX;
        transform.position = new Vector3(positionX, positionY, transform.position.z);

        if (directionX == transform.localScale.x)
        {
            transform.localScale = new Vector3(-directionX, transform.localScale.y, transform.localScale.z);
        }
    }

    public void SetPatrollingPoint(int directionX, float positionX, float positionY, float patrollingPointX, float patrollingPointY)
    {
        SetPosition(directionX, positionX, positionY);
        currentPatrolPoint = new Vector2(patrollingPointX, patrollingPointY);
    }

    #endregion

    #region Messaging

    protected virtual void SendEnemyDiedToServer(float damage)
    {
        string message = "EnemyDied/" + enemyId + "/" + damage;
        SendMessageToServer(message, false);
    }

    protected virtual void SendPositionToServer()
    {
        string message = "EnemyChangePosition/" +
            enemyId + "/" +
            directionX + "/" +
            transform.position.x + "/" +
            transform.position.y;

        SendMessageToServer(message, false);
    }

    protected void SendPatrollingPoint() //TODO necesitamos esto? 
    {
        string message = "EnemyPatrollingPoint/" +
            enemyId + "/" +
            directionX + "/" +
            transform.position.x + "/" +
            transform.position.y + "/" +
            currentPatrolPoint.x + "/" +
            currentPatrolPoint.y;

        SendMessageToServer(message, true);
    }

    private void SendIgnoreCollisionDataToServer(GameObject player, bool collision)
    {
        SendMessageToServer("IgnoreCollisionBetweenObjects/" + collision + "/" + player.name + "/" + gameObject.name, true);
    }

    protected virtual void SendMessageToServer(string message, bool secure)
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, secure);
        }
    }

    #endregion

    #region Utils

    protected bool LocalPlayerHasControl()
    {
        return levelManager.localPlayer && levelManager.localPlayer.controlOverEnemies;
    }

    protected bool GameObjectIsPlayer(GameObject other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        return playerController && playerController.localPlayer;
    }

    protected void InitializeParticles()
    {
        ParticleSystem[] _particles = gameObject.GetComponentsInChildren<ParticleSystem>();

        if (_particles.Length <= 0)
        {
            return;
        }

        particles = new GameObject[_particles.Length];

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i] = _particles[i].gameObject;
        }

        ToggleParticles(false);
    }

    protected void ToggleParticles(bool activate)
    {
        if (particles != null && particles.Length > 0)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].SetActive(activate);
            }
        }
    }

    protected void ToggleParticles(bool activate, int particleId)
    {
        if (particles != null && particles.Length > 0)
        {
            particles[particleId].SetActive(activate);
        }
    }
    protected void TurnAroundIfNeccessary()
    {
        bool turnAround = false;

        if (currentPatrolPoint.x < transform.position.x)
        {
            if (directionX == 1)
            {
                turnAround = true;
            }

        }

        else if (currentPatrolPoint.x > transform.position.x)
        {
            if (directionX == -1)
            {
                turnAround = true;
            }

        }

        if (turnAround)
        {
            directionX *= -1;
            transform.localScale = new Vector3(-directionX, transform.localScale.y, transform.localScale.z);
        }

    }

    protected void NextPatrollingPoint()
    {

        if (patrollingPoints == null || patrollingPoints.Length == 0)
        {
            Debug.Log(name + " : " + enemyId + " has no patrolling points.");
            return;
        }

        currentPatrolPoint = patrollingPoints[currentPatrolPointCount];
        currentPatrolPointCount = (currentPatrolPointCount + 1) % patrollingPoints.Length;

        TurnAroundIfNeccessary();
    }

    #endregion

    #region Events

    /* protected void OnTriggerStay2D(Collider2D other)
     {
         if (GameObjectIsPlayer(other.gameObject))
         {
             Attack(other.gameObject);
         }
     }*/

    // Attack those who enter the alert zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            Attack(other.gameObject);
        }
    }
    
    // Attack those who collide with me
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            Attack(other.gameObject);
        }
    }

    public IEnumerator WaitDying()
    {
        yield return new WaitForSeconds(WaitToDie);
        Destroy(gameObject);
    }

    public IEnumerator WaitToAttack()
    {
        yield return new WaitForSeconds(timeForAttack);
    }

    public IEnumerator WaitTillNoMaged()
    {
        yield return new WaitForSeconds(poweredTime);

        UnmageThisEnemy();
    }

    #endregion

}
