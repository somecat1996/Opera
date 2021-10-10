using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_QiaoErLiu : CardPrototype,ICardOperation,ICardEffectTrigger
{
    /*
     *      �˿���MainValueΪ���� radiusΪ�ظ���
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
            // ��⵱ǰѡ��Ŀ��� ���ڴ����˺�����
            if(curCard != BattleDataManager.instance.selectingCard)
            {
                curCard = BattleDataManager.instance.selectingCard;

                // ����
                if(cardInfo.cost % 2 == 1)
                {
                    damageIncreasing = true;
                    GlobalValue.damageIncrement_General += cardInfo.mainValue_Cur;
                }
                // ż�������ظ��ж�
                else
                {
                    damageIncreasing = false;
                    GlobalValue.damageIncrement_General -= cardInfo.mainValue_Cur;
                }
            }

            // ����ϴ�ѡ��Ŀ��� ���ڴ����ظ�����ֵ �� ���Ƽ������ж�
            if(totalCard != BattleDataManager.instance.totalUsedCard)
            {
                totalCard = BattleDataManager.instance.totalUsedCard;
                totalCard--;

                // ���� �ջ��˺�����
                if(BattleDataManager.instance.lastUsedCard.cardInfo.cost % 2 == 1)
                {
                    if (damageIncreasing)
                    {
                        damageIncreasing = false;
                        GlobalValue.damageIncrement_General -= cardInfo.mainValue_Cur;
                    }
                }
                // ż�� �����ظ�
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
        // ��δ��⵽Ŀ���������ԭ��ʧЧʱ ����λ��
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
