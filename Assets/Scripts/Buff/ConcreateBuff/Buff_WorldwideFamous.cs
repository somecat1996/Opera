using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_WorldwideFamous : BuffPrototype
{

    public float counter_Origin = 100;
    public float counter;

    public float appealPoint = 0;

    public float hpIncrement = 10;


    private void Update()
    {
        if (appealPoint != BattleDataManager.instance.appealPoint)
        {
            appealPoint = BattleDataManager.instance.appealPoint;

            if(appealPoint >= 100 && !activated)
            {
                activated = true;
                GlobalValue.hpIncrement_Reward += 10;
            }

        }
    }

    private void OnEnable()
    {
;
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
            GlobalValue.hpIncrement_Reward -= 10;
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
