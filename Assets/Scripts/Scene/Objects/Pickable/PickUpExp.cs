using UnityEngine;

public class PickUpExp : MonoBehaviour
{
    public int expGranted;


    #region Events

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            PickUp();
        }
    }

    #endregion

    #region Common

    public void PickUp()
    {
        string expAmount = expGranted.ToString();
        SendDestroyDataToServer();
        SendMessageToServer("GainExp/" + expAmount, true);
        Destroy(gameObject);
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }

    #endregion

    #region Utils

    protected bool GameObjectIsPlayer(GameObject other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        return playerController && playerController.localPlayer;
    }

    #endregion

    #region Messaging

    private void SendMessageToServer(string message, bool secure)
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, secure);
        }
    }

    protected void SendDestroyDataToServer()
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer("ObjectDestroyed/" + name, true);
        }
    }

    #endregion

}