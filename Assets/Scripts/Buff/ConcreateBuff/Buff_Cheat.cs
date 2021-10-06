using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Cheat : BuffPrototype
{
    private int counter_Origin = 3;
    public int counter;
    public float decrement = 1f;

    private int cur_UsedCard;

    // 保存已卡牌信息以及他的费值减少量
    Dictionary<CardPrototype, int> dictionary = new Dictionary<CardPrototype, int>();

    private void Update()
    {
        // 检测到使用卡牌数已更新
        if (cur_UsedCard != BattleDataManager.instance.totalUsedCard)
        {
            cur_UsedCard++;

            // 50%的效果已激活
            if (activated)
            {
                // 所使用卡牌为伤害卡
                if (BattleDataManager.instance.lastUsedCard.CheckIfDamageCard())
                {
                    // 收回效果
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
                if (counter == 0)
                {
                    // 复位
                    counter = counter_Origin;
                    activated = true;
                }
            }
        }
    }

    private void OnEnable()
    {
        activated = true;
        counter = counter_Origin;
        cur_UsedCard = BattleDataManager.instance.totalUsedCard;
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
        }
    }
}
