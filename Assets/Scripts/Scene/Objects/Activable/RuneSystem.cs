using System.Collections;
using UnityEngine;

public class RuneSystem : ActivableSystem
{

    #region Attributes

    public PlannerObstacle obstacleObj = null;

    #endregion

    #region Start

    protected override void Start()
    {
        base.Start();
        systemActions = new RuneSystemActions();
    }

    #endregion

}
