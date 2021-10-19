using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Prepared : BuffPrototype
{
    private int counter_Origin = 5;
    public int counter;
    public float increment = 0.5f;

    private int cur_UsedCard;

    private void Update()
    {
        // 检测到使用卡牌数已更新
        if(cur_UsedCard != BattleDataManager.instance.totalUsedCard)
        {
            cur_UsedCard++;

            // 50%的效果已激活
            if (activated)
            {
                // 所使用卡牌为伤害卡
                if (BattleDataManager.instance.lastUsedCard != null && BattleDataManager.instance.lastUsedCard.CheckIfDamageCard())
                {
                    // 收回效果
                    GlobalValue.damageIncrement_General -= increment;
                    activated = false;
                }
                else
                {
                    // 非伤害卡牌 则跳过
                    return;
                }

                return;
            }
            // 增伤未激活
            else
            {
                counter--;

                // 计数器以达到预定值
                if(counter == 0)
                {
                    // 复位
                    counter = counter_Origin;
                    GlobalValue.damageIncrement_General += increment;
                    activated = true;
                }


                buffGUIScript.UpdateCounter(counter);
            }
        }
    }

    private void OnEnable()
    {
        activated = false;
        counter = counter_Origin;
        cur_UsedCard = BattleDataManager.instance.totalUsedCard;

        Invoke("EnableCounter",Time.deltaTime);

        Invoke("FirstUpdateCounter", Time.deltaTime);
    }

    void FirstUpdateCounter()
    {
        buffGUIScript.UpdateCounter(counter);
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
            GlobalValue.damageIncrement_General -= increment;
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
