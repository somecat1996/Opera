using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_SpringSong :CardPrototype,ICardOperation,ICardEffectTrigger
{
    Ray tempRay;

    public void mouseDrag()
    {
        if (!CheckAvaliablity())
        {
            mouseExit();
            return;
        }

        transform.position = Input.mousePosition;

        SetFadeOutAndShowRange(true);
    }

    public void mouseEnter()
    {
        SetOnSelected(true);
    }

    public void mouseDown()
    {
        GUIManager.instance.DisableCardDesc();
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

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (CheckOnValidArea() && Physics.Raycast(ray, out hit, 1000, CardManager.instance.groundLayer))
        {
            if (PlayerManager.instance.ChangePowerPoint(-cardInfo.cost))
            {
                tempRay = ray;

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
        else
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
        PlayAnimationAndSound();
        StartCoroutine(Timer(_gos));
    }

    public IEnumerator Timer(GameObjectBase[] _gos)
    {
        List<GameObjectBase> temp = new List<GameObjectBase>(_gos);

        int count = 3;
        float damage = cardInfo.mainValue_Cur;

        while(count > 0)
        {
            // 更新受影响单位列表
            temp.Clear();
            foreach (var i in Physics.SphereCastAll(tempRay, cardInfo.radius))
            {
                if (i.transform.tag == "Enemy")
                {
                    temp.Add(i.transform.GetComponent<GameObjectBase>());
                }
            }

            foreach (var i in temp)
            {
                i.Hurt(GlobalValue.GetTrueMagicDamage_ToEnemy(damage, cardInfo.cost), false, 1.0f, HurtType.Magic);
            }

            damage *= 2;

            count--;
            yield return new WaitForSeconds(0.2f);
        }

    }
}
