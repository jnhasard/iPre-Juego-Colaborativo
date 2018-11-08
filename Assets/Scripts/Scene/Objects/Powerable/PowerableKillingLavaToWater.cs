using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerableKillingLavaToWater : PowerableObject
{

    public int id;

    protected override void Start()
    {
        base.Start();
        if (id.Equals(default(int)))
        {
            Debug.LogError("The : " + gameObject.name + " Powerable for lava into water has no Id set");
        }
    }

    protected override void DoYourPowerableThing()
    {
        GameObject killingLavaObject = GameObject.Find("LavaFloor" + id);
        if (killingLavaObject != null)
        {
            TurnTriggersIntoColliders(killingLavaObject);

            if (killingLavaObject.GetComponent<KillingObject>())
            {
                DeactivateKillingObject(killingLavaObject);
            }
        }
        ChangeLavaSpritesIntoWater(true);
    }

    protected override void UndoYourPowerableThing()
    {
        GameObject killingLavaObject = GameObject.Find("LavaFloor" + id);
        if (killingLavaObject != null)
        {
            TurnTriggersIntoColliders(killingLavaObject);

            if (killingLavaObject.GetComponent<KillingObject>())
            {
                DeactivateKillingObject(killingLavaObject);
            }
        }
        ChangeLavaSpritesIntoWater(false);
    }

    protected void TurnTriggersIntoColliders(GameObject killingObject)
    {
        Collider2D lavaCollider = killingObject.GetComponent<Collider2D>();
        if (lavaCollider != null)
        {
            if (lavaCollider.isTrigger)
            {
                lavaCollider.isTrigger = false;
            }
        }
    }

    protected void ChangeLavaSpritesIntoWater(bool isWater)
    {
        GameObject[] changableLavas = GameObject.FindGameObjectsWithTag("ChangableLava" + id);
        foreach (GameObject changableLava in changableLavas)
        {
            SceneAnimator sAnimator = FindObjectOfType<SceneAnimator>();
            sAnimator.SetBool("isWater", isWater, changableLava);
        }
    }

    protected void DeactivateKillingObject(GameObject killingObject)
    {
        KillingObject killingLava = killingObject.GetComponent<KillingObject>();
        killingLava.activated = false;
    }
}
