using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Dizzy : BuffPrototype
{
    public float increment = 0.15f;
    public int costThreshold = 3;

    private void OnEnable()
    {
        if (!activated)
        {
            activated = true;
            GlobalValue.costThreshold = costThreshold;
            GlobalValue.damageIncrement_Special += increment;
        }
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
            GlobalValue.costThreshold = -1;
            GlobalValue.damageIncrement_Special -= increment;
        }
    }
}
