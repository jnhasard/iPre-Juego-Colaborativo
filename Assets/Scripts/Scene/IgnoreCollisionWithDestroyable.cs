using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisionWithDestroyable : MonoBehaviour {

	// Use this for initialization
	void Start () {

        IgnoreCollisionWithDestroyableObjects(true);
		
	}

    public void IgnoreCollisionWithDestroyableObjects(bool ignore)
    {
        DestroyableObject[] objectsWhoHateMe = FindObjectsOfType<DestroyableObject>();

        if (objectsWhoHateMe != null)
        {
            foreach (DestroyableObject objectWhoHatesMe in objectsWhoHateMe)
            {
                Collider2D[] collidersWhoHateME = objectWhoHatesMe.GetComponents<Collider2D>();
                foreach (Collider2D hatingCollider in collidersWhoHateME)
                {
                    Collider2D[] myColliders = GetComponents<Collider2D>();
                    foreach (Collider2D colliderinho in myColliders)
                    {

                            Physics2D.IgnoreCollision(hatingCollider, GetComponent<Collider2D>(), ignore);
                    }
                }
            }
        }
    }
}
