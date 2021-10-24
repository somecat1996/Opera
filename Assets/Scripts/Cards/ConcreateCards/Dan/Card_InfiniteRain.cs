using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_InfiniteRain : CardPrototype,ICardEffectTrigger,ICardOperation
{
    public int alpha = 1;
    public float probability = 0.25f;
    public float time = 0;
    public bool executing = false;
    private Coroutine coroutine;
    private Ray tempRay;

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

        SetFadeOutAndShowRange(true);
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

        if (CheckOnValidArea() && Physics.Raycast(ray, out hit, 1000, CardManager.instance.groundLayer))
        {
            EffectsManager.instance.CreateEffect(5, cardInfo.duration, hit.point, Vector3.zero);
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
        {
            mouseExit();
        }
       
    }

    public void RevokeEffect()
    {
        throw new System.NotImplementedException();
    }

    public void TriggerEffect()
    {

    }

    public IEnumerator Timer(GameObjectBase[] _gos)
    {
        List<GameObjectBase> temp = new List<GameObjectBase>(_gos);

        while (time > 0)
        {
            time--;

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
                i.Hurt(GlobalValue.GetTrueMagicDamage_ToEnemy(cardInfo.mainValue_Cur, cardInfo.cost) * alpha,false,1, HurtType.Magic);

                if (Random.Range(0, 1.0f) <= probability)
                {
                    alpha++;
                }

                alpha = Mathf.Clamp(alpha, 1, 9);
            }



            yield return new WaitForSeconds(1);
        }

        executing = false;
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
            alpha = 1;

            coroutine = StartCoroutine(Timer(_gos));
        }
        else
        {
            time = cardInfo.duration;
            alpha = 1;

            StopCoroutine(coroutine);
            coroutine = StartCoroutine(Timer(_gos));
        }
    }
}
