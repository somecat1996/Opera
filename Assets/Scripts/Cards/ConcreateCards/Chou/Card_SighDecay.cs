using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_SighDecay : CardPrototype,ICardOperation,ICardEffectTrigger
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
        GlobalValue.poisonAttack = false;
    }

    public void TriggerEffect()
    {
        // Ч�����ڼ����� ���¼���
        if (activated)
        {
            OnDisable();
        }

        EffectsManager.instance.CreateEffectFollowPlayer(17, cardInfo.mainValue_Cur, new Vector3(0, 0, 2));
        PlayAnimationAndSound();
        GlobalValue.poisonAttack = true;
        activated = true;

        StartCoroutine(Timer());


    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(cardInfo.mainValue_Cur);
        RevokeEffect();
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
            if(timer != null)
                StopCoroutine(timer);
            RevokeEffect();
        }
    }
    private void OnDestroy()
    {
        OnDisable();
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
