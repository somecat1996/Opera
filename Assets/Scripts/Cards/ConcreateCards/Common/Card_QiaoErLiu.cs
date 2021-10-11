using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_QiaoErLiu : CardPrototype,ICardOperation,ICardEffectTrigger
{
    /*
     *      此卡牌MainValue为增伤 radius为回复量
     */

    int totalCard = 0;
    public int counter_Origin = 5;
    public int counter;
    public bool activated = false;

    public CardPrototype curCard = null;

    public bool damageIncreasing = false;
    private void Update()
    {
        if (activated)
        {
            // 检测当前选择的卡牌 用于触发伤害提升
            if(curCard != BattleDataManager.instance.selectingCard)
            {
                curCard = BattleDataManager.instance.selectingCard;

                // 奇数
                if(cardInfo.cost % 2 == 1)
                {
                    damageIncreasing = true;
                    GlobalValue.damageIncrement_General += cardInfo.mainValue_Cur;
                }
                // 偶数不做回复判断
                else
                {
                    damageIncreasing = false;
                    GlobalValue.damageIncrement_General -= cardInfo.mainValue_Cur;
                }
            }

            // 检测上次选择的卡牌 用于触发回复心流值 及 卡牌计数器判断
            if(totalCard != BattleDataManager.instance.totalUsedCard)
            {
                totalCard = BattleDataManager.instance.totalUsedCard;
                totalCard--;

                // 奇数 收回伤害增益
                if(BattleDataManager.instance.lastUsedCard.cardInfo.cost % 2 == 1)
                {
                    if (damageIncreasing)
                    {
                        damageIncreasing = false;
                        GlobalValue.damageIncrement_General -= cardInfo.mainValue_Cur;
                    }
                }
                // 偶数 触发回复
                else
                {
                    PlayerManager.instance.ChangePowerPoint(cardInfo.radius);
                }

                counter--;

                if(counter == 0)
                {
                    activated = false;
                }
            }

        }
    }

    public void mouseDrag()
    {
        transform.position = Input.mousePosition;
    }

    public void mouseEnter()
    {
        SetOnSelected(true);
    }

    public void mouseExit()
    {
        // 当未检测到目标或因其他原因失效时 返回位置
        CardManager.instance.ReflashLayoutGroup();
        SetOnSelected(false);
    }

    public void mouseUp()
    {
        if (CheckOnValidArea())
        {
            if (PlayerManager.instance.ChangePowerPoint(-cardInfo.cost))
            {
                TriggerEffect();
                CardManager.instance.SendToDiscardedCardGroup(gameObject);
            }
            else
            {
                mouseExit();
            }

        }
        else
            mouseExit();
    }

    public void mouseDown()
    {
        GUIManager.instance.DisableCardDesc();
    }

    public void RevokeEffect()
    {
        throw new System.NotImplementedException();
    }

    public void TriggerEffect()
    {
        if (activated)
        {
            counter = counter_Origin;
            totalCard = BattleDataManager.instance.totalUsedCard;
            damageIncreasing = false;
        }
        else
        {
            activated = true;
            counter = counter_Origin;
            totalCard = BattleDataManager.instance.totalUsedCard;
            damageIncreasing = false;
        }
    }

    public void TriggerEffect(GameObjectBase _go)
    {
        throw new System.NotImplementedException();
    }

    public void TriggerEffect(GameObjectBase[] _gos)
    {
        throw new System.NotImplementedException();
    }
}
