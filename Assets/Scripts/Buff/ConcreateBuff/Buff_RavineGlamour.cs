using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_RavineGlamour : BuffPrototype
{
    public float cirtProbability = 0.5f;
    public float cirtIncrement = 0.5f;

    private void OnEnable()
    {
        if (!activated)
        {
            GlobalValue.probability_Crit_Magic += GlobalValue.GetTrueProbaility(cirtProbability);
            GlobalValue.critIncrement_Magic += cirtIncrement;

            activated = true;
        }
    }

    private void OnDisable()
    {
        if (activated)
        {
            GlobalValue.probability_Crit_Magic -= GlobalValue.GetTrueProbaility(cirtProbability);
            GlobalValue.critIncrement_Magic -= cirtIncrement;

            activated = false;
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
