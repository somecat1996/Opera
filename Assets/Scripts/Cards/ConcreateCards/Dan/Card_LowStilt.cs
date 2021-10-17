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
    
    // 定时器
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
