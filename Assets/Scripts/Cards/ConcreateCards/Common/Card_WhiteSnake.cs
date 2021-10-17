using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_WhiteSnake : CardPrototype, ICardOperation, ICardEffectTrigger
{
    public int counter = 0;
    public bool activated = false;

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

    public void RevokeEffect()
    {

        
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;

            GlobalValue.extraDamage -= counter * cardInfo.mainValue_Cur;
        }
    }
    private void OnDestroy()
    {
        OnDisable();
    }

    public void TriggerEffect()
    {
        counter++;
        GlobalValue.extraDamage += cardInfo.mainValue_Cur;

        if (!activated)
            activated = true;
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

