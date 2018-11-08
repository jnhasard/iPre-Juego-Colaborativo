using UnityEngine;

/**
 * When a player touches this objects it loses health and respwans.
 */
public class KillingObject : MonoBehaviour
{
    #region Attributes

    protected LevelManager levelManager;

    protected ParticleSystem particles;
    public bool activated;
    public int damage;
    public int hitsBeforeKilled;
    protected int hits;

    #endregion

    #region Start & Update

    protected virtual void Start()
    {
        particles = GetComponent<ParticleSystem>();
        levelManager = FindObjectOfType<LevelManager>();

        if (particles)
        {
            SetActive(activated);
        }
    }

    #endregion

    #region Common

    public virtual void SetActive(bool active)
    {

        activated = active;

        if (!particles)
        {
            Debug.Log("This killing object does not have particles");
            return;
        }

        if (active)
        {
            particles.Play();
        }
        else
        {
            particles.Stop();
        }
    }

    protected virtual void Kill(GameObject _player)
    {   
        if (activated)
        {
            PlayerController player = _player.GetComponent<PlayerController>();

            if (PlayerIsLocalPlayer(player))
            {
                player.TakeDamage(damage, new Vector2(0, 0));
                levelManager.Respawn();
            }
            else
            {
                levelManager.Respawn(player);
            }
        }
    }

    public virtual void HitByPoweredDamaging()
    {
        hits++;
        if (hits == hitsBeforeKilled)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Events

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            Kill(other.gameObject);
        }
    }

    // Attack those who enter the alert zone
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            Kill(other.gameObject);
        }
    }

    #endregion

    #region Utils

    protected bool GameObjectIsPlayer(GameObject other)
    {
        return other.GetComponent<PlayerController>();
    }

    protected bool PlayerIsLocalPlayer(PlayerController player)
    {
        return player.localPlayer;
    }

    #endregion

}


