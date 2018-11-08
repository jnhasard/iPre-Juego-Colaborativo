using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerController : PlayerController
{

    #region Attributes

    public GameObject parasiteMagedParticle;
    private bool jumpedInAir;

    #endregion

    protected override void Start()
    {
        base.Start();
        if (parasiteMagedParticle == null)
        {
            Debug.LogError("you must set your parasiteMagedParticle in your Engineer");
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
        if (levelManager.GetWarrior().IsWarriored(this.gameObject))
        {
            var attackType = new PunchController().GetType();
            string attackName = (isPowerOn) ? "SuperPunch" : "Punch";
            GameObject attackObject = (GameObject)Instantiate(Resources.Load(attackPrefabName + attackName));
            PunchController attackController = (PunchController)attackObject.GetComponent(attackType);
            return attackController;
        }

        else
        {
            var attackType = new ProjectileController().GetType();
            string attackName = (isPowerOn) ? "SuperProjectile" : "Projectile";

            GameObject attackObject = (GameObject)Instantiate(Resources.Load(attackPrefabName + attackName));
            ProjectileController attackController = (ProjectileController)attackObject.GetComponent(attackType);
            return attackController;
        }

    }

    protected PunchController GetPunch()
    {
        var attackType = new PunchController().GetType();
        string attackName = (isPowerOn) ? "SuperPunchController" : "PunchController";
        GameObject attackObject = (GameObject)Instantiate(Resources.Load(attackPrefabName + attackName));
        PunchController attackController = (PunchController)attackObject.GetComponent(attackType);
        return attackController;
    }

    public override void SetPowerState(bool active)
    {
        base.SetPowerState(active);

        if (active)
        {
            if (localPlayer)
            {
                SoundManager sManager = FindObjectOfType<SoundManager>();
                sManager.PlaySound(gameObject, GameSounds.EngineerPower, true);
            }
        }
        else if (active == false)
        {
            if (localPlayer)
            {
                SoundManager sManager = FindObjectOfType<SoundManager>();
                sManager.StopSound(gameObject, GameSounds.EngineerPower);
            }
        }
    }

    protected override bool IsJumping(bool isGrounded)
    {
        if (localPlayer)
        {

            if (!isPowerOn)
            {
                return base.IsJumping(isGrounded);
            }

            if (isGrounded)
            {
                jumpedInAir = false;
            }

            bool pressedJump = CnInputManager.GetButtonDown("Jump Button") && !justJumped;

            if (pressedJump && isGrounded && !remoteJumping)
            {
                Debug.LogError("I'll Do My First Jump");

                remoteJumping = true;
                SendPlayerDataToServer();
                return true;
            }

            if (pressedJump && !isGrounded && !jumpedInAir && !remoteJumping)
            {
                Debug.LogError("Gonna Jump Twice");
                remoteJumping = true;
                jumpedInAir = true;
                SendPlayerDataToServer();
                return true;
            }

            if (remoteJumping)
            {
                remoteJumping = false;
                SendPlayerDataToServer();
            }

            return false;
        }

        return remoteJumping;
    }

    public bool IsElectrified(GameObject playerOrMovable)
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
                        if (power.InPowerArea(playerOrMovable, true))
                        {
                            return true;
                        }
                    }
                }
                else if (power.attack != null)
                {
                    if (power.attack.GetType().Equals(new ProjectileController().GetType()))
                    {
                        if (power.InPowerArea(playerOrMovable, true))
                        {
                            return true;
                        }
                    }
                }

                else if (power.expectedParticle != null)
                {
                    if (power.expectedParticle.GetType().Equals(new EngineerPoweredParticles().GetType()))
                    {
                        if (power.InPowerArea(playerOrMovable, true))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public void ProtectedByMage(bool imProtected)
    {
    }

    #endregion
}

