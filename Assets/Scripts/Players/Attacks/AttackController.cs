using UnityEngine;

public class AttackController : MonoBehaviour
{

    #region Attributes

    public enum MoveType
    {
        Target,
        Direction
    }

    public PlayerController caster;

    protected static float destroyDelayTime = .04f;
    protected MoveType moveType;
    protected Vector2 target;

    protected float currentDistance;
    protected float maxDistance;
    protected bool initialized;
    protected float direction;
    protected bool isMoving;
    protected bool enhanced;
    protected float speed;
    protected int damage;

    #endregion

    #region Start & Update

    protected virtual void Start()
    {
        currentDistance = 0;
        damage = 5;
        maxDistance = 6f;

        IgnoreCollisionWithPlayers();
        IgnoreCollisionWithObjectsWhoHateMe();
    }


    protected virtual void Update()
    {
        if (isMoving)
        {
            Move();
        }
    }

    #endregion

    #region Common

    public void Initialize(PlayerController _caster, MoveType _moveType)
    {
        enhanced = _caster.isPowerOn;
        moveType = _moveType;
        caster = _caster;

        initialized = true;
    }

    public void SetMovement(Vector2 startPosition, int _direction, int _yAxis, float _speed)
    {
        if (!initialized || moveType.Equals(MoveType.Target))
        {
            Debug.LogError("Attack was not initialized correctly");
            return;
        }

        direction = _direction;
        speed = _speed;
        isMoving = true;

        transform.position = new Vector2(startPosition.x + (direction * 0.0001f), startPosition.y);

        if (direction == -1)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        if (_yAxis == -1)
        {
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        }
    }

    public void SetMovement(Vector2 startPosition, Vector2 _target, float _speed)
    {
        if (!initialized || moveType.Equals(MoveType.Direction))
        {
            Debug.LogError("Attack was not initialized correctly");
            return;
        }

        target = _target;
        speed = _speed;
        isMoving = true;

        transform.position = startPosition;

        if (_target.x < startPosition.x)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    protected void Move()
    {

        switch (moveType)
        {
            case MoveType.Direction:
                MoveInDirection();
                break;
            case MoveType.Target:
                MoveToTarget();
                break;
        }

    }

    // Hacer que reciba un enemigo
    protected void DealDamage(GameObject enemy)
    {
        float dealtDamage = GetDamage();

        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemyController.TakeDamage(dealtDamage);

    }

    // Este método debe ser sobreescrito para calcular el daño en cada caso
    protected virtual int GetDamage()
    {
        return damage;
    }

    public bool IsPowered()
    {
        return enhanced;
    }

    protected virtual float GetDistance()
    {
        return maxDistance;
    }

    #endregion

    #region Utils

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
        transform.position = Vector3.MoveTowards(transform.position, target, GetSpeedToTarget());

        if (transform.position.Equals((Vector3)target))
        {
            Destroy(gameObject);
        }
    }

    protected void IgnoreCollisionWithObjectsWhoHateMe()
    {
        IgnoreCollisionWithAttacks[] objectsWhoHateMe = FindObjectsOfType<IgnoreCollisionWithAttacks>();

        if (objectsWhoHateMe != null)
        {
            foreach (IgnoreCollisionWithAttacks objectWhoHatesMe in objectsWhoHateMe)
            {
                Collider2D[] collidersWhoHateME = objectWhoHatesMe.GetComponents<Collider2D>();
                foreach (Collider2D hatingCollider in collidersWhoHateME)
                {
                    Physics2D.IgnoreCollision(hatingCollider, GetComponent<Collider2D>(), true);
                }
            }
        }
    }

    protected void IgnoreCollisionWithPlayers()
    {
        LevelManager levelManager = GameObject.FindObjectOfType<LevelManager>();
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), levelManager.GetMage().GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), levelManager.GetWarrior().GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), levelManager.GetEngineer().GetComponent<Collider2D>());
    }

    protected bool IsCasterLocal()
    {
        return caster.localPlayer;
    }

    protected bool CollidedWithEnemy(GameObject other)
    {
        return other.GetComponent<EnemyController>();
    }

    protected float GetSpeedToTarget()
    {
        return GetSpeed() * Time.deltaTime * 1.3f;
    }

    protected float GetSpeedInDirection()
    {
        return GetSpeed() * direction * Time.deltaTime;
    }
    
    #endregion

    #region Events

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Im an Attack and i crushed with " + collision.gameObject.name);

        if (CollidedWithEnemy(collision.gameObject))
        {
            DealDamage(collision.gameObject);
        }

        Destroy(gameObject);
    }

    protected virtual float GetSpeed()
    {
        return speed; //Every Player Checks His Speed n' Shit
    }
    #endregion
}