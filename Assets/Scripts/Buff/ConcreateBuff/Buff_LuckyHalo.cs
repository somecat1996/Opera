using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_LuckyHalo : BuffPrototype
{
    public float probabilityIncrement = 0.05f;

    private void OnEnable()
    {
        if (!activated)
        {
            activated = true;
            GlobalValue.probabilityIncrement_Event += probabilityIncrement;
        }
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
            GlobalValue.probabilityIncrement_Event -= probabilityIncrement;
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
