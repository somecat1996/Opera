using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Cheat : BuffPrototype
{
    private int counter_Origin = 3;
    public int counter;
    public float decrement = 1f;

    private int cur_UsedCard;

    public CardPrototype lastUsesdCard = null;

    // �����ѿ�����Ϣ�Լ����ķ�ֵ������
    Dictionary<CardPrototype, int> dictionary = new Dictionary<CardPrototype, int>();

    private void Update()
    {
        // ��⵽ʹ�ÿ������Ѹ���
        if (cur_UsedCard != BattleDataManager.instance.totalUsedCard)
        {
            cur_UsedCard = BattleDataManager.instance.totalUsedCard;
            counter--;

            // �������ԴﵽԤ��ֵ
            if (counter == 0)
            {
                 // ��λ
                 counter = counter_Origin;
                 activated = true;

                // �����ȡ���Ͽ���
                List<CardPrototype> tempList = new List<CardPrototype>();
                foreach(var i in CardManager.instance.GetAllUsableCard())
                {
                    tempList.Add(i.GetComponent<CardPrototype>());
                }

                // �ų�0�ѿ���
                for(int i = 0; i < tempList.Count; i++)
                {
                    if(tempList[i].cardInfo.cost == 0)
                    {
                        tempList.RemoveAt(i);
                    }
                }

                // ȷ��ʣ����ѿ�������
                if(tempList.Count == 0)
                {
                    
                }
                else
                {
                    int index = Random.Range(0, tempList.Count);

                    tempList[index].cardInfo.cost--;
                    tempList[index].UpdateGUIInfo();

                    // ��¼�����ѿ��Ƽ��������
                    if (dictionary.ContainsKey(tempList[index]))
                    {
                        dictionary[tempList[index]]++;
                    }
                    else
                    {
                        dictionary.Add(tempList[index], 1);
                    }
                }
            }

            buffGUIScript.UpdateCounter(counter);
        }

        // �����ѿ����Ƿ��Ѿ���ʹ��
        if(lastUsesdCard != BattleDataManager.instance.lastUsedCard)
        {
            lastUsesdCard = BattleDataManager.instance.lastUsedCard;

            // ��һ��ʹ�õĿ����Ǳ����ѵĿ��� ����������ֵ
            if (dictionary.ContainsKey(lastUsesdCard))
            {
                lastUsesdCard.cardInfo.cost += dictionary[lastUsesdCard];
                dictionary.Remove(lastUsesdCard);
            }
        }
    }

    private void OnEnable()
    {
        Invoke("EnableCounter",Time.deltaTime);

        activated = true;
        counter = counter_Origin;
        cur_UsedCard = BattleDataManager.instance.totalUsedCard;

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

            // ���������ڼ�¼�п��Ƶķ�ֵ
            foreach(var i in dictionary)
            {
                i.Key.cardInfo.cost += i.Value;
            }

            dictionary.Clear();
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
