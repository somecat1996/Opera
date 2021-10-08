using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_GuanKou : CardPrototype,ICardOperation,ICardEffectTrigger
{
    Ray tempRay;

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
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Enemy")
        {
            if (PlayerManager.instance.ChangePowerPoint(-cardInfo.cost))
            {
                tempRay = ray;

                TriggerEffect(hit.transform.GetComponent<GameObjectBase>());
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
        StartCoroutine(Timer(_go));
        BattleDataManager.instance.UpdateTargetEnemy(_go);
    }

    public void TriggerEffect(GameObjectBase[] _gos)
    {

    }

    public IEnumerator Timer(GameObjectBase _go)
    {
        int count = 7;

        while (count > 0)
        {
            _go.Hurt(GlobalValue.GetTruePhysicsDamage_ToEnemy(cardInfo.mainValue_Cur, cardInfo.cost), false, 1.0f);

            count--;
            yield return new WaitForSeconds(0.2f);
        }

    }
}
