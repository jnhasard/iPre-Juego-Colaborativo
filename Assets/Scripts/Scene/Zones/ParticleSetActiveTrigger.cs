using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSetActiveTrigger : MonoBehaviour
{

    public GameObject[] particlesOfDiferentKind;
    private int numberOfPlayers;

    private void Start()
    {
        numberOfPlayers = 0;
        ToggleParticles(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameObjectIsPlayer(collision.gameObject))
        {
            EnterTrigger(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (GameObjectIsPlayer(collision.gameObject))
        {
            ExitTrigger(collision.gameObject);
        }
    }

    public void ExitTrigger(GameObject player)
    {
        numberOfPlayers--;
        if (numberOfPlayers <= 0)
        {
            ToggleParticles(false);
        }
        ToggleMyInfoToPlayer(player, false);
    }

    public void EnterTrigger(GameObject player)
    {
        numberOfPlayers++;
        if (numberOfPlayers == 1)
        {
            ToggleParticles(true);
        }
        ToggleMyInfoToPlayer(player, true);
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

    protected void ToggleMyInfoToPlayer(GameObject player, bool toggle)
    {
        if (toggle == true)
        {
            PlayerController pController = player.GetComponent<PlayerController>();
            pController.availableParticleTrigger = gameObject;
        }
        else
        {
            PlayerController pController = player.GetComponent<PlayerController>();
            pController.availableParticleTrigger = null;
        }

    }
    protected void ToggleParticles(bool toggle)
    {
        foreach (GameObject particle in particlesOfDiferentKind)
        {
            particle.SetActive(toggle);
        }
    }
}
