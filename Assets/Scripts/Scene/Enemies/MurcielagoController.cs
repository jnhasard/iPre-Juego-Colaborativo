using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurcielagoController : EnemyController
{

    #region Start & Update

    protected override void Start()
    {
        force = strenght;
        damage = 5;
        maxHp = 20f;
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (patrolling)
        {
            Patroll();
        }

    }

    #endregion

}
