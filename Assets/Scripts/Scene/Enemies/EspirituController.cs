using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EspirituController : EnemyController
{

    #region Start & Update

    protected override void Start()
    {
        force = new Vector2(1500f, 200f);
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
