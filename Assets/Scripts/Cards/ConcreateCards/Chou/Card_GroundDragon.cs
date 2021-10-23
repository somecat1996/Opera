using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_GroundDragon : CardPrototype,ICardOperation,ICardEffectTrigger
{
    public bool activated_5s = false;
    public bool activated_6s = false;

    public Coroutine timer_5s;
    public Coroutine timer_6s;

    public float mv_Crappy;
    public float mv_ShakeShoulder;
    public float mv_CrotchLift;

    private void Start()
    {
        UpdateGUIInfo();

        // 该卡牌的主要参数暂时用于效果倍乘
        mv_Crappy = CardManager.instance.cardLibrary[53].mainValue_Cur * cardInfo.mainValue_Cur;
        mv_ShakeShoulder = CardManager.instance.cardLibrary[54].mainValue_Cur * cardInfo.mainValue_Cur;
        mv_CrotchLift = CardManager.instance.cardLibrary[55].mainValue_Cur * cardInfo.mainValue_Cur;
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
        // 当未检测到目标或因其他原因失效时 返回位置
        CardManager.instance.ReflashLayoutGroup();
        SetOnSelected(false);
    }

    public void mouseUp()
    {
        if (!CheckAvaliablity())
        {
            GUIManager.instance.SpawnSystemText("无法释放卡牌!");
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
        GlobalValue.damageIncrement_General -= cardInfo.mainValue_Cur;
        activated_5s = activated_6s = false;
    }

    public void TriggerEffect()
    {
        PlayAnimationAndSound();

        if (activated_5s)
        {
            StopCoroutine(timer_5s);
            timer_5s = StartCoroutine(Timer_5s());
        }
        else
        {
            activated_5s = true;
            timer_5s = StartCoroutine(Timer_5s());
        }

        if (activated_6s)
        {
            StopCoroutine(timer_6s);
            timer_6s = StartCoroutine(Timer_6s());
        }
        else
        {
            activated_6s = true;
            timer_6s = StartCoroutine(Timer_6s());
        }


    }

    IEnumerator Timer_5s()
    {
        int time = 5;

        while(time > 0)
        {
            PlayerManager.instance.ChangeHealthPoint(mv_Crappy);
            time--;
            yield return new WaitForSeconds(1);
        }

        activated_5s = false;
    }

    IEnumerator Timer_6s()
    {
        float ppIncrement = PlayerManager.instance.cur_RecoverySpeed_PowerPoint * mv_ShakeShoulder;
        PlayerManager.instance.ChangeRecoverySpeed_PowerPoint(ppIncrement);
        GlobalValue.damageIncrement_General += mv_CrotchLift;

        yield return new WaitForSeconds(6);

        activated_6s = false;
        PlayerManager.instance.ChangeRecoverySpeed_PowerPoint(-ppIncrement);
        GlobalValue.damageIncrement_General -= mv_CrotchLift;
    }

    private void OnDisable()
    {
        if (activated_5s)
        {
            StopCoroutine(timer_5s);
            activated_5s = false;
        }

        if (activated_6s)
        {
            StopCoroutine(timer_6s);
            activated_6s = false;

            float ppIncrement = PlayerManager.instance.cur_RecoverySpeed_PowerPoint * mv_ShakeShoulder;
            PlayerManager.instance.ChangeRecoverySpeed_PowerPoint(-ppIncrement);
            GlobalValue.damageIncrement_General -= mv_CrotchLift;
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
