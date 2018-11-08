using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableBox : DestroyableObject
{

    #region Attributes

    public Sprite brokenBox;
    public GameObject metalCorners;

    #endregion

    #region Common

    public override void DestroyMe(bool destroyedFromLocal)
    {
        Collider2D collider = GetComponent<Collider2D>();
        Destroy(collider);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = brokenBox;
        Destroy(metalCorners);

        base.DestroyMe(destroyedFromLocal);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
		if (!collision.gameObject.GetComponent<AttackController>() || !collision.gameObject.GetComponent <BubbleController>() || !collision.gameObject.GetComponent <BubbleParticleController>())
        {
            Rigidbody2D rgbd = gameObject.GetComponent<Rigidbody2D>();
            rgbd.velocity = Vector3.zero;
            rgbd.angularVelocity = 0f;
        }
			
    }

    #endregion

}