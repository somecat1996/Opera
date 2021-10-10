using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_TheMoreTheBetter : BuffPrototype
{
    public float increment = 0.2f;

    private void OnEnable()
    {
        if (!activated)
        {
            activated = true;
            GlobalValue.lootIncrement += increment;
        }
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
            GlobalValue.lootIncrement -= increment;
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
