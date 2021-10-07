using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Notable : BuffPrototype
{
    public float increment = 0.15f;

    private void OnEnable()
    {
        if (!activated)
        {
            activated = true;
            GlobalValue.rewardIncrement += increment;
        }
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
            GlobalValue.rewardIncrement -= increment;
        }
    }
}
