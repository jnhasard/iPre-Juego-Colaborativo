using UnityEngine;

public class PickUpItem : MonoBehaviour
{

    #region Attributes

    public string info;
    public int id;

    public PlannerItem itemObj = null;

    #endregion

    #region Common

    public void PickUp()
    {
        Inventory.instance.AddItem(this);

        if (itemObj != null)
        {
            if (FindObjectOfType<Planner>())
            {
                LevelManager levelManager = GameObject.FindObjectOfType<LevelManager>();
                Planner planner = FindObjectOfType<Planner>();

                itemObj.PickUp(levelManager.localPlayer.playerObj);
                planner.Monitor();
            }
        }

        SendMessageToServer("ObjectDestroyed/" + name, true);
        Destroy(this.gameObject);
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }

    #endregion

    #region Events

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {   
            PickUp();
        }
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

    #endregion

}