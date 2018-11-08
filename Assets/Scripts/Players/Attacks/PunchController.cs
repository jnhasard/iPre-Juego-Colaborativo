using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchController : AttackController
{
    #region Attributes

    private static Vector2 attackForce = new Vector2(2500f, 100f);

    #endregion

    #region Start & Update

    protected override void Start()
    {

        base.Start();
        /*if (IsCasterLocal())
        {
            if (enhanced == false)
            {
                SoundManager sManager = FindObjectOfType<SoundManager>();
                sManager.PlaySound(gameObject, GameSounds.WarriorAttack, false);
            }
            else if (enhanced)
            {
                SoundManager sManager = FindObjectOfType<SoundManager>();
                sManager.PlaySound(gameObject, GameSounds.WarriorAttackEnhanced, false);
            }
        }*/

        maxDistance = 5f;
    }

    protected override void Update()
    {
        base.Update();

    }

    #endregion

    #region Common

    protected void DestroyObject(GameObject other)
    {
        DestroyableObject destroyable = other.GetComponent<DestroyableObject>();

        if (destroyable.reinforced && !enhanced)
        {
            return;
        }

        destroyable.DestroyMe(true);
    }

    protected void MoveObject(GameObject other)
    {

        MovableObject movable = other.GetComponent<MovableObject>();
        Vector2 force = attackForce;

        if (enhanced)
        {
            force *= 700;
        }

        if (other.transform.position.x < transform.position.x)
        {
            force.x *= -1;
        }

        movable.MoveMe(force, true);
    }

    #endregion

    #region Events

    private new void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Im an Attack and i Collided with : " + collision.gameObject.name);

        if (CollidedWithEnemy(collision.gameObject))
        {
            DealDamage(collision.gameObject);
        }

        if (ColidedWithBurnable(collision.gameObject))
        {
            BurnObject(collision.gameObject);
        }

        if (CollidedWithDestroyable(collision.gameObject))
        {
            DestroyObject(collision.gameObject);
        }

        if (caster.localPlayer)
        {
             if (CollidedWithMovable(collision.gameObject))
            {
                MoveObject(collision.gameObject);
            }
        }

        Destroy(gameObject, destroyDelayTime);
    }

    #endregion

    #region Utils

    protected bool CollidedWithDestroyable(GameObject other)
    {
        return other.GetComponent<DestroyableObject>();
    }

    protected bool CollidedWithMovable(GameObject other)
    {
        return other.GetComponent<MovableObject>();
    }

    protected override int GetDamage()
    {
        if (enhanced)
        {
            return damage + 6;
        }
        else
        {
            return damage;
        }
    }

    protected override float GetSpeed()
    {
        if (enhanced)
        {
            return speed * 1.5f;
        }
        else
        {
            return speed;
        }
    }

    protected bool ColidedWithBurnable(GameObject otherObject)
    {
        if (enhanced)
        {
            if (otherObject.GetComponent<BurnableObject>())
            {
                return true;
            }
        }

        return false;
    }

    protected void BurnObject (GameObject burnableObject)
    {
        BurnableObject bObject = burnableObject.GetComponent<BurnableObject>();
        bObject.Burn();
    }
    protected override float GetDistance()
    {
        if (enhanced)
        {
            return maxDistance * 1.5f;
        }
        else
        {
            return maxDistance;
        }
    }
}
#endregion
