using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Break : CardPrototype,ICardOperation,ICardEffectTrigger
{
    public void mouseDown()
    {
        GUIManager.instance.DisableCardDesc();
    }

    public void mouseDrag()
    {
        if (!CheckAvaliablity())
        {
            mouseExit();
            return;
        }

        transform.position = Input.mousePosition;

        SetFadeOutAndShowTargetMarker(true);
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
        if (!CheckAvaliablity())
        {
            GUIManager.instance.SpawnSystemText("无法释放卡牌!");
            mouseExit();
            return;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Enemy")
        {
            if (PlayerManager.instance.ChangePowerPoint(-cardInfo.cost))
            {
                TriggerEffect(hit.transform.GetComponent<GameObjectBase>());
                CardManager.instance.SendToDiscardedCardGroup(gameObject);
                return;
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
        _go.Hurt(GlobalValue.GetTruePhysicsDamage_ToEnemy(cardInfo.mainValue_Cur,cardInfo.cost), true, 3);
        BattleDataManager.instance.UpdateTargetEnemy(_go);
    }

    public void TriggerEffect(GameObjectBase[] _gos)
    {
        throw new System.NotImplementedException();
    }
}
