using UnityEngine;

public class SwitchMovingObject : MovingObject
{

    #region Start

    protected override void Start()
    {
        base.Start();
        IgnoreSwitchMovingObjects();
    }

    #endregion

    #region Utils

    private void IgnoreSwitchMovingObjects()
    {
        SwitchMovingObject[] movingSwitchs = GameObject.FindObjectsOfType<SwitchMovingObject>();
        Collider2D collider = GetComponent<Collider2D>();

        foreach (SwitchMovingObject movingSwitch in movingSwitchs)
        {
            Physics2D.IgnoreCollision(collider, movingSwitch.GetComponent<Collider2D>());
        }
    }

    #endregion

}
