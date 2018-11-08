using System.Collections;
using UnityEngine;

public class AttackTeleporter : MonoBehaviour
{

    #region Attributes

    public Vector2 startPosition;
    public Vector2 targetPosition;
    protected bool teleporting;

    #endregion

    #region Common

    private void TeleportAttack(AttackController attack)
    {
        attack.caster.CastLocalAttack(startPosition, targetPosition);
        Destroy(attack.gameObject, .1f);
    }

    #endregion

    #region Events

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<AttackController>())
        {
            AttackController attack = other.GetComponent<AttackController>();
            if (teleporting)
            {
                return;
            }

            if (attack)
            {
                teleporting = true;
                TeleportAttack(attack);
                StartCoroutine(WaitTillStopTeleporting());
            }
        }
    }

    private IEnumerator WaitTillStopTeleporting()
    {
        yield return new WaitForSeconds(.8f);
        teleporting = false; 
    }
    #endregion

}