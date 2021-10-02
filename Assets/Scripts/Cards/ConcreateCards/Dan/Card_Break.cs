using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Break : CardPrototype,ICardOperation,ICardEffectTrigger
{
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
        CardManager.instance.SendToDiscardedCardGroup(gameObject);
    }

    public void RevokeEffect()
    {
        throw new System.NotImplementedException();
    }

    public void TriggerEffect()
    {
        throw new System.NotImplementedException();
    }
}
