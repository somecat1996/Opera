using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_SinkOrSwim : BuffPrototype
{
    public float increment = 0.2f;
    public float threshold = 0.2f;

    private void Update()
    {
        if(PlayerManager.instance.GetPercentage_HealthPoint() < threshold)
        {
            if (!activated)
            {
                activated = true;

                GlobalValue.damageIncrement_General += increment;
            }

        }
    }

    private void OnEnable()
    {
        if (activated)
            activated = false;
    }

    private void OnDisable()
    {
        if (activated)
        {
            GlobalValue.damageIncrement_General -= increment;
            activated = false;
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
