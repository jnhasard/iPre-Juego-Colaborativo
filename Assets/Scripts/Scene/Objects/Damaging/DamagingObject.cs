using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/** 
 *  This class is for static damaging objects such as a lava
 *  in order to work this objects must have a trigger collider
 */
public class DamagingObject : MonoBehaviour
{
    #region Attributes

    protected Dictionary<string, bool> ignoresCollisions;

    public Vector2 force;
    public int damage;
    public int id;
    protected bool alreadyEntered;
    protected bool alreadyLeft;
    public bool turnedIntoWater;


    #endregion

    #region Start & Update

    protected virtual void Start()
    {
        ignoresCollisions = new Dictionary<string, bool> { { "Verde", false }, { "Rojo", false }, { "Amarillo", false } };
    }

    // Update is called once per frame
    void Update()
    {
    }

    #endregion

    #region Common

    protected virtual void DealDamage(GameObject player)
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        PlayerController playerController = player.GetComponent<PlayerController>();
        MageController mage = levelManager.GetMage();

        Vector2 playerPosition = player.transform.position;
        Vector2 attackForce = force;

        // Only hit local players
        if (!playerController.localPlayer)
        {
            return;
        }

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

        // If player is at the left side of the enemy push it to the left
        if (playerPosition.x < transform.position.x)
        {
            attackForce.x *= -1;
        }

        playerController.TakeDamage(damage, attackForce);
    }

    #endregion

    #region Messaging

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

    #region Events

    // Attack those who collide with me
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            DealDamage(other.gameObject);
        }

    }

    /*  protected void OnTriggerStay2D(Collider2D other)
      {
          if (GameObjectIsPlayer(other.gameObject))
          {
              DealDamage(other.gameObject);
          }
      }
          */
    // Attack those who enter the alert zone
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            DealDamage(other.gameObject);
        }

        if (GameObjectIsMageParticle(other.gameObject))
        {
            if (alreadyEntered == false)
            {
                alreadyEntered = true;

                if (id > 0)
                {
                    ChangeLavaIntoWater(true);
                }
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (GameObjectIsMageParticle(other.gameObject))
        {
            if (alreadyLeft == false)
            {
                alreadyLeft = true;

                if (id > 0)
                {
                    ChangeLavaIntoWater(false);
                }
            }
        }
    }

    #endregion

    #region Utils
    protected void DestroyMeAndParticles()
    {
        if (gameObject.GetComponent<OneTimeMovingObject>())
        {
            if (gameObject.GetComponent<OneTimeMovingObject>().needsParticles)
            {
                gameObject.GetComponent<OneTimeMovingObject>().DestroyParasiteParticles();
            }
        }

        Destroy(gameObject);
    }

    public void ChangeLavaIntoWater(bool boolValue)
    {
        LavaIntoWaterIdentifier[] changeableLavas = FindObjectsOfType<LavaIntoWaterIdentifier>();

        foreach (LavaIntoWaterIdentifier lava in changeableLavas)
        {
            if (id == lava.id)
            {
                SceneAnimator sAnimator = FindObjectOfType<SceneAnimator>();
                sAnimator.SetBool("WaterFalling", boolValue, lava.gameObject);
                StartCoroutine(WaitForLavaAnimator());
            }
        }

        if (boolValue)
        {
            turnedIntoWater = true;
        }
        else if (boolValue == false)
        {
            turnedIntoWater = false;
        }
    }

    protected void GetThisEnemyMaged(GameObject enemy)
    {
        EnemyController eC = enemy.gameObject.GetComponent<EnemyController>();
        eC.GetThisEnemyMaged();
    }

    protected bool GameObjectIsMageParticle(GameObject gobject)
    {
        return gobject.GetComponent<MagePoweredParticles>();
    }

    protected void KillEnemy(GameObject enemy)
    {
        EnemyController eC = enemy.gameObject.GetComponent<EnemyController>();
        eC.TakeDamage(150);
        Destroy(gameObject, .2f);
    }

    protected bool GameObjectIsBurnable(GameObject other)
    {
        return other.GetComponent<BurnableObject>();
    }

    protected IEnumerator WaitForLavaAnimator()
    {
        yield return new WaitForSeconds(.3f);
        alreadyEntered = false;
        alreadyLeft = false;
    }


    protected bool GameObjectIsDeactivableKillPlane(GameObject other)
    {
        if (other.gameObject.GetComponent<KillingObject>())
        {
            if (other.gameObject.tag == "DeactivableKillPlane")
            {
                return true;
            }

        }
        else
        {
            return false;
        }

        return false;
    }

    protected bool CheckIfImMaged()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager.GetMage())
        {
            return levelManager.GetMage().ProtectedByShield(gameObject);
        }
        else
        {
            Debug.LogError("It Seems there is no Mage");
        }
        return false;
    }


    protected bool CheckIfImWarriored(GameObject myself)
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager.GetWarrior() != null)
        {
            return levelManager.GetWarrior().IsWarriored(gameObject);
        }
        else
        {
            Debug.LogError("It Seems there is no warrior");
            return false;
        }
    }

    protected bool GameObjectIsDestroyable(GameObject other)
    {
        return other.GetComponent<DestroyableObject>();
    }

    protected bool GameObjectIsEnemy(GameObject other)
    {
        if (other.GetComponent<EnemyController>())
        {
            return true;
        }
        return false;
    }

    protected bool GameObjectIsPlayer(GameObject other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        return playerController && playerController.localPlayer;
    }

    public virtual void UpdateCollisionsWithPlayer(GameObject player, bool ignores)
    {
        foreach (Collider2D collider in GetComponents<Collider2D>())
        {
            if (!collider.isTrigger)
            {
                Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>(), ignores);
            }
        }

        ignoresCollisions[player.name] = ignores;
        SendIgnoreCollisionDataToServer(player, ignores);
    }

    #endregion

}
