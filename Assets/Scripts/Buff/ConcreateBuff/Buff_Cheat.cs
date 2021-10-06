using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Cheat : BuffPrototype
{
    private int counter_Origin = 3;
    public int counter;
    public float decrement = 1f;

    private int cur_UsedCard;

    // �����ѿ�����Ϣ�Լ����ķ�ֵ������
    Dictionary<CardPrototype, int> dictionary = new Dictionary<CardPrototype, int>();

    private void Update()
    {
        // ��⵽ʹ�ÿ������Ѹ���
        if (cur_UsedCard != BattleDataManager.instance.totalUsedCard)
        {
            cur_UsedCard++;

            // 50%��Ч���Ѽ���
            if (activated)
            {
                // ��ʹ�ÿ���Ϊ�˺���
                if (BattleDataManager.instance.lastUsedCard.CheckIfDamageCard())
                {
                    // �ջ�Ч��
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
                if (counter == 0)
                {
                    // ��λ
                    counter = counter_Origin;
                    activated = true;
                }
            }
        }
    }

    private void OnEnable()
    {
        activated = true;
        counter = counter_Origin;
        cur_UsedCard = BattleDataManager.instance.totalUsedCard;
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
        }
    }
}
