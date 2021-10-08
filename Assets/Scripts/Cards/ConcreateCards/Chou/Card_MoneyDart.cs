using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * *************注意**************** 
 * 此卡牌较为特殊，由于包含2个主要参数
 * 故Duration当作第二个主要参数使用
 */

public class Card_MoneyDart : CardPrototype,ICardOperation,ICardEffectTrigger
{
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
        // 当未检测到目标或因其他原因失效时 返回位置
        CardManager.instance.ReflashLayoutGroup();
        SetOnSelected(false);
    }

    public void mouseUp()
    {
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
        _go.Hurt(GlobalValue.GetTruePhysicsDamage_ToEnemy(GlobalValue.GetTruePhysicsDamage_ToEnemy(cardInfo.mainValue_Cur, cardInfo.cost)), false, 1.0f);
        BattleDataManager.instance.UpdateTargetEnemy(_go);

        // 进行散射伤害 不记录单体攻击对象


    }

    public void TriggerEffect(GameObjectBase[] _gos)
    {
        throw new System.NotImplementedException();
    }
}
