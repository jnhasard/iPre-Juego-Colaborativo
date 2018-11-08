using UnityEngine;

public class BurningPowerable : PowerableObject
{
    #region Attributes

    private GameObject[] particles;

    #endregion

    #region Events

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (powered)
        {
            if (collision.gameObject.GetComponent<BurnableObject>())
            {
                BurnableObject bObject = collision.gameObject.GetComponent<BurnableObject>();
                bObject.Burn();
            }
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (powered)
        {
            if (collision.gameObject.GetComponent<BurnableObject>())
            {
                BurnableObject bObject = collision.gameObject.GetComponent<BurnableObject>();
                bObject.Burn();
            }
        }
    }

    #endregion

    #region Utils

    protected override void DoYourPowerableThing()
    {
        //
    }

    protected override void UndoYourPowerableThing()
    {
        //   
    }
    #endregion

}
