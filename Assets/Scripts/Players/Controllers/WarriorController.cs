using System.Collections;
using UnityEngine;


public class WarriorController : PlayerController
{

    #region Attributes

    public GameObject parasiteMagedParticle;
    protected int attacks = 0;

    #endregion

    protected override void Start()
    {
        base.Start();
        if (parasiteMagedParticle == null)
        {
            Debug.LogError("you must set your parasiteMagedParticle in your Warrior");
        }
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        parasiteMagedParticle.transform.position = gameObject.transform.position;
    }

    #region Utils

    protected override AttackController GetAttack()
    {
        string attackName = (isPowerOn) ? "SuperPunch" : "Punch";
        var attackType = new PunchController().GetType();

        GameObject attackObject = (GameObject)Instantiate(Resources.Load(attackPrefabName + attackName));
        PunchController attackController = (PunchController)attackObject.GetComponent(attackType);

        return attackController;
    }

    public void ProtectedByMage(bool imProtected)
    {
        EnemyController[] eControllers = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemyController in eControllers)
        {
            enemyController.UpdateCollisionsWithPlayer(gameObject, imProtected);
        }
    }

    public bool IsWarriored(GameObject player)
    {
        PowerableObject[] powerables = FindObjectsOfType<PowerableObject>();

        foreach (PowerableObject powerable in powerables)
        {
            if (powerable.IsPowered())
            {
                PowerableObject.Power power = powerable.GetActivatedPower();

                if (power.caster != null)
                {
                    if (power.caster.Equals(this))
                    {
                        if (power.InPowerArea(player, true))
                        {
                            return true;
                        }
                    }
                }
                else if (power.attack != null)
                {
                    if (power.attack.GetType().Equals(new PunchController().GetType()))
                    {
                        if (power.InPowerArea(player, true))
                        {
                            return true;
                        }
                    }
                }

                else if (power.expectedParticle != null)
                {
                    if (power.expectedParticle.GetType().Equals(new WarriorPoweredParticles().GetType()))
                    {
                        if (power.InPowerArea(player, true))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    #endregion

}
