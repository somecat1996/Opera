using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Joke : CardPrototype,ICardOperation,ICardEffectTrigger
{
    public int effectCount = 2;

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

        if (Physics.Raycast(ray, out hit))
        {
            if (PlayerManager.instance.ChangePowerPoint(-cardInfo.cost))
            {
                List<GameObjectBase> temp = new List<GameObjectBase>();
                foreach (var i in Physics.SphereCastAll(ray, cardInfo.radius))
                {
                    if (i.transform.tag == "Enemy")
                    {
                        temp.Add(i.transform.GetComponent<GameObjectBase>());
                    }
                }

                // 注意 此卡牌特殊 升级是升级持续时间 固主要参数为duration

                while (temp.Count > effectCount)
                {
                    temp.RemoveAt(Random.Range(0, temp.Count));
                }

                TriggerEffect(temp.ToArray());
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
