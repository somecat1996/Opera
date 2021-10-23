using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Debut : CardPrototype, ICardEffectTrigger,ICardOperation
{
    public bool executing = false;
    public float time = 0;
    Ray tempRay;

    public void mouseDown()
    {
        GUIManager.instance.DisableCardDesc();
    }

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

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (CheckOnValidArea() && Physics.Raycast(ray, out hit, CardManager.instance.groundLayer))
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

    }

    public void TriggerEffect(GameObjectBase _go)
    {
        throw new System.NotImplementedException();
    }

    public void TriggerEffect(GameObjectBase[] _gos)
    {
        PlayAnimationAndSound();

        if (!executing)
        {
            executing = true;
            time = cardInfo.duration;
            StartCoroutine(Timer(_gos));
        }
        else
        {
            time = cardInfo.duration;
        }
    }

    public IEnumerator Timer(GameObjectBase[] _gos)
    {
        List<GameObjectBase> temp = new List<GameObjectBase>(_gos);

        while(time > 0)
        {
            time--;

            temp.Clear();
            foreach (var i in Physics.SphereCastAll(tempRay, cardInfo.radius))
            {
                if (i.transform.tag == "Enemy")
                {
                    temp.Add(i.transform.GetComponent<GameObjectBase>());
                }
            }

            foreach (var i in temp)
                i.Hurt(GlobalValue.GetTruePhysicsDamage_ToEnemy(cardInfo.mainValue_Cur, cardInfo.cost), false, 1);
            yield return new WaitForSeconds(1.0f);
        }

        executing = false;
    }
}
