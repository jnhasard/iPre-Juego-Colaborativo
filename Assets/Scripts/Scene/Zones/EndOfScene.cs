using UnityEngine;
using System.Collections;

public class EndOfScene : MonoBehaviour
{
    private PlayerController[] playerControllers;
    #region Attributes

    LevelManager levelManager;

    private int playersWhoArrived;
    public int playersToArrive;
    public bool needsExp;
    public int expRequired;
    public string whatToDo;
    public NPCtrigger feedback;

    #endregion

    #region Start

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        playersWhoArrived = 0;

        playerControllers = new PlayerController[3];

        if (playersToArrive == 0)
        {
            Debug.Log("Theres an End of Scene without an amount of players needed");
        }
    }

    #endregion

    #region Utils

    protected bool GameObjectIsPlayer(GameObject other)
    {
        return other.GetComponent<PlayerController>();
    }

    #endregion

    #region Events

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            if (CheckIfPlayerHasntEntered(other.gameObject))
            {
                playersWhoArrived++;
                Debug.Log("Players who arrived: " + playersWhoArrived);

                if (playersWhoArrived == playersToArrive)
                {
                    if (needsExp)
                    {
                        CheckIfExpIsEnough();
                    }
                    else
                    {
                        if (levelManager.GetLocalPlayerController().controlOverEnemies)
                        {
                            levelManager.GoToNextScene();
                        }
                    }
                }

                else 
                {
                    if (other.gameObject.GetComponent<PlayerController>().localPlayer)
                    {
                        levelManager.ActivateNPCFeedback("Asegúrate de que lleguen los demás");
                    }
                }
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            if (CheckIfPlayerAlreadyLeft(other.gameObject))
            {
               --playersWhoArrived;
            }
        }
    }

    #endregion

    protected bool CheckIfPlayerAlreadyLeft(GameObject playerObject)
    {
        PlayerController player = playerObject.GetComponent<PlayerController>();
        int i = player.playerId;

        if (playerControllers[i] != null)
        {
            playerControllers[i].availableEndOfScene = null;
            playerControllers[i] = null;
            return true;
        }
        else if (playerControllers[i] == null)
        {
            return false;
        }

        return false;
    }

    protected void CheckIfExpIsEnough()
    {
        SendMessageToServer("IsThisExpEnough", true);
    }

    public void HandleExpQuestion(string[] msg)
    {
        int incomingExp = int.Parse(msg[1]);
        if (incomingExp >= expRequired)
        {
            switch (whatToDo)
            {
                case "InstanciarEngranaje":
                    HandleEngInstantation();
                    break;
                default:
                    return;
            }
            
        }
    }

    protected bool CheckIfPlayerHasntEntered(GameObject playerObject)
    {
        PlayerController player = playerObject.GetComponent<PlayerController>();
        int i = player.playerId;

        if (playerControllers[i] != null)
        {
            return false;
        }

        else if (playerControllers[i] == null)
        {
            playerControllers[i] = player;
            player.availableEndOfScene = gameObject;
            Debug.Log(playerObject.name + " reached the end of the scene With an ID of: " + i);
            return true;
        }

        return false;
    }


    public void ErasePlayerInEndOfScene(GameObject player)
    {
        PlayerController pController = player.GetComponent<PlayerController>();
        int playerID = pController.playerId;

        if (playerControllers[playerID] != null)
        {
            playerControllers[playerID] = null;
            playersWhoArrived--;
        }
    }

    protected void HandleEngInstantation()
    {
        feedback.ReadNextFeedback();
        levelManager.InstantiatePrefab("Items/EngranajeA", new Vector2(-4f, -4f));
    }

    protected void SendMessageToServer(string message, bool secure)
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, secure);
        }
    }

}
