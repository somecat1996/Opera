using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_UmbrellaSword : CardPrototype, ICardEffectTrigger,ICardOperation
{
    public void mouseDrag()
    {
        transform.position = Input.mousePosition;
    }

    public void mouseEnter()
    {
        Vector3 scale = new Vector3(1.2f, 1.2f, 1.2f);
        transform.localScale = scale;
    }

    public void mouseExit()
    {
        // 当未检测到目标或因其他原因失效时 返回位置
        CardManager.instance.ReflashLayoutGroup();
        Vector3 scale = Vector3.one;
        transform.localScale = scale;
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
