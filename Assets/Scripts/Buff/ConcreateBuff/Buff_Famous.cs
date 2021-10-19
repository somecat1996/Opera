using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Famous : BuffPrototype
{
    public int cur_Spectator = 0;
    public float damageIncrement = 0;

    void Update()
    {
        if (activated)
        {
            if (cur_Spectator != BattleDataManager.instance.activatedSpectator + BattleDataManager.instance.highlightSpectator)
            {
                cur_Spectator = BattleDataManager.instance.activatedSpectator + BattleDataManager.instance.highlightSpectator;

                // œ» ’ªÿ
                GlobalValue.damageIncrement_General -= damageIncrement;
                damageIncrement = cur_Spectator * 0.03f;
                GlobalValue.damageIncrement_General += damageIncrement;
            }
        }
    }

    private void OnEnable()
    {
        if (!activated)
        {
            activated = true;
        }   
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
            GlobalValue.damageIncrement_General -= damageIncrement;
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
