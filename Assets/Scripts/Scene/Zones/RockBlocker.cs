using System.Collections;
using UnityEngine;

public class RockBlocker : IgnoreCollisionWithAttacks
{

    protected void Start()
    {
        IgnoreCollisionWithPlayers();
    }

    private void IgnoreCollisionWithPlayers()
    {
        Collider2D collider = gameObject.GetComponent<Collider2D>();

        GameObject player1 = GameObject.Find("Verde");
        GameObject player2 = GameObject.Find("Rojo");
        GameObject player3 = GameObject.Find("Amarillo");
        Physics2D.IgnoreCollision(collider, player1.GetComponent<Collider2D>(), true);
        Physics2D.IgnoreCollision(collider, player2.GetComponent<Collider2D>(), true);
        Physics2D.IgnoreCollision(collider, player3.GetComponent<Collider2D>(), true);

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<MovableObject>())
        {
            PushOtherObject(collision.gameObject);
            StopOtherObjectIfNeeded(collision.gameObject);
        }
    }

    protected void PushOtherObject(GameObject objectToPush)
    {
        if (objectToPush.GetComponent<Rigidbody2D>())
        {
            Rigidbody2D otherBody = objectToPush.GetComponent<Rigidbody2D>();
            float incomingHorizontalForce = otherBody.velocity.x * otherBody.mass;
            float incomingVertical = otherBody.velocity.y * otherBody.mass;

            Push(otherBody);
        }
    }

    protected void StopOtherObjectIfNeeded(GameObject objectToStop)
    {
        StartCoroutine(WaitTillStop(objectToStop));
    }

    protected void Push(Rigidbody2D bodyToPush)
    {
        if (ComesFromLeft(bodyToPush))
        {
            bodyToPush.AddForce(new Vector2(-3500000, 100));
        }
        else if (ComesFromRight(bodyToPush))
        {
            bodyToPush.AddForce(new Vector2(3500000, 100));
        }
    }

    protected bool ComesFromLeft(Rigidbody2D bodyToPush)
    {
        return bodyToPush.transform.position.x <= gameObject.transform.position.x;
    }
    protected bool ComesFromRight(Rigidbody2D bodyToPush)
    {
        return bodyToPush.transform.position.x >= gameObject.transform.position.x;
    }
    protected IEnumerator WaitTillStop(GameObject objectToStop)
    {
        yield return new WaitForSeconds(1f);
        Rigidbody2D rb2d = objectToStop.GetComponent<Rigidbody2D>();
        rb2d.constraints = RigidbodyConstraints2D.FreezePositionX;

        yield return new WaitForSeconds(1f);
        rb2d.constraints = RigidbodyConstraints2D.None;
    }
}
