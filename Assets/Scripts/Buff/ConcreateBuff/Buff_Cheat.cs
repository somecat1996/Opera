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

    // 保存已卡牌信息以及他的费值减少量
    Dictionary<CardPrototype, int> dictionary = new Dictionary<CardPrototype, int>();

    private void Update()
    {
        // 检测到使用卡牌数已更新
        if (cur_UsedCard != BattleDataManager.instance.totalUsedCard)
        {
            cur_UsedCard = BattleDataManager.instance.totalUsedCard;
            counter--;

            // 计数器以达到预定值
            if (counter == 0)
            {
                 // 复位
                 counter = counter_Origin;
                 activated = true;

                // 随机抽取手上卡牌
                List<CardPrototype> tempList = new List<CardPrototype>();
                foreach(var i in CardManager.instance.GetAllUsableCard())
                {
                    tempList.Add(i.GetComponent<CardPrototype>());
                }

                // 排除0费卡牌
                for(int i = 0; i < tempList.Count; i++)
                {
                    if(tempList[i].cardInfo.cost == 0)
                    {
                        tempList.RemoveAt(i);
                    }
                }

                // 确定剩余减费卡牌数量
                if(tempList.Count == 0)
                {
                    
                }
                else
                {
                    int index = Random.Range(0, tempList.Count);

                    tempList[index].cardInfo.cost--;
                    tempList[index].UpdateGUIInfo();

                    // 记录被减费卡牌及其减少量
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

        // 检测减费卡牌是否已经被使用
        if(lastUsesdCard != BattleDataManager.instance.lastUsedCard)
        {
            lastUsesdCard = BattleDataManager.instance.lastUsedCard;

            // 上一张使用的卡牌是被减费的卡牌 返还正常费值
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

            // 返还所有在记录中卡牌的费值
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
