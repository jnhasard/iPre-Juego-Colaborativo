using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerPoweredParticles : PoweredParticles
{
    #region Events

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>())
        {
            CheckIfPlayerEntered(other.gameObject);
            PlayerController player = other.GetComponent<PlayerController>();
            player.SetPositiveGravity(false);
        }

        if (other.GetComponent<MovableObject>())
        {
            MovableObject movable = other.GetComponent<MovableObject>();
            if (movable.GetMovableAlreadyIn())
            {
                return;
            }
            else
            {
                other.GetComponent<Rigidbody2D>().gravityScale *= -1;
                GameObject[] particles = movable.particles;
                movable.ToggleParticles(particles, false);
                movable.SetIfImInOrNot(true);
            }
        }

    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>())
        {
            CheckIfPlayerAlreadyLeft(other.gameObject);
            PlayerController player = other.GetComponent<PlayerController>();
            player.SetPositiveGravity(true);
        }

        if (other.GetComponent<MovableObject>())
        {
            MovableObject movable = other.GetComponent<MovableObject>();
            if (movable.GetMovableAlreadyIn() == false)
            {
                return;
            }
            else
            {
                other.GetComponent<Rigidbody2D>().gravityScale *= -1;
                GameObject[] particles = movable.particles;
                movable.ToggleParticles(particles, false);
                movable.SetIfImInOrNot(false);
            }
        }
    }



    #endregion

}
