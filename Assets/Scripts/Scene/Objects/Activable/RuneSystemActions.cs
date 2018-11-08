using UnityEngine;


public class RuneSystemActions : ActivableSystemActions
{

    #region Common

    public void DoSomething(RuneSystem runeSystem, bool notifyOthers)
    {

        // Hide every placed rune
        SpriteRenderer[] componentSlots = runeSystem.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < componentSlots.Length; i++)
        {
            componentSlots[i].sprite = null;
        }

        // Dispose every used rune in case of reconnection
        for (int i = 0; i < runeSystem.components.Length; i++)
        {
            string usedRuneName = runeSystem.components[i].sprite.name;
            GameObject usedRune = GameObject.Find(usedRuneName);

            if (usedRune)
            {
                DestroyObject(usedRuneName, .1f);
            }

        }

        // Change the door sprite
        SpriteRenderer systemSpriteRenderer = runeSystem.GetComponent<SpriteRenderer>();
        if (runeSystem.activatedSprite != null)
        {
            systemSpriteRenderer.sprite = runeSystem.activatedSprite;
        }

        // Allow players to pass through the door
        if (runeSystem.GetComponent<Collider2D>())
        {
            Collider2D collider = runeSystem.GetComponent<Collider2D>();
            if (collider)
            {
                collider.enabled = false;
            }
        }

        if (Object.FindObjectOfType<Planner>())
        {
            if (runeSystem.obstacleObj != null)
            {
                runeSystem.obstacleObj.OpenDoor();

                Planner planner = Object.FindObjectOfType<Planner>();
                planner.Monitor();

            }
        }

        if (notifyOthers)
        {
            SendMessageToServer("ActivateSystem/" + runeSystem.name, true);
        }

    }

    #endregion

}
