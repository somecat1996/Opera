using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Famous : BuffPrototype
{
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
        }
    }
}
