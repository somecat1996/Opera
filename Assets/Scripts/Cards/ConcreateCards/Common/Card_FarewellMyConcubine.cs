using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_FarewellMyConcubine : CardPrototype,ICardOperation,ICardEffectTrigger
{
    public bool activated = false;
    Coroutine timer;

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
        CardManager.instance.ReflashLayoutGroup();
        SetOnSelected(false);
    }

    public void mouseUp()
    {
        if (!CheckAvaliablity())
        {
            GUIManager.instance.SpawnSystemText("ÎÞ·¨ÊÍ·Å¿¨ÅÆ!");
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

    public void RevokeEffect()
    {
        GlobalValue.damageIncrement_General -= cardInfo.mainValue_Cur;
    }

    public void TriggerEffect()
    {
        if (!activated)
        {
            timer = StartCoroutine(Timer());
            activated = true;
            GlobalValue.damageIncrement_General += cardInfo.mainValue_Cur;
        }
        else
        {
            StopCoroutine(timer);
            timer = StartCoroutine(Timer());
        }
    }

    public IEnumerator Timer()
    {
        float count = cardInfo.duration;
        while(count-- > 0)
        {
            if((PlayerManager.instance.cur_HealthPoint / PlayerManager.instance.max_HealthPoint) > 0.2f)
                PlayerManager.instance.ChangeHealthPoint(-10);

            yield return new WaitForSeconds(1);
        }

        activated = false;
        RevokeEffect();

    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
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
