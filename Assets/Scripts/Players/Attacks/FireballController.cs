using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : AttackController
{

    protected override void Start()
    {
        base.Start();
        maxDistance = 6f;
        /*if (IsCasterLocal())
        {
            if (enhanced == false)
            {
                SoundManager sManager = FindObjectOfType<SoundManager>();
                sManager.PlaySound(gameObject, GameSounds.MageAttack, false);
            }
            else if (enhanced)
            {
                SoundManager sManager = FindObjectOfType<SoundManager>();
                sManager.PlaySound(gameObject, GameSounds.MageAttackEnhanced, false);
            }
        }*/
    }

    protected override int GetDamage()
    {
        if (enhanced)
        {
            return damage + 2;
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
            return speed * 1.2f;
        }
        else
        {
            return speed;
        }
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

