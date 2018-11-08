using UnityEngine;

public class GravityZone : MonoBehaviour
{

    #region Events

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>())
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            player.SetPositiveGravity(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>())
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            player.SetPositiveGravity(true);
        }
    }

    #endregion

}
