using UnityEngine;

public class DecisionSystem : MonoBehaviour
{

    #region Attributes

    #region Enum

    public enum Choice
    {
        ChoosePathUp,
        ChoosePathMiddle,
        ChoosePathBottom,
        AceptarNPC,
        NegarNPC,
        Pensar,
    }

    #endregion


    public Choice[] choices;
    public int agreement;
    public int timer;
    public string[] choiceTexts;

    protected Choice?[] votes;
    protected int playersWhoArrived;
	protected LevelManager levelManager;

    #endregion

    #region Start

    void Start()
    {
        CheckDecisionAttributes();
		levelManager = FindObjectOfType<LevelManager> ();
        playersWhoArrived = 0;
        ClearVotes();
    }

    #endregion

    #region Common

    public void Vote(Choice choice)
    {
		int playerId = levelManager.GetLocalPlayerController().playerId;

        votes[playerId] = choice;
		SendVote (choice);
    }

    protected void RestartDecision()
    {
        votes = new Choice?[] { null, null, null };
        levelManager.RestartVoting();
    }
    protected void ClearVotes()
    {
        votes = new Choice?[] { null, null, null };
    }

    public void ResetDecision()
    {
        playersWhoArrived -= 1;
    }

    #endregion

    #region Utils

    protected void OnVoteFinished(Choice choice)
    {
        DecisionSystemActions dsa = new DecisionSystemActions();
        dsa.DoSomething(choice);
    }

    protected void EvaluateVote()
    {
        for (int i = 0; i < votes.Length; i++)
        {
            int agreementCount = 1;

            for (int j = 0; j < votes.Length; j++)
            {
                if (i != j)
                {
                    if (votes[i].Equals(votes[j]))
                    {
                        agreementCount += 1;
                        if (agreementCount == agreement)
                        {
                            OnVoteFinished((Choice)votes[i]);
                            return;
                        }
                        else
                        {
                            //TODO: Aquí Efectuar código para no agreement
                        }
                    }
                }
            }
        }
        RestartDecision();
    }

    protected bool GameObjectIsPlayer(GameObject other)
    {
        return other.GetComponent<PlayerController>();
    }

    private void CheckDecisionAttributes()
    {
        if (choices == null || choices.Length != 3)
        {
            Debug.LogError("This decision has either more or less than needed.");
        }

        if (choices[0] == choices[1] || choices[0] == choices[2] || choices[1] == choices[2])
        {
            Debug.LogError("Two or more Choices in the " + gameObject.name + " system are equal");
        }

        if (agreement != 2 && agreement != 3)
        {
            Debug.LogError("No agreement man.");
        }

        if (choiceTexts == null || choiceTexts.Length != 3)
        {
            Debug.LogError("This decision has no text setted for its buttons.");
        }
    }

    #endregion


    #region Messaging

    protected void SendVote(Choice vote)
    {
		int playerId = levelManager.GetLocalPlayerController().playerId;

		SendMessageToServer("PlayerVote/" + playerId + "/" + vote);
    }

    public void SendPreVote(int preVote)
    {
        int playerId = levelManager.GetLocalPlayerController().playerId;

        SendMessageToServer("PlayerPreVote/" + playerId + "/" + preVote);
    }

    public void ReceiveVote(int playerId, Choice vote)
    {
        votes[playerId] = vote;

        if (votes[0] != null && votes[1] != null && votes[2] != null)
        {
            EvaluateVote();
        }
    }

    public void ReceivePreVote(int playerId, int vote)
    {
        levelManager.ReceivePreVote(playerId, vote);
    }

    protected void SendMessageToServer(string message)
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, false);
        }
    }

    #endregion

    #region Events

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameObjectIsPlayer(collision.gameObject))
        {
            PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
            pc.decisionName = name;
	        
			if (++playersWhoArrived == 3)
            {
				pc.StopMoving ();
                StartThisVoting();
			}
            else 
			{
                if (collision.GetComponent<PlayerController>().localPlayer)
                {
				    levelManager.ActivateNPCFeedback ("Han llegado tus compañeros?");
                }
			}

        }
    }


    public void StartThisVoting()
    {
        levelManager.StartVoting(choiceTexts);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            pc.decisionName = name;
            --playersWhoArrived;
        }
    }

    #endregion

}

