using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_RainyOrchid :CardPrototype,ICardOperation,ICardEffectTrigger
{
    public bool healing = false;
    public float time;
    Coroutine timer;

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
        if(healing == false)
        {
            timer = StartCoroutine(Effect());
            healing = true;
        }
        else
        {
            // 如果效果仍在 则刷新次数
            time = cardInfo.duration;
        }

    }

    private void OnDisable()
    {
        if (healing)
        {
            StopCoroutine(timer);
            healing = false;
        }
    }

    public IEnumerator Effect()
    {
        time = cardInfo.duration;

        while (time > 0)
        {
            time--;
            Debug.Log(time);
            PlayerManager.instance.ChangeHealthPoint(cardInfo.mainValue_Cur);
            yield return new WaitForSeconds(1);
        }

        healing = false;
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
