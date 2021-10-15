using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_SleeveBlade : CardPrototype,ICardOperation,ICardEffectTrigger
{
    public void mouseDrag()
    {
        transform.position = Input.mousePosition;

        SetFadeOutAndShowDirectionPointer(true);
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

    public void mouseDown()
    {
        GUIManager.instance.DisableCardDesc();
    }

    public void mouseUp()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (CheckOnValidArea() && Physics.Raycast(ray, out hit, 1000, CardManager.instance.groundLayer))
        {
            if (PlayerManager.instance.ChangePowerPoint(-cardInfo.cost))
            {
                Vector3 origin = PlayerManager.instance.player.transform.position;
                origin.y = 0;

                Vector3 dir = Vector3.Normalize(hit.point - origin);
                SummonedObjectManager.instance.SummonKnife(GlobalValue.GetTruePhysicsDamage_ToEnemy(cardInfo.mainValue_Cur), origin, dir);
                CardManager.instance.SendToDiscardedCardGroup(gameObject);
                TriggerEffect();
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
        
    }

    public void TriggerEffect(GameObjectBase _go)
    {
        throw new System.NotImplementedException();
    }

    public void TriggerEffect(GameObjectBase[] _gos)
    {

    }
}
