using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    [Header("Configuration")]
    public int max_Total_Card = 30; // 牌库最大可持有卡牌数
    public int min_Total_Card = 15; // 牌库最小持有数量
    public int max_Cur_Card = 6; // 当前最大可使用卡牌数

    [Header("Real-Time Setting")]
    public int total_Card; // 当前卡牌数
    public int cur_Card; // 当前可操作卡牌数

    public List<GameObject> cardQueue = new List<GameObject>(); // 未上场卡牌队列
    public List<GameObject> cardQueue_Discarded= new List<GameObject>(); // 已使用的卡牌

    public Dictionary<int, CardBasicInfomation> cardLibrary = new Dictionary<int, CardBasicInfomation>(); // 卡牌库 存放所有与角色相关的卡牌
    public Dictionary<int, CardBasicInfomation> cardLibrary_Common = new Dictionary<int, CardBasicInfomation>(); // 额外卡牌库
    public Dictionary<int, GameObject> instanceCardLibrary = new Dictionary<int, GameObject>(); // 卡牌信息对应实例卡牌

    public List<CardBasicInfomation> cardLibrary_Selected = new List<CardBasicInfomation>(); // 玩家选择的卡牌

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

        // 测试用——会与UI按钮控件相冲突 *****下列方法调用时 不要使用UI中的部分按钮*****

        LoadCardLibrary(); // 将指定角色卡牌载入到库

        //InitializeAllCards(); // 初始化解锁角色所有卡牌 等级设定为 1 同时锁定所有通用卡牌
        LoadCommonCardLibrary(); // 将通用卡牌载入到列表中以备使用
        LoadCardInstance(); // 将库中卡牌对应的实例载入到字典中以备使用
        LoadAllCardIntoUnselectedList(); // 将卡牌库的信息载入到选择列表中
        LoadAllCardIntoCardList();// 卡牌展示 测试

    }

    void Update()
    {
        // 测试用 生成战斗画面卡牌
        if (Input.GetMouseButtonDown(1))
        {
            //LoadSelectedCard();
            RealignAndLoadCards();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ClearAllCardQueue();
        }
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
    public void LoadCommonCardLibrary()
    {

    }

    // 读取卡牌实例并放入字典中以作备用
    public void LoadCardInstance()
    {
        // 判断角色类型
        foreach (var i in Resources.LoadAll<GameObject>("CardInstances/Dan"))
        {
            instanceCardLibrary.Add(i.GetComponent<CardPrototype>().GetID(),i);
        }
    }

    

    // 将卡牌库中的所有卡牌载入到卡牌展示界面中
    public void LoadAllCardIntoCardList()
    {
        GUIManager.instance.ClearCardList();
        foreach (var i in cardLibrary.Values)
        {
            GUIManager.instance.LoadCardIntoList(i);
        }

        // 通用卡牌
        foreach(var i in cardLibrary_Common.Values)
        {
            GUIManager.instance.LoadCardIntoList(i);
        }
    }

    // 重新排列并载入玩家所选择的卡牌——***开始战斗场景时使用***
    public void RealignAndLoadCards()
    {
        List<CardBasicInfomation> tempCardList = new List<CardBasicInfomation>();

        // 载入角色卡
        foreach(var i in cardLibrary.Values)
        {
            if(i.belongner == PlayerManager.instance.cur_Character)
            {
                tempCardList.Add(i);
            }
        }

        // 载入所选择的通用卡
        for(int i = 0; i < cardLibrary_Selected.Count; i++)
        {
            tempCardList.Add(cardLibrary[cardLibrary_Selected[i].id]);
        }

        // 重新排列
        int count = tempCardList.Count - 1;

        while (count != 0)
        {
            int index = Random.Range(0, count + 1);
            
            CardBasicInfomation temp = tempCardList[count];
            tempCardList[count] = tempCardList[index];
            tempCardList[index] = temp;

            count --;
        }

        // 所选卡牌进行压入等待队列
        // 后在将其放入操作栏
        for (int i = 0; i < tempCardList.Count; i++)
        {
            GameObject go = Instantiate(instanceCardLibrary[tempCardList[i].id]);
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
    public void InitializeAllCards()
    {
        // 初始化角色卡牌
        foreach(var i in cardLibrary.Values)
        {
            i.InitilizeCard();
        }
        // 初始化额外卡牌
        foreach(var i in cardLibrary_Common.Values)
        {
            i.LockCard();
        }
    }

    // 将所有卡牌载入到未选中容器中
    public void LoadAllCardIntoUnselectedList()
    {
        // 清除卡牌操作由PlayerManager执行 此处临时使用
       // GUIManager.instance.ClearUnselectedCardList();
        //ClearSelectedCard();

        // 应遍历通用卡牌库
        foreach (var i in cardLibrary_Common.Values)
        {
            GUIManager.instance.AddUnselectedCard(i);
        }
    }
    // 清空所有已选中的卡牌
    public void ClearSelectedCard()
    {
        cardLibrary_Selected.Clear();

        total_Card = 0;
    }
    // 选择卡牌
    public void SelectCard(CardBasicInfomation _card)
    {
        cardLibrary_Selected.Add(_card);

        total_Card++;
    }
    // 移除指定已选中的卡牌
    public void RemoveSelectedCard(CardBasicInfomation _card)
    {
        cardLibrary_Selected.Remove(_card);

        total_Card--;
    }

    // 载入所有已选择的卡牌-战斗场景时使用
    public void LoadSelectedCard()
    {
        foreach(var i in cardLibrary_Selected)
        {
            GameObject go = Instantiate<GameObject>(instanceCardLibrary[i.id]);
            go.transform.parent = tempLayoutGroup;
            go.transform.localScale = Vector3.one;

            cardQueue.Add(go);
        }

        while(cardQueue.Count != 0 && cur_Card != max_Cur_Card)
        {
            SendToLayoutGroup();
        }
    }

    // 清楚所有队列中的卡牌及其实例
    public void ClearAllCardQueue()
    {
        cardQueue.Clear();
        cardQueue_Discarded.Clear();

        for(int i = 0; i < layoutGroup.childCount; i++)
        {
            Destroy(layoutGroup.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < tempLayoutGroup.childCount; i++)
        {   
            Destroy(tempLayoutGroup.transform.GetChild(i).gameObject);
        }

        cur_Card = 0;
    }
}
