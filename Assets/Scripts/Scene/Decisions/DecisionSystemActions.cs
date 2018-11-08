using UnityEngine;
using System.Collections;

public class DecisionSystemActions : MonoBehaviour
{
	private LevelManager levelManager;

	#region Start
	public DecisionSystemActions()
	{
		levelManager = FindObjectOfType<LevelManager> ();
	}
	#endregion

    #region Common

    public void DoSomething(DecisionSystem.Choice choice)
    {

        switch(choice)
        {
            case DecisionSystem.Choice.ChoosePathUp:
                HandleChoice0();
                break;
			case DecisionSystem.Choice.ChoosePathMiddle:
				HandleChoice1();
				break;
			case DecisionSystem.Choice.ChoosePathBottom:
				HandleChoice2();
				break;
            case DecisionSystem.Choice.AceptarNPC:
                HandleChoice3();
                break;
            case DecisionSystem.Choice.NegarNPC:
                HandleChoice4();
                break;
            case DecisionSystem.Choice.Pensar:
                HandleChoice5();
                break;
            default: 
				Debug.LogError ("no handler for this " + choice + " SHIT");
				break;
        }
    }

    #endregion


    #region Handlers

    protected void HandleChoice0()
    {
		levelManager.InstantiatePortal("MageTeleporter", new Vector2(23.73f, 23.78f), new Vector2(23f, 22f)); //Solucionar Vectores}
    }

	protected void HandleChoice1()
	{
		levelManager.InstantiatePortal("WarriorTeleporter", new Vector2(23.73f, 23.28f), new Vector2(24f, 22f)); //Solucionar Vectores}
	}

	protected void HandleChoice2()
	{
		levelManager.InstantiatePortal("EnginTeleporter", new Vector2(23.73f, 22.78f), new Vector2(25f, 22f)); //Solucionar Vectores}	
	}

    protected void HandleChoice3()
    {
        levelManager.InstantiatePrefab("EngranajeA", new Vector2(-7.27f, -3.87f)); //Solucionar Vectores e instanciar nueva decisión.
        levelManager.ActivateNPCFeedback("Excelente! Los esperaré. ¡Y recuerden, 1500 de EXP!");
    }

    protected void HandleChoice4()
    {
        levelManager.InstantiatePortal("EnginTeleporter", new Vector2(23.73f, 22.78f), new Vector2(25f, 22f)); //Solucionar Vectores}	
    }

    protected void HandleChoice5()
    {
        levelManager.RestartVoting();
        levelManager.ActivateNPCFeedback("Ok, vuelvan cuando lo decidan");
        StartCoroutine(WaitForFeedback(levelManager.GetWaitToNPC()));
    }

    #endregion

    private IEnumerator WaitForFeedback(float timeToWaitForFeedBack)
    {
        yield return new WaitForSeconds(timeToWaitForFeedBack);
    }

}
