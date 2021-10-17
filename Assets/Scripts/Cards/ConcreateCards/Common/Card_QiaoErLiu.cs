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
                if(curCard.cardInfo.cost % 2 == 1)
                {
                    if (!damageIncreasing)
                    {
                        damageIncreasing = true;
                        GlobalValue.damageIncrement_General += cardInfo.mainValue_Cur;
                        GUIManager.instance.EnableCardDesc(curCard.cardInfo, curCard.transform.GetComponent<RectTransform>().position);
                    }
                }
                // ż�������ظ��ж�
                else
                {
                    if (damageIncreasing)
                    {
                        damageIncreasing = false;
                        GlobalValue.damageIncrement_General -= cardInfo.mainValue_Cur;
                    }
                }
            }

            // ����ϴ�ѡ��Ŀ��� ���ڴ����ظ�����ֵ �� ���Ƽ������ж�
            if(totalCard != BattleDataManager.instance.totalUsedCard)
            {
                totalCard = BattleDataManager.instance.totalUsedCard;
                counter--;

                // ����
                if(BattleDataManager.instance.lastUsedCard.cardInfo.cost % 2 == 1)
                {

                }
                // ż�� �����ظ�
                else
                {
                    PlayerManager.instance.ChangePowerPoint(cardInfo.radius);
                }

                if(counter == 0)
                {
                    activated = false;

                    if (damageIncreasing)
                    {
                        damageIncreasing = false;
                        GlobalValue.damageIncrement_General -= cardInfo.mainValue_Cur;
                    }
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
        if (!CheckAvaliablity())
        {
            GUIManager.instance.SpawnSystemText("�޷��ͷſ���!");
            mouseExit();
            return;
        }

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
            counter = counter_Origin + 1;
            totalCard = BattleDataManager.instance.totalUsedCard;
            damageIncreasing = false;
        }
        else
        {
            counter = counter_Origin + 1;
            totalCard = BattleDataManager.instance.totalUsedCard;
            damageIncreasing = false;

            activated = true;
        }
    }

    private void OnDisable()
    {
        if (damageIncreasing)
        {
            GlobalValue.damageIncrement_General -= cardInfo.mainValue_Cur;
            damageIncreasing = false;
        }

    }

    private void OnDestroy()
    {
        
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
