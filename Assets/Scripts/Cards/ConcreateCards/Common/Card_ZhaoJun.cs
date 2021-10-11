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
        // 当未检测到目标或因其他原因失效时 返回位置
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
                // 将卡牌放入不可用队列
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
            // 获取除自身外的卡牌信息
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
