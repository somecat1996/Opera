using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_WorldwideFamous : BuffPrototype
{
    public int hpIncrement = 100;


    private void Update()
    {

    }

    private void OnEnable()
    {
;        if (!activated)
        {
            activated = true;
            GlobalValue.hpIncrement_Reward += hpIncrement;
        }
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
            GlobalValue.hpIncrement_Reward -= hpIncrement;
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
