using UnityEngine;
using System.Collections;

public class GearSystemActions : ActivableSystemActions
{
    public float blockerSpeed;

    #region Common

    public void DoSomething(GearSystem gearSystem, bool notifyOthers)
    {

        switch (gearSystem.name)
        {
            case "MaquinaEngranajeA":
                HandleGearSystemA(gearSystem, notifyOthers);
                break;
            case "MaquinaEngranajeB":
                HandleGearSystemB(gearSystem, notifyOthers);
                break;
			case "MaquinaEngranajeC":
				HandleGearSystemC(gearSystem, notifyOthers);
				break;
			case "MaquinaEngranajeD":
				HandleGearSystemD(gearSystem, notifyOthers);
				break;
            case "MaquinaEngranajeE":
                HandleGearSystemE(gearSystem, notifyOthers);
                break;
            case "MaquinaEngranajeF":
                HandleGearSystemF(gearSystem, notifyOthers);
                break;
            case "MaquinaEngranajeG":
                HandleGearSystemG(gearSystem, notifyOthers);
                break;
            case "MaquinaEngranajeH":
                HandleGearSystemH(gearSystem, notifyOthers);
                break;
            case "MaquinaEngranajeI":
                HandleGearSystemI(gearSystem, notifyOthers);
                break;
            case "MaquinaEngranajeJ":
                HandleGearSystemJ(gearSystem, notifyOthers);
                break;
            case "MaquinaEngranajeK":
                HandleGearSystemK(gearSystem, notifyOthers);
                break;
            case "MaquinaEngranajeL":
                HandleGearSystemL(gearSystem, notifyOthers);
                break;
        }

    }

    #endregion

    #region Handlers

    private void HandleGearSystemA(GearSystem gearSystem, bool notifyOthers)
    {

        // Dispose every used gear in case of reconnection
        for (int i = 0; i < gearSystem.components.Length; i++)
        {
            string usedGearName = gearSystem.components[i].sprite.name;
            GameObject usedGear = GameObject.Find(usedGearName);

            if (usedGear)
            {
                DestroyObject(usedGearName, .1f);
            }

        }

        // Hide every placed gear
        SpriteRenderer[] componentSlots = gearSystem.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < componentSlots.Length; i++)
        {
            componentSlots[i].sprite = null;
        }

        // Change the gearsystem sprite

        SpriteRenderer systemSpriteRenderer = gearSystem.GetComponent<SpriteRenderer>();
        systemSpriteRenderer.sprite = gearSystem.activatedSprite;


        gearSystem.ToggleParticles(true);
        SetAnimatorBool("startMoving", true, gearSystem);


        GameObject secondMachine = GameObject.Find("MaqEngranaje2");
        if (secondMachine)
        {
            ActivableSystem secondGear = secondMachine.GetComponent<ActivableSystem>();
            SetAnimatorBool("startMoving", true, secondGear);
        }

        MoveTowardsAndDie blocksMover = GameObject.Find("GiantBlockers").GetComponent<MoveTowardsAndDie>();
        blocksMover.StartMoving(gearSystem.GetParticles());

        if (notifyOthers)
        {
            SetAnimatorBool("startMovingMachine", false, gearSystem, 2f);
        }

        if (Object.FindObjectOfType<Planner>())
        {
            if (gearSystem.switchObj)
            {
                gearSystem.switchObj.ActivateSwitch();

                Planner planner = Object.FindObjectOfType<Planner>();
                planner.Monitor();
            }
        }

        if (notifyOthers)
        {
            SendMessageToServer("ObstacleDestroyed/GiantBlockers", true);
            SendMessageToServer("ActivateSystem/" + gearSystem.name, true);
        }
    }

    private void HandleGearSystemB(GearSystem gearSystem, bool notifyOthers)
    {

        // Dispose every used gear in case of reconnection
        for (int i = 0; i < gearSystem.components.Length; i++)
        {
            string usedGearName = gearSystem.components[i].sprite.name;
            GameObject usedGear = GameObject.Find(usedGearName);

            if (usedGear)
            {
                DestroyObject(usedGearName, .1f);
            }

        }

        // Hide every placed gear
        SpriteRenderer[] componentSlots = gearSystem.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < componentSlots.Length; i++)
        {
            componentSlots[i].sprite = null;
        }

        // Change the gearsystem sprite

        SpriteRenderer systemSpriteRenderer = gearSystem.GetComponent<SpriteRenderer>();
        systemSpriteRenderer.sprite = gearSystem.activatedSprite;

        // Doing Something

        SetAnimatorBool("startMoving", true, gearSystem);
        OneTimeMovingObject altarEngin1 = GameObject.Find("AltarEnginMovable").GetComponent<OneTimeMovingObject>();
        altarEngin1.move = true;
        DestroyObject("ActivateNPCForGear", .1f);


        //  Planner 
        if (Object.FindObjectOfType<Planner>())
        {
            if (gearSystem.switchObj)
            {
                gearSystem.switchObj.ActivateSwitch();

                Planner planner = Object.FindObjectOfType<Planner>();
                planner.Monitor();
            }
        }

        if (notifyOthers)
        {
            SendMessageToServer("ActivateSystem/" + gearSystem.name, true);
        }

    }

	private void HandleGearSystemC(GearSystem gearSystem, bool notifyOthers)
	{

		// Dispose every used gear in case of reconnection
		for (int i = 0; i < gearSystem.components.Length; i++)
		{
			string usedGearName = gearSystem.components[i].sprite.name;
			GameObject usedGear = GameObject.Find(usedGearName);

			if (usedGear)
			{
				DestroyObject(usedGearName, .1f);
			}

		}

		// Hide every placed gear
		SpriteRenderer[] componentSlots = gearSystem.GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < componentSlots.Length; i++)
		{
			componentSlots[i].sprite = null;
		}

		// Change the gearsystem sprite

		SpriteRenderer systemSpriteRenderer = gearSystem.GetComponent<SpriteRenderer>();
		systemSpriteRenderer.sprite = gearSystem.activatedSprite;
        SetAnimatorBool("startMoving", true, gearSystem);


        // Doing Something

        TeleportersEndOfScene tEndOfScene = GameObject.Find ("TeleportersEndOfScene").GetComponent<TeleportersEndOfScene>();
		tEndOfScene.GearActivation ();

        //  Planner 
        if (Object.FindObjectOfType<Planner>())
        {
            if (gearSystem.switchObj)
            {
                gearSystem.switchObj.ActivateSwitch();

                Planner planner = Object.FindObjectOfType<Planner>();
                planner.Monitor();
            }
        }

        if (notifyOthers)
		{
			SendMessageToServer("ActivateSystem/" + gearSystem.name, true);
		}
	}

	private void HandleGearSystemD(GearSystem gearSystem, bool notifyOthers)
	{

		// Dispose every used gear in case of reconnection
		for (int i = 0; i < gearSystem.components.Length; i++)
		{
			string usedGearName = gearSystem.components[i].sprite.name;
			GameObject usedGear = GameObject.Find(usedGearName);

			if (usedGear)
			{
				DestroyObject(usedGearName, .1f);
			}

		}

		// Hide every placed gear
		SpriteRenderer[] componentSlots = gearSystem.GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < componentSlots.Length; i++)
		{
			componentSlots[i].sprite = null;
		}

		// Change the gearsystem sprite

		SpriteRenderer systemSpriteRenderer = gearSystem.GetComponent<SpriteRenderer>();
		systemSpriteRenderer.sprite = gearSystem.activatedSprite;

        // Doing Something

        TeleportersEndOfScene tEndOfScene = GameObject.Find("TeleportersEndOfScene").GetComponent<TeleportersEndOfScene>();
        tEndOfScene.GearActivation();

        //  Planner 
        if (Object.FindObjectOfType<Planner>())
        {
            if (gearSystem.switchObj)
            {
                gearSystem.switchObj.ActivateSwitch();

                Planner planner = Object.FindObjectOfType<Planner>();
                planner.Monitor();
            }
        }

        if (notifyOthers)
		{
			SendMessageToServer("ActivateSystem/" + gearSystem.name, true);
		}
	}

    private void HandleGearSystemE(GearSystem gearSystem, bool notifyOthers)
    {

        // Dispose every used gear in case of reconnection
        for (int i = 0; i < gearSystem.components.Length; i++)
        {
            string usedGearName = gearSystem.components[i].sprite.name;
            GameObject usedGear = GameObject.Find(usedGearName);

            if (usedGear)
            {
                DestroyObject(usedGearName, .1f);
            }

        }

        // Hide every placed gear
        SpriteRenderer[] componentSlots = gearSystem.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < componentSlots.Length; i++)
        {
            componentSlots[i].sprite = null;
        }

        // Change the gearsystem sprite

        SpriteRenderer systemSpriteRenderer = gearSystem.GetComponent<SpriteRenderer>();
        systemSpriteRenderer.sprite = gearSystem.activatedSprite;

        // Doing Something

        BubbleRotatingInstantiator bInstantiatior = GameObject.Find("BubbleCentralInstatiator").GetComponent<BubbleRotatingInstantiator>();
        bInstantiatior.GearActivation();

        //  Planner 
        if (Object.FindObjectOfType<Planner>())
        {
            if (gearSystem.switchObj)
            {
                gearSystem.switchObj.ActivateSwitch();

                Planner planner = Object.FindObjectOfType<Planner>();
                planner.Monitor();
            }
        }

        if (notifyOthers)
        {
            SendMessageToServer("ActivateSystem/" + gearSystem.name, true);
        }
    }
    private void HandleGearSystemF(GearSystem gearSystem, bool notifyOthers)
    {

        // Dispose every used gear in case of reconnection
        for (int i = 0; i < gearSystem.components.Length; i++)
        {
            string usedGearName = gearSystem.components[i].sprite.name;
            GameObject usedGear = GameObject.Find(usedGearName);

            if (usedGear)
            {
                DestroyObject(usedGearName, .1f);
            }

        }

        // Hide every placed gear
        SpriteRenderer[] componentSlots = gearSystem.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < componentSlots.Length; i++)
        {
            componentSlots[i].sprite = null;
        }

        // Change the gearsystem sprite

        SpriteRenderer systemSpriteRenderer = gearSystem.GetComponent<SpriteRenderer>();
        systemSpriteRenderer.sprite = gearSystem.activatedSprite;

        // Doing Something

        BubbleRotatingInstantiator bInstantiatior = GameObject.Find("BubbleCentralInstatiator").GetComponent<BubbleRotatingInstantiator>();
        bInstantiatior.GearActivation();

        //  Planner 
        if (Object.FindObjectOfType<Planner>())
        {
            if (gearSystem.switchObj)
            {
                gearSystem.switchObj.ActivateSwitch();

                Planner planner = Object.FindObjectOfType<Planner>();
                planner.Monitor();
            }
        }

        if (notifyOthers)
        {
            SendMessageToServer("ActivateSystem/" + gearSystem.name, true);
        }
    }
    private void HandleGearSystemG(GearSystem gearSystem, bool notifyOthers)
    {

        // Dispose every used gear in case of reconnection
        for (int i = 0; i < gearSystem.components.Length; i++)
        {
            string usedGearName = gearSystem.components[i].sprite.name;
            GameObject usedGear = GameObject.Find(usedGearName);

            if (usedGear)
            {
                DestroyObject(usedGearName, .1f);
            }

        }

        // Hide every placed gear
        SpriteRenderer[] componentSlots = gearSystem.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < componentSlots.Length; i++)
        {
            componentSlots[i].sprite = null;
        }

        // Change the gearsystem sprite

        SpriteRenderer systemSpriteRenderer = gearSystem.GetComponent<SpriteRenderer>();
        systemSpriteRenderer.sprite = gearSystem.activatedSprite;

        // Doing Something

        BubbleRotatingInstantiator bInstantiatior = GameObject.Find("BubbleCentralInstatiator").GetComponent<BubbleRotatingInstantiator>();
        bInstantiatior.GearActivation();

        //  Planner 
        if (Object.FindObjectOfType<Planner>())
        {
            if (gearSystem.switchObj)
            {
                gearSystem.switchObj.ActivateSwitch();

                Planner planner = Object.FindObjectOfType<Planner>();
                planner.Monitor();
            }
        }

        if (notifyOthers)
        {
            SendMessageToServer("ActivateSystem/" + gearSystem.name, true);
        }
    }
    private void HandleGearSystemH(GearSystem gearSystem, bool notifyOthers)
    {

        // Dispose every used gear in case of reconnection
        for (int i = 0; i < gearSystem.components.Length; i++)
        {
            string usedGearName = gearSystem.components[i].sprite.name;
            GameObject usedGear = GameObject.Find(usedGearName);

            if (usedGear)
            {
                DestroyObject(usedGearName, .1f);
            }

        }

        // Hide every placed gear
        SpriteRenderer[] componentSlots = gearSystem.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < componentSlots.Length; i++)
        {
            componentSlots[i].sprite = null;
        }

        // Change the gearsystem sprite

        SpriteRenderer systemSpriteRenderer = gearSystem.GetComponent<SpriteRenderer>();
        systemSpriteRenderer.sprite = gearSystem.activatedSprite;

        // Doing Something

        BubbleRotatingInstantiator bInstantiatior = GameObject.Find("BubbleCentralInstatiator").GetComponent<BubbleRotatingInstantiator>();
        bInstantiatior.GearActivation();

        //  Planner 
        if (Object.FindObjectOfType<Planner>())
        {
            if (gearSystem.switchObj)
            {
                gearSystem.switchObj.ActivateSwitch();

                Planner planner = Object.FindObjectOfType<Planner>();
                planner.Monitor();
            }
        }

        if (notifyOthers)
        {
            SendMessageToServer("ActivateSystem/" + gearSystem.name, true);
        }
    }
    private void HandleGearSystemI(GearSystem gearSystem, bool notifyOthers)
    {

        // Dispose every used gear in case of reconnection
        for (int i = 0; i < gearSystem.components.Length; i++)
        {
            string usedGearName = gearSystem.components[i].sprite.name;
            GameObject usedGear = GameObject.Find(usedGearName);

            if (usedGear)
            {
                DestroyObject(usedGearName, .1f);
            }

        }

        // Hide every placed gear
        SpriteRenderer[] componentSlots = gearSystem.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < componentSlots.Length; i++)
        {
            componentSlots[i].sprite = null;
        }

        // Change the gearsystem sprite

        SpriteRenderer systemSpriteRenderer = gearSystem.GetComponent<SpriteRenderer>();
        systemSpriteRenderer.sprite = gearSystem.activatedSprite;

        // Doing Something

        BubbleRotatingInstantiator bInstantiatior = GameObject.Find("BubbleCentralInstatiator").GetComponent<BubbleRotatingInstantiator>();
        bInstantiatior.GearActivation();

        //  Planner 
        if (Object.FindObjectOfType<Planner>())
        {
            if (gearSystem.switchObj)
            {
                gearSystem.switchObj.ActivateSwitch();

                Planner planner = Object.FindObjectOfType<Planner>();
                planner.Monitor();
            }
        }

        if (notifyOthers)
        {
            SendMessageToServer("ActivateSystem/" + gearSystem.name, true);
        }
    }
    private void HandleGearSystemJ(GearSystem gearSystem, bool notifyOthers)
    {

        // Dispose every used gear in case of reconnection
        for (int i = 0; i < gearSystem.components.Length; i++)
        {
            string usedGearName = gearSystem.components[i].sprite.name;
            GameObject usedGear = GameObject.Find(usedGearName);

            if (usedGear)
            {
                DestroyObject(usedGearName, .1f);
            }

        }

        // Hide every placed gear
        SpriteRenderer[] componentSlots = gearSystem.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < componentSlots.Length; i++)
        {
            componentSlots[i].sprite = null;
        }

        // Change the gearsystem sprite

        SpriteRenderer systemSpriteRenderer = gearSystem.GetComponent<SpriteRenderer>();
        systemSpriteRenderer.sprite = gearSystem.activatedSprite;

        // Doing Something

        BubbleRotatingInstantiator bInstantiatior = GameObject.Find("BubbleCentralInstatiator").GetComponent<BubbleRotatingInstantiator>();
        bInstantiatior.GearActivation();

        //  Planner 
        if (Object.FindObjectOfType<Planner>())
        {
            if (gearSystem.switchObj)
            {
                gearSystem.switchObj.ActivateSwitch();

                Planner planner = Object.FindObjectOfType<Planner>();
                planner.Monitor();
            }
        }

        if (notifyOthers)
        {
            SendMessageToServer("ActivateSystem/" + gearSystem.name, true);
        }
    }
    private void HandleGearSystemK(GearSystem gearSystem, bool notifyOthers)
    {

        // Dispose every used gear in case of reconnection
        for (int i = 0; i < gearSystem.components.Length; i++)
        {
            string usedGearName = gearSystem.components[i].sprite.name;
            GameObject usedGear = GameObject.Find(usedGearName);

            if (usedGear)
            {
                DestroyObject(usedGearName, .1f);
            }

        }

        // Hide every placed gear
        SpriteRenderer[] componentSlots = gearSystem.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < componentSlots.Length; i++)
        {
            componentSlots[i].sprite = null;
        }

        // Change the gearsystem sprite

        SpriteRenderer systemSpriteRenderer = gearSystem.GetComponent<SpriteRenderer>();
        systemSpriteRenderer.sprite = gearSystem.activatedSprite;

        // Doing Something

        BubbleRotatingInstantiator bInstantiatior = GameObject.Find("BubbleCentralInstatiator").GetComponent<BubbleRotatingInstantiator>();
        bInstantiatior.GearActivation();

        //  Planner 
        if (Object.FindObjectOfType<Planner>())
        {
            if (gearSystem.switchObj)
            {
                gearSystem.switchObj.ActivateSwitch();

                Planner planner = Object.FindObjectOfType<Planner>();
                planner.Monitor();
            }
        }

        if (notifyOthers)
        {
            SendMessageToServer("ActivateSystem/" + gearSystem.name, true);
        }
    }
    private void HandleGearSystemL(GearSystem gearSystem, bool notifyOthers)
    {

        // Dispose every used gear in case of reconnection
        for (int i = 0; i < gearSystem.components.Length; i++)
        {
            string usedGearName = gearSystem.components[i].sprite.name;
            GameObject usedGear = GameObject.Find(usedGearName);

            if (usedGear)
            {
                DestroyObject(usedGearName, .1f);
            }

        }

        // Hide every placed gear
        SpriteRenderer[] componentSlots = gearSystem.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < componentSlots.Length; i++)
        {
            componentSlots[i].sprite = null;
        }

        // Change the gearsystem sprite

        SpriteRenderer systemSpriteRenderer = gearSystem.GetComponent<SpriteRenderer>();
        systemSpriteRenderer.sprite = gearSystem.activatedSprite;

        // Doing Something

        BubbleRotatingInstantiator bInstantiatior = GameObject.Find("BubbleCentralInstatiator").GetComponent<BubbleRotatingInstantiator>();
        bInstantiatior.GearActivation();

        //  Planner 
        if (Object.FindObjectOfType<Planner>())
        {
            if (gearSystem.switchObj)
            {
                gearSystem.switchObj.ActivateSwitch();

                Planner planner = Object.FindObjectOfType<Planner>();
                planner.Monitor();
            }
        }

        if (notifyOthers)
        {
            SendMessageToServer("ActivateSystem/" + gearSystem.name, true);
        }
    }
}

    #endregion

