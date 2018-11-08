using UnityEngine;

public class PoweredParticles : MonoBehaviour
{
    protected PlayerController[] pControllers;

    protected void Start()
    {
        pControllers = new PlayerController[3];
    }


    protected virtual void OnTriggerEnter2D(Collider2D other)
    {

    }
    protected virtual void OnTriggerExit2D(Collider2D other)
    {

    }

    protected void CheckIfPlayerEntered(GameObject playerObject)
    {
        PlayerController player = playerObject.GetComponent<PlayerController>();
        int i = player.playerId;
        if (pControllers[i] == null)
        {
            pControllers[i] = player;
        }
        else
        {
            return;
        }
    }


    protected void CheckIfPlayerAlreadyLeft(GameObject playerObject)
    {
        PlayerController player = playerObject.GetComponent<PlayerController>();
        int i = player.playerId;
        if (pControllers[i] != null)
        {
            pControllers[i] = null;
        }
        else
        {
            return;
        }
    }
}


