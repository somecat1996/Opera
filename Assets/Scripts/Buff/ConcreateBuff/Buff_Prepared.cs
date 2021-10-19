using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Prepared : BuffPrototype
{
    private int counter_Origin = 5;
    public int counter;
    public float increment = 0.5f;

    private int cur_UsedCard;

    private void Update()
    {
        // ��⵽ʹ�ÿ������Ѹ���
        if(cur_UsedCard != BattleDataManager.instance.totalUsedCard)
        {
            cur_UsedCard++;

            // 50%��Ч���Ѽ���
            if (activated)
            {
                // ��ʹ�ÿ���Ϊ�˺���
                if (BattleDataManager.instance.lastUsedCard != null && BattleDataManager.instance.lastUsedCard.CheckIfDamageCard())
                {
                    // �ջ�Ч��
                    GlobalValue.damageIncrement_General -= increment;
                    activated = false;
                }
                else
                {
                    // ���˺����� ������
                    return;
                }

                return;
            }
            // ����δ����
            else
            {
                counter--;

                // �������ԴﵽԤ��ֵ
                if(counter == 0)
                {
                    // ��λ
                    counter = counter_Origin;
                    GlobalValue.damageIncrement_General += increment;
                    activated = true;
                }


                buffGUIScript.UpdateCounter(counter);
            }
        }
    }

    private void OnEnable()
    {
        activated = false;
        counter = counter_Origin;
        cur_UsedCard = BattleDataManager.instance.totalUsedCard;

        Invoke("EnableCounter",Time.deltaTime);

        Invoke("FirstUpdateCounter", Time.deltaTime);
    }

    void FirstUpdateCounter()
    {
        buffGUIScript.UpdateCounter(counter);
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
            GlobalValue.damageIncrement_General -= increment;
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
