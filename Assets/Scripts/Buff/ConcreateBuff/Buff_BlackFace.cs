using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_BlackFace : BuffPrototype
{
    public float interval = 20f;
    Coroutine timer;

    public IEnumerator Timer()
    {
        yield return new WaitForSeconds(interval);

        foreach(var i in BattleDataManager.instance.enemyList)
        {
            Debug.Log(i.transform.parent + " ‹µΩÃÏ∑£");
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
