using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_LowStilt : CardPrototype,ICardEffectTrigger,ICardOperation
{
    public bool activated = false;
    public float increment = 0;
    Coroutine timer;

    public void mouseDrag()
    {
        if (!CheckAvaliablity())
        {
            mouseExit();
            return;
        }

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
        if (!CheckAvaliablity())
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
        throw new System.NotImplementedException();
    }

    public void TriggerEffect()
    {
        increment = PlayerManager.instance.cur_RecoverySpeed_PowerPoint * cardInfo.mainValue_Cur;

        if (!activated)
        {
            activated = true;
            PlayerManager.instance.ChangeRecoverySpeed_PowerPoint(increment);
            timer = StartCoroutine(Timer());
        }
        else
        {
            StopCoroutine(timer);
            timer = StartCoroutine(Timer());
        }
    }
    
    // ��ʱ��
    public IEnumerator Timer()
    {
        yield return new WaitForSeconds(cardInfo.duration);

        PlayerManager.instance.ChangeRecoverySpeed_PowerPoint(-increment);
        activated = false;
    }

    private void OnDisable()
    {
        if (activated)
        {
            StopCoroutine(timer);
            activated = false;
            PlayerManager.instance.ChangeRecoverySpeed_PowerPoint(-increment);
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
