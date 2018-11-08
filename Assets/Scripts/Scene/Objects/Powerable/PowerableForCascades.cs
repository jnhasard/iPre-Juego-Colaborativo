using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerableForCascades : PowerableObject {

    public int[] lavaId;

    protected override void DoYourPowerableThing()
    {
        for (int id = 0; id<lavaId.Length; id++)
        {
            levelManager.PowerableToggleLavaIntoWater("WaterFalling", true, lavaId[id]);
        }
    }

    protected override void UndoYourPowerableThing()
    {
        for (int id = 0; id < lavaId.Length; id++)
        {
            levelManager.PowerableToggleLavaIntoWater("WaterFalling", false, lavaId[id]);
        }
    }

}
