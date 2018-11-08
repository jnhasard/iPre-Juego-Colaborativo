using UnityEngine;

public class MageController : PlayerController
{
    #region Attributes

    protected static float shieldArea;

    #endregion

    #region Start & Update

    protected override void Start()
    {
        base.Start();
        shieldArea = 0;
        LoadShieldArea();
    }

    #endregion

    #region Common

    public bool ProtectedByShield(GameObject player)
    {
        // Check for player proximity to mage with activated shield
        if (isPowerOn)
        {
            if (Vector2.Distance(player.transform.position, transform.position) <= shieldArea)
            {
                return true;
            }
        }

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
                    if (power.attack.GetType().Equals(new FireballController().GetType()))
                    {
                        if (power.InPowerArea(player, true))
                        {
                            return true;
                        }
                    }
                }

                else if (power.expectedParticle != null)
                {
                    if (power.expectedParticle.GetType().Equals(new MagePoweredParticles().GetType()))
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

        #region Utils

    protected override AttackController GetAttack()
    {
        if (levelManager.GetWarrior().IsWarriored(gameObject))
        {
            var attackType = new PunchController().GetType();
            string attackName = (isPowerOn) ? "SuperPunch" : "Punch";
            GameObject attackObject = (GameObject)Instantiate(Resources.Load(attackPrefabName + attackName));
            PunchController attackController = (PunchController)attackObject.GetComponent(attackType);
            return attackController;
        }
        else
        {
            var attackType = new FireballController().GetType();
            string attackName = (isPowerOn) ? "SuperFireball" : "Fireball";
            GameObject attackObject = (GameObject)Instantiate(Resources.Load(attackPrefabName + attackName));
            FireballController attackController = (FireballController)attackObject.GetComponent(attackType);
            return attackController;
        }
    }

    public override void SetPowerState(bool active)
    {
        base.SetPowerState(active);
        if (active == false)
        {
            SoundManager sManager = FindObjectOfType<SoundManager>();
            sManager.StopSound(gameObject, GameSounds.MagePower);

            DamagingObject[] lavas = FindObjectsOfType<DamagingObject>();
            foreach(DamagingObject lava in lavas)
            {
                if (lava.id > 0)
                {
                    if (lava.turnedIntoWater)
                    {
                        lava.ChangeLavaIntoWater(false);
                    }
                }
            }
        }
        if (active)
        {
            SoundManager sManager = FindObjectOfType<SoundManager>();
            sManager.PlaySound(gameObject, GameSounds.MagePower, true);
        }
    }


    protected void LoadShieldArea()
    {
        foreach (GameObject particle in particles)
        {

            float radius = particle.GetComponent<ParticleSystem>().shape.radius;
            if (shieldArea < radius)
            {
                shieldArea = radius;
            }
        }
    }

    #endregion

}
