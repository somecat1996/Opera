using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_WorldwideFamous : BuffPrototype
{
    // 将伤害改成喝彩值即可

    public float counter_Origin = 100;
    public float counter;

    public float totalDamage;

    public float hpIncrement = 10;


    private void Update()
    {
        if (totalDamage != BattleDataManager.instance.totalDamage)
        {
            counter -= (BattleDataManager.instance.totalDamage - totalDamage);
            counter = Mathf.Clamp(counter, 0, Mathf.Infinity);

            totalDamage = BattleDataManager.instance.totalDamage;

            if (counter == 0)
            {
                PlayerManager.instance.ChangeHealthPoint(hpIncrement);
                counter = counter_Origin;
            }

        }
    }

    private void OnEnable()
    {
        totalDamage = BattleDataManager.instance.totalDamage;
        counter = counter_Origin;
    }

    private void OnDisable()
    {

    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
