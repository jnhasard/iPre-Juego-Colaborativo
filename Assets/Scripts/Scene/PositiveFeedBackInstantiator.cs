using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositiveFeedBackInstantiator : MonoBehaviour
{

    private SoundManager soundManager;
    private int playersArrived;
    private bool beenUsed;

    public Vector2[] instantiationVectors;
    public string[] prefabNames;
    public string[] playersNeeded;
    public bool mustDoSomething;


    private void Start()
    {
        playersArrived = 0;
    }

    public bool BeenUsed ()
    {
        return beenUsed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            for (int i = 0; i < playersNeeded.Length; i++)
            {
                if (playersNeeded[i] != null)
                {
                    if (playersNeeded[i] == collision.gameObject.name)
                    {
                        playersArrived++;
                        playersNeeded[i] = null;

                        if (playersArrived == playersNeeded.Length)
                        {
                            if (!beenUsed)
                            {
                                for (int j = 0; j < prefabNames.Length; j++)
                                {
                                    LevelManager levelManager = FindObjectOfType<LevelManager>();
                                    levelManager.InstantiatePrefab(prefabNames[j], instantiationVectors[j]);
                                }

                                beenUsed = true;
                            }

                            if (mustDoSomething)
                            {
                                DoYourThing();
                            }
                        }
                    }
                }
            }
        }
    }

    private void DoYourThing()
    {
        switch(gameObject.name)
        {
            case "PositiveFeedBackForGreenRune":
                HandleCase1();
                break;
            default:
                return;
        }
    }

    private void HandleCase1()
    {
        GameObject cartelVerde = GameObject.Find("mageArrowLeft4Others");
        GameObject helpOthersFeedback = GameObject.Find("ActivateNPCForMage4");

        Destroy(helpOthersFeedback);
        Destroy(cartelVerde);
    }
}
