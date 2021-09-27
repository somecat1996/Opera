using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    一.建立新的卡牌顺序
        1.创建卡牌基本信息
        2.创建实体卡牌脚本并手动配置卡牌信息
        3.具体实现卡牌各个功能
 */

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    [Header("Configuration")]
    public int max_Total_Card = 15; // 牌库最大可持有卡牌数
    public int min_Total_Card = 15; // 牌库最小持有数量
    public int max_Cur_Card = 6; // 当前最大可使用卡牌数

    [Header("Real-Time Setting")]
    public int total_Card; // 当前卡牌数
    public int cur_Card; // 当前可操作卡牌数

    public List<GameObject> cardQueue = new List<GameObject>(); // 未上场卡牌队列
    public List<GameObject> cardQueue_Discarded= new List<GameObject>(); // 已使用的卡牌
    public Dictionary<int, CardBasicInfomation> cardLibrary = new Dictionary<int, CardBasicInfomation>(); // 卡牌库 存放所有与角色相关的卡牌
    public Dictionary<int, CardBasicInfomation> cardLibrary_Extra = new Dictionary<int, CardBasicInfomation>(); // 额外卡牌库
    public Dictionary<int, GameObject> instanceCardLibrary = new Dictionary<int, GameObject>(); // 卡牌信息对应实例卡牌
    public List<CardBasicInfomation> cardLibrary_Selected = new List<CardBasicInfomation>(); // 玩家选择卡牌

    [Header("Objects")]
    public CardCommonDatas cardCommonData;

    public Transform layoutGroup; // 当前操作卡牌容器
    public RectTransform recTran_layoutGroup;
    public Transform tempLayoutGroup; // 临时卡牌容器

    [Header("Temp")]
    public GameObject card_Attack;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

        // 卡牌列表库测试
        LoadCardLibrary();
        LoadExtraCardLibrary();
        LoadCardInstance();

        // 初始化解锁角色所有卡牌 等级设定为 1 同时锁定所有额外卡牌
        // InitializeCardLibrary();

        GUIManager.instance.ClearCardList();
        foreach(var i in cardLibrary.Values)
        {
            GUIManager.instance.LoadCardIntoList(i);
        }

        // 测试用
        // 正式开始游戏前 需要将所选卡牌进行压入队列操作
        // 后在将其放入操作栏
        for (int i = 0; i < 5; i++)
        {
            GameObject go = GameObject.Instantiate<GameObject>(card_Attack);
            go.transform.parent = tempLayoutGroup;
            go.name = "card_Attack_" + i;
            go.transform.localScale = Vector3.one;
            cardQueue.Add(go);

            total_Card++;
        }

        // 测试增加新卡牌 **** 暂时没有限定卡牌携带数量 *****
        for (int i = 1; i <= 15; i++)
        {
            GameObject go = Instantiate <GameObject>(instanceCardLibrary[i]);
            go.transform.parent = tempLayoutGroup;
            go.transform.localScale = Vector3.one;

            cardQueue.Add(go);
            total_Card++;
        }

        while (cardQueue.Count != 0 && cur_Card != max_Cur_Card)
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
        GameObject go = cardQueue[0];
        cardQueue.RemoveAt(0);
        go.transform.parent = layoutGroup;
        cur_Card++;
    }
    // 将使用过的卡牌送出画面并放入弃牌队列中
    public void SendToDiscardedCardGroup(GameObject _card)
    {
        _card.transform.parent = tempLayoutGroup;
        cardQueue_Discarded.Add(_card);

        cur_Card--;

        if (cardQueue.Count == 0)
        {
            // 可使用卡牌队列已空 打乱弃牌队列顺序并重新放入等待队列中
            while (cardQueue_Discarded.Count != 0)
            {
                int index = Random.Range(0, cardQueue_Discarded.Count);
                cardQueue.Add(cardQueue_Discarded[index]);
                cardQueue_Discarded.RemoveAt(index);
            }
        }

        SendToLayoutGroup();
    }

    public void ReflashLayoutGroup()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(recTran_layoutGroup);
    }

    // 根据不同角色读取对应角色卡牌库
    public void LoadCardLibrary()
    {
        // 目前暂时只载入旦的卡牌

        //... 检查玩家所选角色
        // if(xx == CharacterType.Character.Dan) ...

        foreach(var i in Resources.LoadAll<CardBasicInfomation>("CardInfomation/Dan"))
        {
            cardLibrary.Add(i.id, i);
        }
    }

    // 读取额外卡牌库
    public void LoadExtraCardLibrary()
    {

    }

    // 读取卡牌实例并放入字典中
    public void LoadCardInstance()
    {
        // 判断角色类型
        foreach (var i in Resources.LoadAll<GameObject>("CardInstances/Dan"))
        {
            instanceCardLibrary.Add(i.GetComponent<CardPrototype>().GetID(),i);
        }
    }

    // 重新排列并载入玩家所选择的卡牌-开始战斗场景时使用
    public void RealignAndLoadCards()
    {
        // 重新排列
        int count = cardLibrary_Selected.Count - 1;

        while (count != 0)
        {
            int index = Random.Range(0, count + 1);
            
            CardBasicInfomation temp = cardLibrary_Selected[count];
            cardLibrary_Selected[count] = cardLibrary_Selected[index];
            cardLibrary_Selected[index] = temp;

            count --;
        }

        // 所选卡牌进行压入等待队列
        // 后在将其放入操作栏
        for (int i = 0; i < cardLibrary_Selected.Count; i++)
        {
            GameObject go = instanceCardLibrary[cardLibrary_Selected[i].id];
            go.transform.parent = tempLayoutGroup;
            go.transform.localScale = Vector3.one;
            cardQueue.Add(go);

            total_Card++;
        }

        while (cardQueue.Count != 0 && cur_Card != max_Cur_Card)
        {
            SendToLayoutGroup();
        }
    }

    // 初始化所有卡牌-开始新游戏时调用 (读取后使用)
    public void InitializeCardLibrary()
    {
        // 初始化角色卡牌
        foreach(var i in cardLibrary.Values)
        {
            i.UnlockCard();
        }
        // 初始化额外卡牌
        foreach(var i in cardLibrary_Extra.Values)
        {
            i.LockCard();
        }
    }
}
