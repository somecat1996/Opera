using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    [Header("Configuration")]
    public int max_total_card = 10; // 牌库最大可持有卡牌数
    public int max_cur_card = 6; // 当前最大可使用卡牌数

    [Header("Real-Time Setting")]
    public int total_card; // 当前卡牌数
    public int cur_card; // 当前可操作卡牌数

    public Queue<GameObject> cardQueue = new Queue<GameObject>(); // 未上场卡牌队列

    [Header("Objects")]
    public Transform layoutGroup; // 当前操作卡牌容器
    public RectTransform recTran_layoutGroup;
    public Transform tempLayoutGroup; // 临时卡牌容器

    [Header("Temp")]
    public GameObject card_Attack;
    public Dictionary<int, CardBasicInfomation> dic = new Dictionary<int, CardBasicInfomation>();

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // ***动态读取测试***
        int j = 10;
        foreach (var i in Resources.LoadAll<CardBasicInfomation>("CardInfomation")) {
            Debug.Log(i.id);
            i.level = j;
            j += j;
        }

        // 测试用
        // 正式开始游戏前 需要将所选卡牌进行压入队列操作
        // 后在将其放入操作栏
        for (int i = 0; i < 10; i++)
        {
            GameObject go = GameObject.Instantiate<GameObject>(card_Attack);
            go.transform.parent = tempLayoutGroup;
            go.name = "card_Attack_"+i;
            go.transform.localScale = Vector3.one;
            cardQueue.Enqueue(go);

            total_card++;
        }

        while(cardQueue.Count!=0 && cur_card != max_cur_card)
        {
            SendToLayoutGroup();
        }
    }

    void Update()
    {
        
    }

    // 将等待队列中的卡牌送入游戏画面
    public void SendToLayoutGroup()
    {
        if (cardQueue.Count != 0)
        {
            GameObject go = cardQueue.Dequeue();
            go.transform.parent = layoutGroup;
            cur_card++;
        }
    }
    // 将使用过的卡牌送出画面并放入等待队列
    public void SendToTempGroup(GameObject _card)
    {
        _card.transform.parent = tempLayoutGroup;
        cardQueue.Enqueue(_card);

        cur_card--;

        SendToLayoutGroup();
    }

    public void ReflashLayoutGroup()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(recTran_layoutGroup);
    }
}
