using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_UmbrellaSword : CardPrototype, ICardEffectTrigger,ICardOperation
{
    public void mouseDrag()
    {
        transform.position = Input.mousePosition;

        SetFadeOutAndShowTargetMarker(true);
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
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Enemy")
        {
            if (PlayerManager.instance.ChangePowerPoint(-cardInfo.cost))
            {
                StartCoroutine(Delay(1, hit.transform.GetComponent<GameObjectBase>()));
                CardManager.instance.SendToDiscardedCardGroup(gameObject);
                return;
            }
            else
            {
                mouseExit();
            }
        }
        else
            mouseExit();
    }

    public IEnumerator Delay(float _time,GameObjectBase _go)
    {
        yield return new WaitForSeconds(_time);

        if(_go)
            TriggerEffect(_go);
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
        _go.Hurt(GlobalValue.GetTruePhysicsDamage_ToEnemy(cardInfo.mainValue_Cur, cardInfo.cost), false, 1.0f);
        BattleDataManager.instance.UpdateTargetEnemy(_go);
    }

    public void TriggerEffect(GameObjectBase[] _gos)
    {
        throw new System.NotImplementedException();
    }
}
