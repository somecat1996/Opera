using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_MoneyDart : CardPrototype,ICardOperation,ICardEffectTrigger
{
    public int firstObjectMultiple = 5;
    public int catapultCount = 3;
    public float interval = 0.1f;

    public void mouseDown()
    {
        GUIManager.instance.DisableCardDesc();


    }

    public void mouseDrag()
    {
        transform.position = Input.mousePosition;

        SetFadeOutAndShowTargetMarker(true);
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

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Enemy")
        {
            if (PlayerManager.instance.ChangePowerPoint(-cardInfo.cost))
            {
                TriggerEffect(hit.transform.GetComponent<GameObjectBase>());
                CardManager.instance.SendToDiscardedCardGroup(gameObject);
                return;
            }
        }

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
        List<GameObjectBase> attackableObjectList = new List<GameObjectBase>(BattleDataManager.instance.enemyList);
        attackableObjectList.Remove(_go); // �Ƴ���һ��Ŀ��


        // �׸�Ŀ��5���˺�
        _go.Hurt(GlobalValue.GetTruePhysicsDamage_ToEnemy(GlobalValue.GetTruePhysicsDamage_ToEnemy(cardInfo.mainValue_Cur * firstObjectMultiple, cardInfo.cost)), false, 1.0f);
        BattleDataManager.instance.UpdateTargetEnemy(_go);

        // ����ɢ���˺� ����¼���幥������
        StartCoroutine(Timer(_go,attackableObjectList));

    }

    public IEnumerator Timer(GameObjectBase _firstTarget,List<GameObjectBase> _attackableList)
    {
        int temp = catapultCount;
        Vector3 originPoint = _firstTarget.transform.position;

        while(temp-- > 0)
        {
            GameObjectBase newTarget = null;
            float minDis = Mathf.Infinity;
            // ��Ѱָ���뾶������Ŀ��
            foreach(var i in Physics.OverlapSphere(originPoint, cardInfo.radius))
            {
                // �ҵ����Ŀ��
                if (Vector3.Distance(originPoint, i.transform.position) < minDis && _attackableList.Contains(i.transform.GetComponent<GameObjectBase>()))
                {
                    newTarget = i.transform.GetComponent<GameObjectBase>();
                    minDis = Vector3.Distance(originPoint, i.transform.position);
                }
            }

            // ����Ŀ�� ��ֱ�ӷ���
            if(newTarget == null)
            {
                break;
            }
            else
            {
                // ����˺���ת�������
                newTarget.Hurt(GlobalValue.GetTruePhysicsDamage_ToEnemy(cardInfo.mainValue_Cur));
                _attackableList.Remove(newTarget);

                originPoint = newTarget.transform.position;
                newTarget = null;
            }

            yield return new WaitForSeconds(interval);
        }
    }

    public void TriggerEffect(GameObjectBase[] _gos)
    {
        throw new System.NotImplementedException();
    }
}
