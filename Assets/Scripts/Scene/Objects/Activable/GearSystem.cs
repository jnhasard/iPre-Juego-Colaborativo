using System.Collections;
using UnityEngine;

public class GearSystem : ActivableSystem
{

    #region Attributes

    public PlannerSwitch switchObj;

    #endregion

    #region Start

    protected override void Start()
    {
        base.Start();
        systemActions = new GearSystemActions();
    }

    #endregion
}
