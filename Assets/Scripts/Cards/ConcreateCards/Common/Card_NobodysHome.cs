using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_NobodysHome : CardPrototype,ICardOperation,ICardEffectTrigger
{
    public void mouseDrag()
    {
        transform.position = Input.mousePosition;
    }

    public void mouseEnter()
    {
        SetOnSelected(true);
    }

    public void mouseDown()
    {
        GUIManager.instance.DisableCardDesc();
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
                TriggerEffect(BattleDataManager.instance.enemyList.ToArray());
                CardManager.instance.SendToDiscardedCardGroup(gameObject);
            }
            else
            {
                mouseExit();
            }
        }
        mouseExit();
    }

    public void RevokeEffect()
    {
        throw new System.NotImplementedException();
    }

    public void TriggerEffect()
    {
        throw new System.NotImplementedException();
    }

    public void TriggerEffect(GameObjectBase _go)
    {
        throw new System.NotImplementedException();
    }

    public void TriggerEffect(GameObjectBase[] _gos)
    {
        foreach (var i in _gos)
            i.Stun(cardInfo.mainValue_Cur);
    }
}
