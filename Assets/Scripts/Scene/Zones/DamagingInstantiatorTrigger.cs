using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingInstantiatorTrigger : MonoBehaviour
{

    public GameObject[] damagingInstantiators;
    private int numberOfPlayers;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameObjectIsPlayer(collision.gameObject))
        {
            EnterTrigger();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (GameObjectIsPlayer(collision.gameObject))
        {
            ExitTrigger();
        }
    }
    
    public void ExitTrigger()
    {
        numberOfPlayers--;
        if (numberOfPlayers <= 0)
        {
            foreach (GameObject damaging in damagingInstantiators)
            {
                DamagingInstantiator dInstantiator = damaging.GetComponent<DamagingInstantiator>();
                dInstantiator.needed = false;
            }
        }
    }

    public void EnterTrigger()
    {
        numberOfPlayers++;
        if (numberOfPlayers == 1)
        {
            foreach (GameObject damaging in damagingInstantiators)
            {
                DamagingInstantiator dInstantiator = damaging.GetComponent<DamagingInstantiator>();
                dInstantiator.needed = true;
                StartCoroutine(dInstantiator.InstantiateDamaging());
            }
        }
    }

    protected bool GameObjectIsPlayer(GameObject other)
    {
        if (other.GetComponent<PlayerController>())
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            return playerController;
        }
        else
        {
            return false;
        }
    }
}
