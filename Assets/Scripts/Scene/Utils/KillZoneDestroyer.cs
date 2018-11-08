using UnityEngine;

public class KillZoneDestroyer : MonoBehaviour
{

    #region Attributes

    protected GameObject killzoneKiller;
    protected GameObject killzone;

    #endregion

    #region Common

    public void SetKillzone(GameObject _killzoneKiller, GameObject _killzone)
    {
        killzoneKiller = _killzoneKiller;
        killzone = _killzone;
    }

    #endregion

    #region Events

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (killzone && killzoneKiller)
        {
            if (collision.gameObject.name == killzoneKiller.name)
            {
                Destroy(killzone);
                Destroy(gameObject);
            }
        }
    }

    #endregion

}
