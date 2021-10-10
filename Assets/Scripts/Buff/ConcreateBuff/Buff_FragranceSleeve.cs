using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_FragranceSleeve : BuffPrototype
{
    public float probability = 0.2f;

    private CardPrototype cur_UsedCard;

    private void Update()
    {
        // 检测已经使用卡牌
        if (cur_UsedCard != BattleDataManager.instance.lastUsedCard)
        {
            cur_UsedCard = BattleDataManager.instance.lastUsedCard;

            if(cur_UsedCard.cardInfo.cardType == CardTag.Type.Magic)
            {
                if(Random.Range(0,1f) <= GlobalValue.GetTrueProbaility(probability))
                {
                    int cost = cur_UsedCard.cardInfo.cost;
                    PlayerManager.instance.ChangePowerPoint(cost);
                }
            }
        }
    }

    private void OnEnable()
    {
        if (!activated)
        {
            activated = true;
            cur_UsedCard = BattleDataManager.instance.lastUsedCard;
        }
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
