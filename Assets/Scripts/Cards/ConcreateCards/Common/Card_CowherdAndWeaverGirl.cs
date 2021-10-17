using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_CowherdAndWeaverGirl : CardPrototype,ICardOperation,ICardEffectTrigger
{
    public List<CardPrototype> cardList = new List<CardPrototype>();
    public CardPrototype copiedCard;

    private void Start()
    {
        UpdateGUIInfo();
    }

    public void mouseDown()
    {
        GUIManager.instance.DisableCardDesc();
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
        CardManager.instance.ReflashLayoutGroup();
        SetOnSelected(false);
    }

    public void mouseUp()
    {
        if (CheckAvaliablity())
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
            }
            else
            {
                mouseExit();
            }

        }
        else
            mouseExit();
    }

    public void RevokeEffect()
    {
        throw new System.NotImplementedException();
    }

    public void TriggerEffect()
    {
        foreach(var i in CardManager.instance.GetAllActivatedCard(false))
        {
            if(i.GetComponent<CardPrototype>().cardInfo.belongner == CharacterType.CharacterTag.Common && i.GetComponent<CardPrototype>().cardInfo != cardInfo)
                cardList.Add(i.GetComponent<CardPrototype>());
        }

        // ��Я������һ�����鿨 ��ֱ���Ƴ�
        if(cardList.Count == 0)
        {
            CardManager.instance.SendToDisabledCardGroup(gameObject);
        }
        else
        {
            copiedCard = cardList[Random.Range(0, cardList.Count)];
            copiedCard.GetComponent<ICardEffectTrigger>().TriggerEffect();
            Debug.Log(copiedCard.cardInfo.cardName);

            if (copiedCard.cardInfo.id != 109)
                CardManager.instance.SendToDiscardedCardGroup(gameObject);// �����Ʒ���ȴ�����
            else
                CardManager.instance.SendToDisabledCardGroup(gameObject);// ���벻���ö���
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
