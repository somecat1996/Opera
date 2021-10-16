using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_RippleUmbrealla : CardPrototype,ICardOperation,ICardEffectTrigger
{
    public bool activated = false;
    public Coroutine timer;

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
        // ��δ��⵽Ŀ���������ԭ��ʧЧʱ ����λ��
        CardManager.instance.ReflashLayoutGroup();
        SetOnSelected(false);
    }

    public void mouseUp()
    {
        if (CardManager.instance.lockingCards || transform.parent != CardManager.instance.layoutGroup)
        {
            GUIManager.instance.SpawnSystemText("�޷��ͷſ���!");
            mouseExit();
            return;
        }

        if (CheckOnValidArea())
        {
            if (PlayerManager.instance.ChangePowerPoint(-cardInfo.cost))
            {
                TriggerEffect();
                CardManager.instance.SendToDiscardedCardGroup(gameObject);
            }
            else
            {
                mouseExit();
            }

        }
        else
            mouseExit();
    }

    public void mouseDown()
    {
        GUIManager.instance.DisableCardDesc();
    }
    public void RevokeEffect()
    {
        GlobalValue.damageIncrement_Magic -= cardInfo.mainValue_Cur;
        activated = false;
    }

    public void TriggerEffect()
    {
        // Ч�����ڼ����� ���¼���
        if (activated)
        {
            OnDisable();
        }

        GlobalValue.damageIncrement_Magic += cardInfo.mainValue_Cur;
        activated = true;

        StartCoroutine(Timer());


    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(cardInfo.duration);
        RevokeEffect();
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
            StopCoroutine(timer);
            RevokeEffect();
        }
    }

    public void TriggerEffect(GameObjectBase _go)
    {
        throw new System.NotImplementedException();
    }

    public void TriggerEffect(GameObjectBase[] _gos)
    {
        throw new System.NotImplementedException();
    }
}
