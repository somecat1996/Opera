using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_ZhaoJun : CardPrototype,ICardOperation,ICardEffectTrigger
{
    public bool activated = false;
    public List<CardPrototype> cardList = new List<CardPrototype>();

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

    private void OnDisable()
    {
        RevokeEffect();
    }

    private void OnDestroy()
    {
        RevokeEffect();
    }

    public void mouseUp()
    {
        if (CheckOnValidArea())
        {
            if (PlayerManager.instance.ChangePowerPoint(-cardInfo.cost))
            {
                TriggerEffect();
                // �����Ʒ��벻���ö���
                CardManager.instance.SendToDisabledCardGroup(gameObject);
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
        if (activated)
        {
            activated = false;

            foreach(var i in cardList)
            {
                i.cardInfo.cost++;
                i.UpdateGUIInfo();
            }
        }
    }

    public void TriggerEffect()
    {
        if (!activated)
        {
            activated = true;
            // ��ȡ��������Ŀ�����Ϣ
            foreach(var i in CardManager.instance.GetAllUsableCard())
            {
                if(i.GetComponent<CardPrototype>().cardInfo != cardInfo)
                    cardList.Add(i.GetComponent<CardPrototype>());
            }

            while(cardList.Count != 3)
            {
                cardList.RemoveAt(Random.Range(0, cardList.Count));
            }

            foreach(var i in cardList)
            {
                i.cardInfo.cost--;
                i.UpdateGUIInfo();
            }
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
