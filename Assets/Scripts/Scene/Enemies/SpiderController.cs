using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : EnemyController
{

    #region Attributes

    private Vector3 bottomTunnelPosition;
    private Vector3 upperTunnelPosition;
    private Vector3 lastPosition;

    private static float alertDistance = 2.1f;
    private static float movingSteps = .6f;

    private int timesHit;

    public string firstMessage;
    public string secondMessage; 
    public string thirdMessage; 
    #endregion

    #region Start & Update

    protected override void Start()
    {
        force = new Vector2(3500f, 150f);
        maxHp = 2000f;
        damage = 5;
        timesHit = 0;

        upperTunnelPosition = new Vector3(73.38f, 0.73f, transform.position.z);
        bottomTunnelPosition = new Vector3(upperTunnelPosition.x, -3.14f, transform.position.z);

        IgnoreCollisionsWithRock();

        lastPosition = transform.position;

        base.Start();
    }

    protected override void Update()
    {
        ProtectTunnel();
        UpdatePosition();

        lastPosition = transform.position;
    }

    #endregion

    #region Common

    protected void UpdatePosition()
    {
        if (lastPosition != transform.position)
        {
            SendPositionToServer();
        }
    }

    protected void ProtectTunnel()
    {

        if (levelManager.players == null || levelManager.players.Length == 0)
        {
            return;
        }

        bool playerIsInBottomTunnel = false;

        foreach (GameObject player in levelManager.players)
        {
            if (Mathf.Abs(transform.position.x - player.transform.position.x) < alertDistance)
            {
                if (player.transform.position.y <= 0f) // Is in the bottom tunnel
                {
                    playerIsInBottomTunnel = true;
                    break;
                }
            }
        }

        if (playerIsInBottomTunnel)
        {
            if (transform.position.y != bottomTunnelPosition.y)
            {
                transform.position = Vector3.MoveTowards(transform.position, bottomTunnelPosition, movingSteps);
            }
        }
        else
        {
            if (transform.position.y != upperTunnelPosition.y)
            {
                transform.position = Vector3.MoveTowards(transform.position, upperTunnelPosition, movingSteps);
            }
        }
    }

    protected override void Patroll()
    {
        //Spider can't Patroll
    }

    public override void TakeDamage(float damage)
    {
        // Spider doesn't take any damage boy
    }

    private void CheckTimesHit()
    {
        timesHit++;
        if (timesHit >= 1 && timesHit <=1)
        {
           levelManager.ActivateSpiderFeedback(firstMessage);
        }
        if (timesHit >= 15 && timesHit <=15)
        {
            levelManager.ActivateSpiderFeedback(secondMessage);
        }
        if (timesHit >= 30 && timesHit <= 30)
        {
            levelManager.ActivateNPCFeedback(thirdMessage);
            timesHit = -15;
        }
    }
    protected void EndFeedback()
    {
        levelManager.ShutSpiderFeedBack(true);
    }

    #endregion

    #region Utils

    protected void IgnoreCollisionsWithRock()
    {
        GameObject spiderRock = GameObject.Find("RocaGiganteAraña");

        foreach (Collider2D rockCollider in spiderRock.GetComponents<Collider2D>())
        {
            if (!rockCollider.isTrigger)
            {
                foreach (Collider2D spiderCollider in GetComponents<Collider2D>())
                {
                    if (!spiderCollider.isTrigger)
                    {
                        Physics2D.IgnoreCollision(rockCollider, spiderCollider);
                    }
                }
            }
        }
    }

    #endregion

    #region Messaging

    protected override void SendMessageToServer(string message, bool secure)
    {
        if (Client.instance && Client.instance.GetLocalPlayer() && Client.instance.GetLocalPlayer().controlOverEnemies)
        {
            Client.instance.SendMessageToServer(message, secure);
        }
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            Attack(other.gameObject);
        }

        if (GameObjectIsAttack(other.gameObject))
        {
            CheckTimesHit();
        }
    }

    private bool GameObjectIsAttack(GameObject other)
    {
        if (other.gameObject.GetComponent<AttackController>() != null)
        {
            AttackController aController = other.gameObject.GetComponent<AttackController>();
            return aController && aController.caster.localPlayer;
        }

        else
        {
            return false;
        }
    }

    #endregion

}
