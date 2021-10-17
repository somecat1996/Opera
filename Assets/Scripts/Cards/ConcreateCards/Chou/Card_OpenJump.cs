using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_OpenJump : CardPrototype,ICardOperation,ICardEffectTrigger
{
    public void mouseDrag()
    {
        transform.position = Input.mousePosition;

        SetFadeOutAndShowRange(true);
    }

    public void mouseEnter()
    {
        SetOnSelected(true);
    }

    public void mouseExit()
    {
        // ��δ��⵽Ŀ���������ԭ��ʧЧʱ ����λ��
        CardManager.instance.ReflashLayoutGroup();
        SetOnSelected(false);
    }

    public void mouseDown()
    {
        GUIManager.instance.DisableCardDesc();
    }

    public void mouseUp()
    {
        if (CheckAvaliablity())
        {
            GUIManager.instance.SpawnSystemText("�޷��ͷſ���!");
            mouseExit();
            return;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (CheckOnValidArea() && Physics.Raycast(ray, out hit, 1000, CardManager.instance.groundLayer))
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
            i.Hurt(GlobalValue.GetTruePhysicsDamage_ToEnemy(cardInfo.mainValue_Cur, cardInfo.cost), false, 1.0f);
    }
}
