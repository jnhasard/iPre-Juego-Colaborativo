using UnityEngine;

public class DamagingArrow : DamagingObject
{

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            DealDamage(other.gameObject);
            DestroyMeAndParticles();
        }

        if (GameObjectIsEnemy(other.gameObject))
        {
            if (CheckIfImWarriored(gameObject))
            {
                KillEnemy(other.gameObject);
                DestroyMeAndParticles();
            }

            if (CheckIfImMaged())
            {
                GetThisEnemyMaged(other.gameObject);
                DestroyMeAndParticles();
            }

            DestroyMeAndParticles();
        }

        if (GameObjectIsDestroyable(other.gameObject))
        {
            if (CheckIfImWarriored(gameObject))
            {
                DestroyableObject destroyable = other.gameObject.GetComponent<DestroyableObject>();
                destroyable.DestroyMe(true);
                DestroyMeAndParticles();
            }

            DestroyMeAndParticles();
        }

        if (GameObjectIsDeactivableKillPlane(other.gameObject))
        {
            if (CheckIfImWarriored(gameObject))
            {
                KillingObject kObject = other.gameObject.GetComponent<KillingObject>();
                kObject.HitByPoweredDamaging();
                DestroyMeAndParticles();
            }

            if (CheckIfImMaged())
            {
                KillingObject kObject = other.gameObject.GetComponent<KillingObject>();
                kObject.HitByPoweredDamaging();
                DestroyMeAndParticles();
            }
        }

        if (GameObjectIsBurnable(other.gameObject))
        {
           if (CheckIfImWarriored(gameObject))
            {
                BurnableObject bObject = other.gameObject.GetComponent<BurnableObject>();
                bObject.Burn();
                DestroyMeAndParticles();
            }
            DestroyMeAndParticles();
        }
    }
}
