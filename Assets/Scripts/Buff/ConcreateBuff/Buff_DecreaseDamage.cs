using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_DecreaseDamage : BuffPrototype
{
    public float decrement = 0.1f;

    private void OnEnable()
    {
        if (!activated)
        {
            activated = true;
            GlobalValue.damageDecrement_Player += decrement;
        }
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
            GlobalValue.damageDecrement_Player -= decrement;
        }
    }
}
