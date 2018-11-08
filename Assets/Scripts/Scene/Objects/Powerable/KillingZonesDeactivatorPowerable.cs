using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillingZonesDeactivatorPowerable : PowerableObject {

    public KillingObject[] killingObjects;
    public EnemyController[] enemies; 

    protected override void DoYourPowerableThing()
    { 
        foreach (KillingObject kObject in killingObjects)
        {
            Collider2D[] colliders = kObject.GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = false; 
            }
            kObject.SetActive(false);
        }

        foreach (EnemyController eController in enemies)
        {
            Collider2D[] colliders = eController.GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = false;
            }
        }
    }

    protected override void UndoYourPowerableThing()
    {
        foreach (KillingObject kObject in killingObjects)
        {
            kObject.SetActive(true);
            Collider2D[] colliders = kObject.GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = true;
            }
        }

        foreach (EnemyController eController in enemies)
        {
            Collider2D[] colliders = eController.GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = true;
            }
        }
    }
}
