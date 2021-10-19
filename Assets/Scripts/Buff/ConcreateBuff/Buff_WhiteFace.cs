using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_WhiteFace : BuffPrototype
{
    public float counter_Origin=1000;
    public float counter;

    public float totalDamage;

    public float hpIncrement = 1;


    private void Update()
    {
        if(totalDamage != BattleDataManager.instance.totalDamage)
        {
            counter -= (BattleDataManager.instance.totalDamage - totalDamage);
            counter = Mathf.Clamp(counter, 0, Mathf.Infinity);

            totalDamage = BattleDataManager.instance.totalDamage;

            if(counter == 0)
            {
                PlayerManager.instance.ChangeHealthPoint(hpIncrement);
                counter = counter_Origin;
            }


            buffGUIScript.UpdateCounter((int)counter);

        }
    }

    private void OnEnable()
    {
        Invoke("EnableCounter",Time.deltaTime);

        totalDamage = BattleDataManager.instance.totalDamage;
        counter = counter_Origin;

        Invoke("FirstUpdateCounter", Time.deltaTime);
    }

    void FirstUpdateCounter()
    {
        buffGUIScript.UpdateCounter((int)counter);
    }

    private void OnDisable()
    {
        
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
