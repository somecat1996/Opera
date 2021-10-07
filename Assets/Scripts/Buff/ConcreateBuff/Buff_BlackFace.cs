using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_BlackFace : BuffPrototype
{
    public float percentage = 0.05f;
    public float maxDamage;

    public float interval = 20f;
    Coroutine timer;

    public IEnumerator Timer()  
    {
        yield return new WaitForSeconds(interval);

        Debug.Log("Hurt");
        foreach(var i in BattleDataManager.instance.enemyList)
        {
            i.PercentHurt(percentage, maxDamage);
            Debug.Log("Hit!");
        }

        timer = StartCoroutine(Timer());
    }

    private void OnEnable()
    {
        if (!activated)
        {
            activated = true;

            timer = StartCoroutine(Timer());
        }
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
            StopCoroutine(timer);
        }
    }
}
