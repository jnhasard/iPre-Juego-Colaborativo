using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDeactivator : MonoBehaviour {

    public GameObject[] gObjects;
    public int numberOfPlayersIn;
    private PlayerController[] pControllers;
    public bool setCompleteObject;
	// Use this for initialization

	void Start ()
    {
        if (setCompleteObject)
        {
            SetCompleteObjectActive(false);
        }
        else
        {
            SetCollidersActive(false);
        }
        numberOfPlayersIn = 0;
        pControllers = new PlayerController[3];
    }
	
    public void SetCollidersActive (bool active)
    {
        foreach (GameObject gObject in gObjects)
        {
            if (gObject.GetComponent<Collider2D>())
            {
                Collider2D[] colliders = gObject.GetComponents<Collider2D>();
                foreach (Collider2D collider in colliders)
                {
                    collider.enabled = active;
                }
            }
        }
    }

    public void SetCompleteObjectActive(bool active)
    {
        foreach (GameObject gObject in gObjects)
        {
            gObject.SetActive(active);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            OnEnterPlayer(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            OnExitPlayer(collision.gameObject);
        }
    }

   public void OnExitPlayer(GameObject player)
    {
        PlayerController pController = player.GetComponent<PlayerController>();
        int playerId = pController.playerId;

        if (pControllers[playerId] == null)
        {
            return;
        }

        else
        {
            pControllers[playerId] = null;
            numberOfPlayersIn--;
        }

        if (numberOfPlayersIn < 0) // Just to make Sure
        {
            numberOfPlayersIn = 0;
        }

        if (numberOfPlayersIn == 0)
        {
            if (setCompleteObject)
            {
                Debug.Log("El jugador " + player.name + " me voy a pitear los collider porque somos " + numberOfPlayersIn);
                SetCompleteObjectActive(false);
            }
            else
            {
                Debug.Log("El jugador " + player.name + " me voy a pitear los collider porque somos " + numberOfPlayersIn);
                SetCollidersActive(false);
            }
        }
    }

    public void OnEnterPlayer(GameObject player)
    {
        PlayerController pController = player.GetComponent<PlayerController>();
        int playerId = pController.playerId;
        if (pControllers[playerId] != null)
        {
            return;
        }
        else
        {
            pControllers[playerId] = pController;
            numberOfPlayersIn++;
            if (numberOfPlayersIn > 3)
            {
                numberOfPlayersIn = 3;
            }
        }
        if (numberOfPlayersIn >= 1)
        {
            if (setCompleteObject)
            {
                SetCompleteObjectActive(true);
            }
            else
            {
                SetCollidersActive(true);
            }
        }
    }
}
