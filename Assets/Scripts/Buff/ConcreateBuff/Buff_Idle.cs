using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Idle : BuffPrototype
{
    public bool curStatus = false;
    public float probability = 0.3f;
    public float ppIncrement = 1;

    private void Update()
    {
        if(curStatus != BattleDataManager.instance.playerMoving)
        {
            curStatus = BattleDataManager.instance.playerMoving;

            if(curStatus == false)
            {
                if(Random.Range(0,1f) < GlobalValue.GetTrueProbaility(probability))
                {
                    PlayerManager.instance.ChangePowerPoint(ppIncrement);
                }
            }
        }
    }

    private void OnEnable()
    {
        if (!activated)
        {
            activated = true;
            curStatus = BattleDataManager.instance.playerMoving;
        }
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
        }
    }
}
