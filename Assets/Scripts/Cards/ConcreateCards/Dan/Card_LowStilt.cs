using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_LowStilt : CardPrototype,ICardEffectTrigger,ICardOperation
{
    public bool recovering = false;
    public float rate = 0.5f;
    public float increment = 0;
    public float time = 0;

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
        if (!recovering)
        {
            recovering = true;
            time = cardInfo.duration;
            increment = PlayerManager.instance.cur_RecoverySpeed_PowerPoint * rate;
            PlayerManager.instance.ChangeRecoverySpeed_PowerPoint(increment);
            StartCoroutine(Timer());
        }
        else
        {
            time = cardInfo.duration;
        }
    }
    
    // 用于周期检测
    public IEnumerator Timer()
    {
        while(time > 0)
        {
            time--;
            yield return new WaitForSeconds(1);
        }
        PlayerManager.instance.ChangeRecoverySpeed_PowerPoint(-increment);
        recovering = false;
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
