using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour
{

    #region Attributes

    public enum MoveType
    {
        Target,
        Targets,
        Direction
    }
    protected MoveType moveType;
    protected Vector2 target;
    protected Vector2[] targets;
    protected SceneAnimator sceneAnim;

    protected float currentDistance;
    protected float maxDistance;
    protected bool initialized;
    protected float direction;
    protected bool isMoving;
    protected bool enhanced;
    protected float speed;
    protected bool dontAcceptMorePlayers;
    public int targetsReached;
    protected bool yetNeeded;
    private float timetoKillBubble;
    private LevelManager levelManager;

    private PlayerController[] playerControllers;
    private BubbleParticleController parasiteParticle;

    #endregion

    // Use this for initialization
    protected virtual void Start()
    {
        yetNeeded = true;
        dontAcceptMorePlayers = false;
        currentDistance = 0;
        maxDistance = 6f;
        targetsReached = 0;
        sceneAnim = FindObjectOfType<SceneAnimator>();
        levelManager = FindObjectOfType<LevelManager>();
        playerControllers = new PlayerController[3];
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        if (isMoving)
        {
            Move();
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (dontAcceptMorePlayers)
        {
            return;
        }
        if (GameObjectIsPlayer(other.gameObject))
        {
            CheckIfPlayerAlreadyEntered(other.gameObject);
        }
    }
    protected virtual void OnCollisionExit2D(Collision2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            CheckIfPlayerAlreadyLeft(other.gameObject);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<DamagingObject>())
        {
            if (!levelManager.GetMage().ProtectedByShield(gameObject))
            {
                for (int i = 0; i < playerControllers.Length; i++)
                {
                    if (playerControllers[i] != null)
                    {
                        PlayerController playerToRelease = playerControllers[i];
                        playerControllers[i] = null;
                        playerToRelease.ResetTransform();
                        playerToRelease.TakeDamage(10, new Vector2(150f, 15f));
                        playerToRelease.parent = null;
                    }
                }
                Destroy(parasiteParticle.gameObject);
                Destroy(gameObject);
            }
        }

        if (other.GetComponent<DestroyableObject>())
        {
            if (levelManager.GetWarrior().IsWarriored(gameObject))
            {
                DestroyableObject destroyable = other.GetComponent<DestroyableObject>();
                destroyable.DestroyMe(true);
            }
            else
            {
                for (int i = 0; i < playerControllers.Length; i++)
                {
                    if (playerControllers[i] != null)
                    {
                        PlayerController playerToRelease = playerControllers[i];
                        playerControllers[i] = null;
                        playerToRelease.ResetTransform();
                        playerToRelease.TakeDamage(10, new Vector2(150f, 15f));
                        playerToRelease.parent = null;
                    }
                }
                Destroy(parasiteParticle.gameObject);
                Destroy(gameObject);
            }
        }

        if (other.GetComponent<BurnableObject>())
        {
            if (levelManager.GetWarrior().IsWarriored(gameObject))
            {
                BurnableObject burnable = other.GetComponent<BurnableObject>();
                burnable.Burn();
            }
            else
            {
                for (int i = 0; i < playerControllers.Length; i++)
                {
                    if (playerControllers[i] != null)
                    {
                        PlayerController playerToRelease = playerControllers[i];
                        playerControllers[i] = null;
                        playerToRelease.ResetTransform();
                        playerToRelease.TakeDamage(10, new Vector2(150f, 15f));
                        playerToRelease.parent = null;
                    }
                }
                Destroy(parasiteParticle.gameObject);
                Destroy(gameObject);
            }
        }

        if (other.GetComponent<KillingObject>() && other.GetComponent<KillingObject>().activated)
        {
            if (!other.GetComponent<DarkElectricity>())
            {
                if (levelManager.GetMage().ProtectedByShield(gameObject))
                {
                    for (int i = 0; i < playerControllers.Length; i++)
                    {
                        if (playerControllers[i] != null)
                        {
                            PlayerController playerSaved = playerControllers[i];
                            Physics2D.IgnoreCollision(playerSaved.GetComponent<BoxCollider2D>(), other.GetComponent<Collider2D>(), true);
                        }
                    }
                }

                else if (!levelManager.GetMage().ProtectedByShield(gameObject))
                {
                    for (int i = 0; i < playerControllers.Length; i++)
                    {
                        if (playerControllers[i] != null)
                        {
                            PlayerController playerToRelease = playerControllers[i];
                            playerControllers[i] = null;
                            playerToRelease.ResetTransform();
                            playerToRelease.parent = null;
                        }
                    }
                    Destroy(parasiteParticle.gameObject);
                    Destroy(gameObject);
                }
            }
            else if (other.GetComponent<DarkElectricity>())
            {
                if (levelManager.GetEngineer().IsElectrified(gameObject))
                {
                    for (int i = 0; i < playerControllers.Length; i++)
                    {
                        if (playerControllers[i] != null)
                        {
                            PlayerController playerSaved = playerControllers[i];
                            Physics2D.IgnoreCollision(playerSaved.GetComponent<BoxCollider2D>(), other.GetComponent<Collider2D>(), true);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < playerControllers.Length; i++)
                    {
                        if (playerControllers[i] != null)
                        {
                            PlayerController playerToRelease = playerControllers[i];
                            playerControllers[i] = null;
                            playerToRelease.parent = null;
                            playerToRelease.ResetTransform();

                        }
                    }
                    Destroy(parasiteParticle.gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }

    private void CheckIfPlayerAlreadyEntered(GameObject player)
    {
        PlayerController myPlayer = player.GetComponent<PlayerController>();
        int i = myPlayer.playerId;
        if (playerControllers[i] != null)
        {
            return;
        }

        else
        {
            playerControllers[i] = myPlayer;
            playerControllers[i].parent = gameObject;
            myPlayer.transform.parent = transform;
        }
    }

    private void CheckIfPlayerAlreadyLeft(GameObject player)
    {
        PlayerController myPlayer = player.GetComponent<PlayerController>();
        int i = myPlayer.playerId;
        if (playerControllers[i] == null)
        {
            return;
        }
        else
        {
            myPlayer.ResetTransform();
            myPlayer.parent = null;
        }
    }


    public void InitializeColouredBubbles(MoveType _moveType, PlayerController caster, GameObject bubbleParticle)
    {
        SetPowerableBubbleCaster(caster);
        SetPowerParticle(bubbleParticle);
        moveType = _moveType;
        initialized = true;
    }

    public void InitializeNeutralBubble(MoveType _moveType)
    {
        moveType = _moveType;
        initialized = true;
    }

    private void SetPowerParticle(GameObject bubbleParticle)
    {
        parasiteParticle = bubbleParticle.GetComponent<BubbleParticleController>();

        PowerableObject.Power[] bubblePowers = gameObject.GetComponent<PowerableObject>().powers;
        if (bubblePowers.Length >= 1)
        {
            bubblePowers[0].particles[0] = bubbleParticle;
        }
    }
    public void SetMovement(Vector2 startPosition, Vector2[] _targets, float _speed, float _timeToWait, float _timeToKillBubble)
    {
        if (!initialized || moveType.Equals(MoveType.Direction) || moveType.Equals(MoveType.Target))
        {
            Debug.LogError("Attack was not initialized correctly");
            return;
        }

        timetoKillBubble = _timeToKillBubble;
        targets = _targets;
        speed = _speed;
        transform.position = startPosition;

        StartCoroutine(WaitToMove(_timeToWait));

    }
    protected IEnumerator WaitToMove(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        isMoving = true;
    }

    protected bool GameObjectIsPlayer(GameObject other)
    {
        return other.GetComponent<PlayerController>();
    }

    protected void Move()
    {

        switch (moveType)
        {
            case MoveType.Direction:
                MoveInDirection();
                break;
            case MoveType.Targets:
                MoveToTarget();
                break;
        }

    }

    protected void MoveInDirection()
    {
        float distance = GetSpeedInDirection();

        transform.position += Vector3.right * distance;

        currentDistance += System.Math.Abs(distance);

        if (maxDistance <= currentDistance)
        {
            Destroy(gameObject);
        }
    }

    protected void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targets[targetsReached], GetSpeedToTarget());
        if (parasiteParticle != null)
        {
            parasiteParticle.transform.position = gameObject.transform.position;
        }

        if (transform.position.x == targets[targetsReached].x && transform.position.y == targets[targetsReached].y)
        {
            if (yetNeeded)
            {
                targetsReached++;

                if (targetsReached == targets.Length)
                {
                    for (int i = 0; i < playerControllers.Length; i++)
                    {
                        if (playerControllers[i] != null)
                        {
                            PlayerController playerToRelease = playerControllers[i];
                            playerToRelease.ResetTransform();
                            playerToRelease.parent = null;
                        }
                    }

                    if (parasiteParticle != null)
                    {
                        Destroy(parasiteParticle.gameObject, timetoKillBubble);
                    }

                    dontAcceptMorePlayers = true;
                    Destroy(gameObject, timetoKillBubble);
                    enabled = false;
                }
            }
        }
    }


    protected float GetSpeedToTarget()
    {
        return GetSpeed() * Time.deltaTime * 1.3f;
    }

    protected float GetSpeedInDirection()
    {
        return GetSpeed() * direction * Time.deltaTime;
    }

    private void SetPowerableBubbleCaster(PlayerController caster)
    {
        PowerableObject.Power[] bubblePowers = gameObject.GetComponent<PowerableObject>().powers;
        if (bubblePowers.Length >= 1)
        {
            bubblePowers[0].caster = caster;
        }
    }

    private void SetNeutralBubblePowerable(GameObject[] casters)
    {
        PowerableObject.Power[] bubblePowers = gameObject.GetComponent<PowerableObject>().powers;
        for (int i = 0; i < bubblePowers.Length; i++)
        {
            bubblePowers[i].caster = casters[i].GetComponent<PlayerController>();
        }
    }
    protected virtual float GetSpeed()
    {
        return speed; //Every Player Checks His Speed n' Shit
    }

}