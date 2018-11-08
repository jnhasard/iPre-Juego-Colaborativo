using System.Collections;
using System; 
using UnityEngine;

public class SwitchActions : MonoBehaviour
{

    #region Attributes

    private bool activated;
    private int groupId;

    public GameObject exp;

    public float spacing = 2f;
    public float gridX = 5f;
    public float gridY = 5f;

	private LevelManager levelManager; 

    #endregion

    #region Constructor

    public SwitchActions(GroupOfSwitchs group)
    {
		levelManager = FindObjectOfType <LevelManager> ();
        groupId = group.groupId;
        Debug.Log("Activated Group Of Switches " + groupId);
        foreach (Switch switchi in group.GetSwitchs())
        {
            switchi.SetJobDone();
        }
    }

    #endregion

    #region Common

    public void DoSomething()
    {
        if (activated)
        {
            return;
        }

        activated = true;

        switch (groupId)
        {

            // Aquí comienzan acciones switch etapa 2

            case 0:
                HandlerGroup0();
                break;

            case 1:
                HandlerGroup1();
                break;

            case 2:
                HandlerGroup2();
                break;

            case 3:
                HandlerGroup3();
                break;
            case 4:
                HandlerGroup4();
                break;

            case 5:
                HandlerGroup5();
                break;

            case 6:
                HandlerGroup6();
                break;

            case 7:
                HandlerGroup7();
                break;

            case 8:
                HandlerGroup8();
                break;

            // Aquí comienzan acciones switch etapa 1

            case 9:     // Primeros peldaños
                HandlerGroup9();
                break;

            case 10:    // peldaño switch 2
                HandlerGroup10();
                break;

            case 11:    // peldaño 3rd Switch + Exp
                HandlerGroup11();
                break;

            case 12:    // OpenPaths
                HandlerGroup12();
                break;

            case 13:    // to the end of scene
                HandlerGroup13();
                break;

            // Cases Scene 3 

            case 14:    // Mueve máquina de engranajes para Yellow
                HandlerGroup14();
                break;
            case 15:    
                HandlerGroup15();
                break;
            case 16:    
                HandlerGroup16();
                break;
            case 17:                //DamagingNeutrals from Warrior
                HandlerGroup17();
                break;
            case 18:    
                HandlerGroup18();
                break;
            case 19:  
                HandlerGroup19();
                break;
            case 20:   
                HandlerGroup20();
                break;
            case 21:
                HandlerGroup21();
                break;
            case 22:
                HandlerGroup22();
                break;
            case 23:
                HandlerGroup23();
                break;
            case 24:
                HandlerGroup24();
                break;
            case 25:
                HandlerGroup25();
                break;
            case 26:
                HandlerGroup26();
                break;
            case 27:
                HandlerGroup27();
                break;
            case 28:
                HandlerGroup28();
                break;
            case 29:
                HandlerGroup29();
                break;
            case 30:
                HandlerGroup30();
                break;
            case 31:
                HandlerGroup31();
                break;
            case 32:
                HandlerGroup32();
                break;
            case 33:
                HandlerGroup33();
                break;
            case 34:
                HandlerGroup34();
                break;
            case 35:
                HandlerGroup35();
                break;
            case 36:
                HandlerGroup36();
                break;
            case 37:
                HandlerGroup37();
                break;
            case 38:
                HandlerGroup38();
                break;
            case 39:
                HandlerGroup39();
                break;
            case 40:
                HandlerGroup40();
                break;
            case 41:
                HandlerGroup41();
                break;
            case 42:
                HandlerGroup42();
                break;
            case 43:
                HandlerGroup43();
                break;
            case 44:
                HandlerGroup44();
                break;
            case 45:
                HandlerGroup45();
                break;
            case 46:
                HandlerGroup46();
                break;
            case 47:
                HandlerGroup47();
                break;
            case 48:
                HandlerGroup48();
                break;
            case 49:
                HandlerGroup49();
                break;
            case 50:
                HandlerGroup50();
                break;
            case 51:
                HandlerGroup51();
                break;
            case 52:
                HandlerGroup52();
                break;
            case 53:
                HandlerGroup53();
                break;
            case 54:
                HandlerGroup54();
                break;
            case 55:
                HandlerGroup55();
                break;
            case 56:
                HandlerGroup56();
                break;
            case 57:
                HandlerGroup57();
                break;
            case 58:
                HandlerGroup58();
                break;
            case 59:
                HandlerGroup59();
                break;
            case 60:
                HandlerGroup60();
                break;
            case 61:
                HandlerGroup61();
                break;
            case 62:
                HandlerGroup62();
                break;
            case 63:
                HandlerGroup63();
                break;
            case 64:
                HandlerGroup64();
                break;
            case 65:
                HandlerGroup65();
                break;
            case 66:
                HandlerGroup66();
                break;
            case 67:
                HandlerGroup67();
                break;
            case 68:
                HandlerGroup68();
                break;
            case 69:
                HandlerGroup69();
                break;
            case 70:
                HandlerGroup70();
                break;
            default:
                return;
        }
    }

    #endregion

    #region Handlers

    //Switches escena 2

    private void HandlerGroup0()
    {
        GameObject platEngineer = levelManager.InstantiatePrefab("MovPlatform", new Vector2(13.63f, -1.96f));
        levelManager.SetMovingObjectData(platEngineer, new Vector2(13.63f, -1.96f), new Vector2(13.63f, 0.5f), 1f, 1.5f, false);
        levelManager.ShowFeedbackParticles("FBMageButt", new Vector2(13.63f, -1.96f), 3f);
    }

    private void HandlerGroup1()
    {
        levelManager.ShowFeedbackParticles("FBMageButt", new Vector2(-25.83f, 16.9f), 4f);

        SendMessageToServer("ObstacleDestroyed/LavaPool", true);

        levelManager.DestroyObject("CajaSwitchFierro", .1f);
		levelManager.DestroyObject("RejaEng", .1f);
		levelManager.DestroyObject("SpikesDead", .1f);
        levelManager.DestroyObject("LavaPool", .1f);


        GameObject sueloMetal = GameObject.Find("SueloMetal");
        sueloMetal.AddComponent<PlatformEffector2D>();
    }

    private void HandlerGroup2()
    {
        CameraController mainCamera = GameObject.Find("MainCamera").GetComponent<CameraController>();
        TriggerCamera tCamera = GameObject.Find("TriggerCameraForSwitchGroup2").GetComponent<TriggerCamera>();

        mainCamera.ChangeState(CameraState.TargetZoom, tCamera.movements[0]);
          
		SlideRock rocaGigante = FindObjectOfType<SlideRock>();
		rocaGigante.Slide();
		Debug.Log ("Got Rock");

        levelManager.InstatiateSprite("Arrows/engineerArrowUp", new Vector2(45.3f, -6.42f));

        BendTree bendTree = FindObjectOfType<BendTree>();
		bendTree.Fall();
		Debug.Log ("Got Tree");

        levelManager.InstatiateSprite("Arrows/engineerArrowUp", new Vector2(33.07f, -6.3f));
        levelManager.InstatiateSprite("Arrows/mageArrowDown", new Vector2(35.16f, -6.3f));
        levelManager.InstatiateSprite("Arrows/warriorArrowDown", new Vector2(35.95f, -6.3f));
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(34.51f, -3.88f));

        GameObject sFeedback = GameObject.Find("ActivateNPCForAllSwitches");
        Destroy(sFeedback);
    }

    private void HandlerGroup3()
    {
        GameObject platLadder = levelManager.InstantiatePrefab("MovPlatform", new Vector2(42.8f, -16.6f));

        Vector2 startPos = platLadder.transform.position;
        Vector2 endPos = new Vector2(startPos.x, -11.9f);
        levelManager.SetMovingObjectData(platLadder, startPos, endPos, 0.8f, 1f, false);

        levelManager.ShowFeedbackParticles("FBMageButt", new Vector2(41.4f, -16.3f), 3f);
		levelManager.TogglePlayerFilter("FilterMage", true);
    }

    private void HandlerGroup4()
    {
        levelManager.InstantiatePortal("AnyPlayerTeleporter", new Vector2(17.86f, -9.7f), new Vector2(6.9f, -13.82f));
    }

    private void HandlerGroup5()
    {
		levelManager.InstantiatePrefab("PlataformaPastVoladora", new Vector2(39f, 7.5f));
		levelManager.InstantiatePrefab("PlataformaPastVoladora", new Vector2(35.5f, 7.5f));

    }

    private void HandlerGroup6()
    {
		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(14.1f, -6.3f));
		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(14.3f, -6.3f));
		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(13.6f, -6.3f));
		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(13.1f, -6.3f));
		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(15f, -6.3f));
		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(15f, -6.3f));
    }

    private void HandlerGroup7()
    {
		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(62f, -14.23f));

		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(62.5f, -14.23f));
		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(64f, -14.23f));
		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(61.5f, -14.23f));
		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(63f, -14.23f));
		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(63.5f, -14.23f));
		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(62f, -14.23f));
		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(62.5f, -14.23f));
		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(64f, -14.23f));
		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(61.5f, -14.23f));
		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(63f, -14.23f));
		levelManager.InstantiatePrefab("Ambientales/Exp", new Vector2(63.5f, -14.23f));
    }

    private void HandlerGroup8()
    {
        GameObject platparaMage = levelManager.InstantiatePrefab("MovPlatform", new Vector2(61.2f, -9.5f));

        Vector2 startPos = platparaMage.transform.position;
        Vector2 endPos = new Vector2(startPos.x, startPos.y + 1.3f);

        levelManager.SetMovingObjectData(platparaMage, startPos, endPos, 1f, 1.5f, false);

        /* Instantiate Arrow feedback y cambiar arrow de warrior*/
        GameObject sprite = GameObject.Find("CartelCambiante");
        Destroy(sprite);

        levelManager.InstantiatePortal("WarriorTeleporter", new Vector2(82.64f, -18.55f), new Vector2(37.23f, -5.58f), true, 7);

        // EXP

        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(80.95f, -18.52f));
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(79.95f, -18.52f));

    }

    // Switches Escena 1

    private void HandlerGroup9()
    {
		levelManager.ShowFeedbackParticles("FBMageButt", new Vector2(26.5f, -43.6f), 4f);

		levelManager.InstantiatePrefab("SueloMetalFlotante", new Vector2(24.78f, -42.31f));
		levelManager.InstantiatePrefab("SueloMetalFlotante", new Vector2(24.78f, -43.16f));
    }

    private void HandlerGroup10()
    {
		levelManager.ShowFeedbackParticles("FBMageButt", new Vector2(26.5f, -42.11f), 4f);
		levelManager.InstantiatePrefab("SueloMetalFlotante", new Vector2(24.86f, -41.2f));
    }

    private void HandlerGroup11()
    {
		levelManager.InstantiatePrefab("SueloMetalFlotanteX2", new Vector2(26.17f, -40.34f));
    }

    private void HandlerGroup12()
    {
		levelManager.ShowFeedbackParticles("FBMageButt", new Vector2(32.11f, -39.31f), 4f);
		levelManager.InstantiatePrefab("TutorialPaths", new Vector2(35.6f, -38.95f));
		levelManager.DestroyObject("PathBlocker", .1f);
    }

    private void HandlerGroup13()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        levelManager.localPlayer.respawnPosition = new Vector3(136.15f, -26.33f, 1f);
        levelManager.Respawn();
    }


    // Switches ESCENA 3

    private void HandlerGroup14()
    {
		GameObject maquinaEngranajesYellow = levelManager.FindGameObject("DesplazarEngranajes");
		GameObject plataformaBajoEngranajes = levelManager.FindGameObject("PlataformaMovilYellow");
		GameObject firstSpikesYellow = levelManager.FindGameObject("FirstSpikes");
        GameObject spikesFeedBack = levelManager.FindGameObject("ActivateNPCSpikes");

        if (spikesFeedBack)
        {
            Destroy(spikesFeedBack);
        }


        OneTimeMovingObject moverEngranajes = maquinaEngranajesYellow.GetComponent<OneTimeMovingObject>();
        OneTimeMovingObject movePlataformaBajoEngranajes = plataformaBajoEngranajes.GetComponent<OneTimeMovingObject>();
        OneTimeMovingObject spikesMover = firstSpikesYellow.GetComponent<OneTimeMovingObject>();


        moverEngranajes.move = true;
        movePlataformaBajoEngranajes.move = true;
        spikesMover.move = true;


        GameObject maquina1Engin = GameObject.Find("MaqEngranaje1");
        GameObject maquina2Engin = GameObject.Find("MaqEngranaje2");

		levelManager.StartAnimatorBool("StartMoving", true, maquina1Engin);
		levelManager.StartAnimatorBool("StartMoving", true, maquina2Engin);

    }
    private void HandlerGroup15()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();

        if (levelManager.GetEngineer().localPlayer)
        {
            CameraController camera = FindObjectOfType<CameraController>();
            TriggerCamera tCamera = GameObject.Find("TriggerCameraForEnginScene3").GetComponent<TriggerCamera>();
            camera.ChangeState(CameraState.TargetZoom, tCamera.movements[0]);
        }
        levelManager.DestroyObject("ActivateNPCForEnginDestroyabe", .1f);
        levelManager.DestroyObject("CajaSwitchFierro", 2.9f);
        levelManager.InstantiatePortal("EnginTeleporter", new Vector2(-11.3f, 17.5f), new Vector2(14.1f, 15.7f));
        levelManager.InstantiatePortal("MageTeleporter", new Vector2(-11.3f, 22.05f), new Vector2(25.1f, -1.1f));
        levelManager.InstantiatePortal("WarriorTeleporter", new Vector2(-21.77f, 17.2f), new Vector2(26.06f, -12.9f));
    }

    private void HandlerGroup16()
    {
        levelManager.InstantiatePrefab("RunaR1", new Vector2(-39.35f, 16f));
    }


    private void HandlerGroup17()  //   Handlers Scene 6 //
    {
        Vector2 startPosition = new Vector2(38.54f, -7.97f);
        Vector2 endPosition = new Vector2(38.54f, -10.94f);

        GameObject mPlatform = levelManager.InstantiatePrefab("MovPlatform", startPosition);
        levelManager.SetMovingObjectData(mPlatform, startPosition, endPosition, 1f, .8f, false);
    }

    private void HandlerGroup18()
    {
        GameObject cajaFierro = GameObject.Find("CajaSwitchFierro2RR");
        if (cajaFierro)
        {
            Destroy(cajaFierro);
        }
    }

    private void HandlerGroup19()
    {
        // Set Active Mucha EXP por todos los Switch
        
    }

    private void HandlerGroup20()
    {
        Vector2 expZone = FindObjectOfType<VectorTeleportAssigner>().WhereAmIGoing("4C");
        Vector2 c2Zone = new Vector2(-5.3f, -6.8f);
        Vector2 l2Zone = new Vector2(-13.47f, -6.8f);

        levelManager.InstantiatePortal("AnyPlayerTeleporter", c2Zone, expZone, true, 25, "2C", "4C");
        levelManager.InstantiatePortal("AnyPlayerTeleporter", l2Zone, expZone, true, 25, "2L", "4C");
    }

    private void HandlerGroup21()
    {
        GameObject obstacle = GameObject.Find("SueloMetal3LLL");
        if (obstacle)
        {
            Destroy(obstacle);
        }
    }

    private void HandlerGroup22()
    {
        Vector2 v2c6 = FindObjectOfType<VectorTeleportAssigner>().WhereAmIGoing("6C");
        levelManager.InstantiatePortal("AnyPlayerTeleporter", new Vector2(-55f, -22.86f), v2c6, true, 25, "3LLL", "6C"); 
    }

    private void HandlerGroup23()
    {
        //  Mucha exp en la zona 
    }

    private void HandlerGroup24()
    {
        Vector2 v2c6 = FindObjectOfType<VectorTeleportAssigner>().WhereAmIGoing("6C");
        levelManager.InstantiatePortal("AnyPlayerTeleporter", new Vector2(-64.25f, -21.74f), v2c6, true, 25, "3RRR", "6C");
    }

    private void HandlerGroup25()
    {
        Vector2 v2c6 = FindObjectOfType<VectorTeleportAssigner>().WhereAmIGoing("6C");
        levelManager.InstantiatePortal("AnyPlayerTeleporter", new Vector2(50.41f, -30.85f), v2c6, true, 25, "3RRR", "6C");
    }

    private void HandlerGroup26()
    {
        GameObject cajaFierro = GameObject.Find("CajaSwitchFierro4RR");
        if (cajaFierro)
        {
            Destroy(cajaFierro);
        }
    }
    private void HandlerGroup27()   //End of hardest zone in scene 6: 5C
    {
        Vector2 v2c6 = FindObjectOfType<VectorTeleportAssigner>().WhereAmIGoing("7C");
        levelManager.InstantiatePortal("AnyPlayerTeleporter", new Vector2(39.9f, -56.75f), v2c6, true, 25, "5C", "7C");
    }

    private void HandlerGroup28()   // Scene5 Zone 5
    {
        GameObject filter = GameObject.Find("KillZoneForAllHomies");
        Destroy(filter);
    }
    private void HandlerGroup29()   // Teleport for the arrival of players to hardest Zone
    {
        levelManager.InstantiatePortal("AnyPlayerTeleporter", new Vector2(6.58f, -55.41f), new Vector2(32.71f, -57.3f));
    }

    private void HandlerGroup30()
    {
    }
    private void HandlerGroup31()
    {
    }
    private void HandlerGroup32()
    {
    }

    private void HandlerGroup33()
    {
        levelManager.InstantiatePrefab("Items/RunaA1", new Vector2(39.91f, 13.61f));
        levelManager.InstantiatePrefab("Items/RunaV1", new Vector2(52.22f, -5.04f));
        levelManager.InstantiatePrefab("Items/RunaR1", new Vector2(43.32f, -12.53f));
    }

    private void HandlerGroup34()
    {
    }
    private void HandlerGroup35()
    {
    }

    private void HandlerGroup36()
    {
    }

    private void HandlerGroup37()
    {
    }

    private void HandlerGroup38()
    {
    }

    private void HandlerGroup39()
    {
    }

    private void HandlerGroup40()
    {
    }
    private void HandlerGroup41()
    {
    }
    private void HandlerGroup42()
    {
    }
    private void HandlerGroup43()
    {
    }
    private void HandlerGroup44()
    {
    }
    private void HandlerGroup45()
    {
    }
    private void HandlerGroup46()
    {
    }

    private void HandlerGroup47()
    {
    }
    private void HandlerGroup48()
    {
    }
    private void HandlerGroup49()
    {
		levelManager.InstantiatePortal("MageTeleporter", new Vector2(62.03f, -5.48f), new Vector2(83f, -5.5f)); 
		levelManager.InstantiatePortal("WarriorTeleporter", new Vector2(50.07f, -12.6f), new Vector2(82.5f, -5.5f)); 
		levelManager.InstantiatePortal("EnginTeleporter", new Vector2(49.66f, 13.25f), new Vector2(82f, -5.5f)); 
		levelManager.PowerableToggleLavaIntoWater ("WaterFalling", true, 4);
    }

    // Switches Scene 4

    private void HandlerGroup50()
    {
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(3f, -2f));
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(4f, -2f));
    }

    private void HandlerGroup51()
    {
        levelManager.InstantiatePortal("WarriorTeleporter", new Vector2(-49.273f, -1.56f), new Vector2(-20.7f, 0.5f), true, 1);
        BurnableObject treeToBurn = GameObject.Find("TreeAltarHolder").GetComponent<BurnableObject>();
        treeToBurn.Burn();

        GameObject mageFeedback = GameObject.Find("ActivateNPCForGreen2");
        if (mageFeedback)
        {
            Destroy(mageFeedback);
        }
    }

    private void HandlerGroup52()
    {
        GameObject bInstantiator = GameObject.Find("BubbleExitUp");
        if (bInstantiator)
        {
            BubbleRotatingInstantiator bIController = bInstantiator.GetComponent<BubbleRotatingInstantiator>();
            bIController.GearActivation();
        }

        GameObject kZone = GameObject.Find("KillZoneForAll");
        if (kZone)
        {
            Destroy(kZone);
        }

        GameObject kZoneFB = GameObject.Find("ActivateNPCKillingObject");
        if (kZoneFB)
        {
            Destroy(kZoneFB);
        }

        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(-32.05f, 7.68f));
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(-32.05f, 7.05f));
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(-32.80f, 7.33f));
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(-31.30f, 7.33f));

        // This one is for the group of Switches in the warriorGearZone;
    }

    private void HandlerGroup53()
    {
        GameObject platEngineer = levelManager.InstantiatePrefab("MovPlatform", new Vector2(-31.76f, 35.69f));
        levelManager.SetMovingObjectData(platEngineer, new Vector2(-31.76f, 35.69f), new Vector2(-39.76f, 33.45f), 1f, 2f, false);

       // MustAlsoInstantiateAlotOfEXP!

        GameObject platEngineer2 = levelManager.InstantiatePrefab("MovPlatform", new Vector2(-30.14f, 29.64f));
        levelManager.SetMovingObjectData(platEngineer2, new Vector2(-30.14f, 29.64f), new Vector2(-39.76f, 32.7f), 1f, 2f, false);
    }

    // For Scene 5

    private void HandlerGroup54() // Switch For ForLoop Destroyer 1: Check // End Of Zone1 in Scene 6
    {
        levelManager.InstantiatePortal("WarriorTeleporter", new Vector2(-29.76f, -13.44f), new Vector2(-2f, .1f), true, 3);
        SwitchForLoopDestroyer swDestroyer = GameObject.Find("SwitchBoxDestroyer1").GetComponent<SwitchForLoopDestroyer>();
        swDestroyer.SwitchReady("Rojo");
    }

    private void HandlerGroup55() // Switch For ForLoop Destroyer 1 // End Of Zone1 in Scene 6
    {
        levelManager.InstantiatePortal("EnginTeleporter", new Vector2(1.76f, -23.48f), new Vector2(-2f, .1f), true, 4);
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(0f, -23.76f));
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(-7.8f, -23.76f));
        SwitchForLoopDestroyer swDestroyer = GameObject.Find("SwitchBoxDestroyer1").GetComponent<SwitchForLoopDestroyer>();
        swDestroyer.SwitchReady("Amarillo");

    }

    private void HandlerGroup64() //Switch For ForLoop Destroyer 1: Check  // End Of Zone1 in Scene 6
    {
        levelManager.InstantiatePortal("MageTeleporter", new Vector2(-41.78f, 1.64f), new Vector2(-2.2f, .1f), true, 2);
        SwitchForLoopDestroyer swDestroyer = GameObject.Find("SwitchBoxDestroyer1").GetComponent<SwitchForLoopDestroyer>();
        swDestroyer.SwitchReady("Verde");       
    }
        
    private void HandlerGroup56() ////Unlocks BubbleInstantiator in Zone 1 for Mage +Exp: Check
    {
        BubbleRotatingInstantiator bInstantiator = GameObject.Find("BubbleInstantiatorMageZone").GetComponent<BubbleRotatingInstantiator>();
        bInstantiator.GearActivation();

        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(-46.19f, -4.47f));
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(-44.69f, -4.47f));

    }


    private void HandlerGroup57() //MovesObjects and opens path to Altar Engin and instantiatesTeleport in Zone 5: Check
    {
        levelManager.InstantiatePortal("EnginTeleporter", new Vector2(-97.34f, 0.85f), new Vector2(-56.15f, 16.4f));
        GameObject mBground = GameObject.Find("DarkMovableBackground");
        if (mBground)
        {
            mBground.GetComponent<OneTimeMovingObject>().move = true;
        }

        GameObject mPlatformThick = GameObject.Find("MovablePlatformThick");
        if (mPlatformThick)
        {
            mPlatformThick.GetComponent<OneTimeMovingObject>().move = true;
        }

        GameObject mWall = GameObject.Find("MovableWallForEngin");
        if (mWall)
        {
            mWall.GetComponent<OneTimeMovingObject>().move = true;
        }
    }

    private void HandlerGroup58() // Destroys Box For Powerable In Zone 6 : Check
    {
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(57.034f, 15.6f));
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(55.774f, 15.6f));
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(54.69f, 15.6f));

        GameObject caja = GameObject.Find("CajaSwitchFierro");
        if (caja)
        {
            Destroy(caja);
        }

    }

    private void HandlerGroup59() // Teleporter to EndOfZone6: Check
    {
        levelManager.InstantiatePortal("AnyPlayerTeleporter", new Vector2(39.87f, 37f), new Vector2(80f, 41f));
    }

    private void HandlerGroup60() // Teleporter to EndOfZone6: Check
    {
        levelManager.InstantiatePortal("AnyPlayerTeleporter", new Vector2(69.68f, 39.41f), new Vector2(80f, 41f));

    }

    private void HandlerGroup61() // Teleporter to EndOfZone6: Check
    {
        levelManager.InstantiatePortal("AnyPlayerTeleporter", new Vector2(80.62f, 31.32f), new Vector2(80f, 41f));

    }

    private void HandlerGroup62() //End of Zone 6 in Scene 5
    {
        levelManager.InstantiatePortal("AnyPlayerTeleporter", new Vector2(113f, 42.8f), new Vector2(.1f, .1f), true, 22);
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(111.53f, 42.8f));
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(110.69f, 42.8f));
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(109.82f, 42.8f));

        ForLoopDestroyer fDestroyer = GameObject.Find("BoxesHolder").GetComponent<ForLoopDestroyer>();
        fDestroyer.DestroyOneMoreObject();
    }   

    private void HandlerGroup63()       //ThisOneIsForScene3
    {
        GameObject burnableBoxes = GameObject.Find("CajasParaMage");
        burnableBoxes.GetComponent<ObjectsToDestroy>().BurnAllThisStuff();
    }

    // Handler64 está sobre el handler56// 

    private void HandlerGroup65() //  End Of Zone 4 in Scene 5
    {
        levelManager.InstantiatePortal("AnyPlayerTeleporter", new Vector2(34.03f, 6.26f), new Vector2(-2f, .1f), true, 20);
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(33.73f, 6.26f));
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(33.43f, 6.26f));
        levelManager.InstantiatePrefab("Exp/ExpFeedback35", new Vector2(33.13f, 6.26f));
            
        ForLoopDestroyer fDestroyer = GameObject.Find("BoxesHolder").GetComponent<ForLoopDestroyer>();
        fDestroyer.DestroyOneMoreObject();
    }

    private void HandlerGroup66() // Switches para zona 3 Mage
    {
        levelManager.InstantiatePortal("AnyPlayerTeleporter", new Vector2(-11.62f, 44.3f), new Vector2(-12f, 34.5f));

        SwitchForLoopDestroyer swDestroyer = GameObject.Find("SwitchBoxDestroyer3").GetComponent<SwitchForLoopDestroyer>();
        swDestroyer.SwitchReady("Verde");

        SwitchForLoopDestroyer swDestroyer2 = GameObject.Find("SwitchBoxDestroyerZone3").GetComponent<SwitchForLoopDestroyer>();
        swDestroyer2.SwitchReady("Verde");
    }

    private void HandlerGroup67()  // Switches para zona 3 Warrior
    {
        levelManager.InstantiatePortal("AnyPlayerTeleporter", new Vector2(10f, 45f), new Vector2(-12f, 34.5f));

        SwitchForLoopDestroyer swDestroyer = GameObject.Find("SwitchBoxDestroyer3").GetComponent<SwitchForLoopDestroyer>();
        swDestroyer.SwitchReady("Rojo");

        SwitchForLoopDestroyer swDestroyer2 = GameObject.Find("SwitchBoxDestroyerZone3").GetComponent<SwitchForLoopDestroyer>();
        swDestroyer2.SwitchReady("Rojo");


    }
    private void HandlerGroup68() // PAra Zona 3 Engin
    {
        levelManager.InstantiatePortal("AnyPlayerTeleporter", new Vector2(10.8f, 39.5f), new Vector2(-12f, 34.5f));
        SwitchForLoopDestroyer swDestroyer = GameObject.Find("SwitchBoxDestroyer3").GetComponent<SwitchForLoopDestroyer>();
        swDestroyer.SwitchReady("Amarillo");

        SwitchForLoopDestroyer swDestroyer2 = GameObject.Find("SwitchBoxDestroyerZone3").GetComponent<SwitchForLoopDestroyer>();
        swDestroyer2.SwitchReady("Amarillo");
    }

    private void HandlerGroup69()  // Para Zona 2
    {
        levelManager.InstantiatePortal("AnyPlayerTeleporter", new Vector2(86f, -25f), new Vector2(-2f, .1f));
        levelManager.InstantiatePortal("AnyPlayerTeleporter", new Vector2(86f, -28.5f), new Vector2(-2f, .1f));
        levelManager.InstantiatePortal("AnyPlayerTeleporter", new Vector2(83f, -19.2f), new Vector2(-2f, .1f));

        SwitchForLoopDestroyer swDestroyer = GameObject.Find("SwitchBoxDestroyer2").GetComponent<SwitchForLoopDestroyer>();
        swDestroyer.SwitchReady("Amarillo");
        swDestroyer.SwitchReady("Rojo");
        swDestroyer.SwitchReady("Verde");

    }

    private void HandlerGroup70()  //End Of Scene 5
    {
        CameraController mainCamera = GameObject.Find("MainCamera").GetComponent<CameraController>();
        TriggerCamera tCamera = GameObject.Find("TriggerCameraForSwitchGroup70").GetComponent<TriggerCamera>();
        mainCamera.ChangeState(CameraState.TargetZoom, tCamera.movements[0]);

        levelManager.InstantiatePrefab("PortalEndOfScene", new Vector2(7.74f, 7.84f));
        GameObject eOfScene = levelManager.InstantiatePrefab("EndOfScene", new Vector2(7.43f, 6.76f));
        EndOfScene theEnd = eOfScene.GetComponent<EndOfScene>();
        theEnd.playersToArrive = 3;

        GameObject oPlatform1 = levelManager.InstantiatePrefab("/Ambientales/OrnatedTreePlatform", new Vector2(10.02f, 8.38f));
        Transform oPlatTransform = oPlatform1.GetComponent<Transform>();
        oPlatTransform.localScale = new Vector3(-1, 1, 1);

        GameObject oPlatform2 = levelManager.InstantiatePrefab("/Ambientales/OrnatedTreePlatform", new Vector2(5.58f, 8.38f));

    }

    private void HandlerGroup71()
    {
        GameObject sueloMetalicoWarrior = GameObject.Find("SueloMetalWarriorSecretBlocker");
        Destroy(sueloMetalicoWarrior);
    }

    private void HandlerGroup72()
    {
        GameObject sueloMetalicoMage = GameObject.Find("SueloMetalMageSecretBlocker");
        Destroy(sueloMetalicoMage);
    }

    private void HandlerGroup73()
    {
        GameObject sueloMetalicoEngin = GameObject.Find("SueloMetalEnginSecretBlocker");
        Destroy(sueloMetalicoEngin);
    }

    #endregion

    #region Utils


    #endregion

    #region Messaging

    private void SendMessageToServer(string message, bool secure)
    {
        if (Client.instance && Client.instance.GetLocalPlayer() && Client.instance.GetLocalPlayer().controlOverEnemies)
        {
            Client.instance.SendMessageToServer(message, secure);
        }
    }

    #endregion

}