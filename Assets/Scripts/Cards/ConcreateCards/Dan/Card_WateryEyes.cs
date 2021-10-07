using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_WateryEyes : CardPrototype,ICardOperation,ICardEffectTrigger
{
    float time;
    bool executing = false;
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

        if (Physics.Raycast(ray, out hit))
        {
            if (PlayerManager.instance.ChangePowerPoint(-cardInfo.cost))
            {
                tempRay = ray;

                List<GameObjectBase> temp = new List<GameObjectBase>();
                foreach (var i in Physics.SphereCastAll(ray, cardInfo.radius))
                {
                    if (i.transform.tag == "Enemy")
                    {
                        temp.Add(i.transform.GetComponent<GameObjectBase>());
                    }
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
        if (!executing)
        {
            executing = true;
            time = cardInfo.duration;
            StartCoroutine(Timer(_gos));
        }
        else
        {
            time = cardInfo.duration;
        }
        
    }

    public IEnumerator Timer(GameObjectBase[] _gos)
    {
        List<GameObjectBase> temp = new List<GameObjectBase>(_gos);

        while(time > 0)
        {
            temp.Clear();
            foreach (var i in Physics.SphereCastAll(tempRay, cardInfo.radius))
            {
                if (i.transform.tag == "Enemy")
                {
                    temp.Add(i.transform.GetComponent<GameObjectBase>());
                }
            }

            foreach (var i in temp)
            {
                if (i.IsStun())
                {
                    i.Hurt(GlobalValue.GetTrueMagicDamage_ToEnemy(cardInfo.mainValue_Cur, cardInfo.cost) * 2, false, 1.0f);
                }
                else
                {
                    i.Hurt(GlobalValue.GetTrueMagicDamage_ToEnemy(cardInfo.mainValue_Cur, cardInfo.cost), false, 1.0f);
                }
                
            }

            time--;
            yield return new WaitForSeconds(1);
        }

        executing = false;
    }
}
