using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    [Header("Configuration")]
    public int max_Total_Card = 30; // 牌库最大可持有卡牌数
    public int min_Total_Card = 15; // 牌库最小持有数量
    public int max_Cur_Card = 5; // 当前最大可使用卡牌数

    public LayerMask groundLayer;

    [Header("Real-Time Setting")]
    public int total_Card; // 当前卡牌数
    public int cur_Card; // 当前可操作卡牌数

    public List<GameObject> cardQueue = new List<GameObject>(); // 未上场且未使用过卡牌队列
    public List<GameObject> cardQueue_Waitting= new List<GameObject>(); // 已商场且已使用过的卡牌列表

    public Dictionary<int, CardBasicInfomation> cardLibrary = new Dictionary<int, CardBasicInfomation>(); // 卡牌库 存放所有与角色相关的卡牌
    public Dictionary<int, CardBasicInfomation> cardLibrary_Common = new Dictionary<int, CardBasicInfomation>(); // 额外卡牌库
    public Dictionary<int, GameObject> instanceCardLibrary = new Dictionary<int, GameObject>(); // 卡牌信息对应实例卡牌

    public List<CardBasicInfomation> cardLibrary_Selected = new List<CardBasicInfomation>(); // 玩家选择的卡牌
    [Space]
    public bool lockingCards = false;

    [Header("Objects")]
    public CardCommonDatas cardCommonData;
    [Space]
    public Transform slotLayoutGroup; // 插槽容器
    public Transform layoutGroup; // 当前操作卡牌容器
    public RectTransform recTran_LayoutGroup;
    public Transform tempLayoutGroup; // 临时卡牌容器
    public RectTransform recTran_TempLayoutGroup;
    public Transform discardedCardLayoutGroup; // 用于放置一次性卡牌容器
    [Space]
    // 卡牌锁定相关
    public GameObject flap_LayoutGroup;
    public Coroutine timer_LockCards;
    [Space]
    public GameObject image_Tip;
    [Header("Optimization Objects")]
    public Transform[] slotPos = new Transform[6]; // 存储插槽坐标
    public float cardMoveTime = 1f;

    [Header("Temp")]
    public Vector3 test = Vector3.zero;

    private void Awake()
    {
        instance = this;

        // 存储布局插槽的坐标
        for(int i = 0; i < slotLayoutGroup.childCount; i++)
        {
            slotPos[i] = slotLayoutGroup.GetChild(i).transform;
        }

        LoadCardLibrary(); // 将所有卡牌信息载入到库中
        LoadCardInstance(); // 将库中卡牌对应的实例载入到字典中以备使用
    }

    void Start()
    {
        // 测试用——会与UI按钮控件相冲突 *****下列方法调用时 不要使用UI中的部分按钮*****

        //InitializeAllCards(); // 初始化解锁角色所有卡牌 等级设定为 1 同时锁定所有通用卡牌

        LoadAllCardIntoUnselectedList(); // 将卡牌库的信息载入到选择列表中
        LoadAllCardIntoCardList();// 卡牌展示 测试
    }

    private void OnDrawGizmos()
    {
        /*
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(test, 1);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(test, 2);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(test, 3);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(test, 4);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(test, 5);

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit,10000, groundLayer))
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(Camera.main.gameObject.transform.position, hit.point);
        }
        */

    }

    void Update()
    {
        /*
        // 测试用 查看技能影响距离范围
        if (true)
        {
            // 此处暂时将TriggerEffect函数内容调用至此
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit,10000, groundLayer))
            {
                test = hit.point;

                foreach(var i in Physics.SphereCastAll(ray, 3))
                {

                }
            }
        }


        // 测试用 生成战斗画面卡牌
        if (Input.GetMouseButtonDown(1))
        {
            //LoadSelectedCard();
            RealignAndLoadCards();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ClearAllActivatedCard();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            InitializeAllCards();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            //LockCards(1);
            DiscardCardRandomly(2);
        }
        */
    }

    private void OnDisable()
    {
        if (lockingCards)
        {
            lockingCards = false;
            StopCoroutine(timer_LockCards);
            flap_LayoutGroup.SetActive(false);
        }   
    }

    private void OnDestroy()
    {
        OnDisable();
    }

    /// <summary>
    /// 将等待队列中的卡牌送入游戏画面
    /// </summary>
    private void SendToUsableLayoutGroup()
    {
        GameObject go = cardQueue[0];
        cardQueue.RemoveAt(0);
        go.transform.parent = layoutGroup;  
        cur_Card++;

        // 刷新所有卡牌的初始位置
        foreach (var i in GetAllUsableCard())
        {
            i.GetComponent<CardPrototype>().ReflashOriginPos();
        }

        // 刷新手牌的位置
        ReflashUsableCardPosition();

        if(cardQueue.Count != 0)
            // 将下一张卡牌置顶
            cardQueue[0].transform.SetSiblingIndex(tempLayoutGroup.transform.childCount - 1);
    }

    /// <summary>
    /// 刷新手牌实体的位置
    /// </summary>
    public void ReflashUsableCardPosition()
    {
        // 刷新可用卡牌的位置
        for(int i = 0; i < layoutGroup.transform.childCount; i++)
        {
            Transform card = layoutGroup.transform.GetChild(i);

            card.DOScale(Vector3.one,cardMoveTime);

            if (card.position != slotPos[i].position)
            {
                card.DOMove(slotPos[i].position,cardMoveTime);
            }
        }
    }

    /// <summary>
    /// 将使用过的卡牌送出画面并放入弃牌队列中
    /// </summary>
    /// <param name="_card"></param>
    public void SendToDiscardedCardGroup(GameObject _card)
    {
        _card.transform.parent = tempLayoutGroup;
        cardQueue_Waitting.Add(_card);

        _card.transform.localScale = Vector3.one;

        // 临时使用 刷新弃牌队列
        ReflashTempLayoutGroup();

        cur_Card--;

        if (cardQueue.Count == 0)
        {
            // 可使用卡牌队列已空 打乱弃牌队列顺序并重新放入等待队列中
            while (cardQueue_Waitting.Count != 0)
            {
                int index = Random.Range(0, cardQueue_Waitting.Count);
                cardQueue.Add(cardQueue_Waitting[index]);
                cardQueue_Waitting.RemoveAt(index);
            }
        }

        // 将下一张卡牌放置在顶部
        cardQueue[0].transform.SetSiblingIndex(tempLayoutGroup.transform.childCount - 1);

        // ***** 尝试在这里代理发送使用过卡牌信号 *****
        BattleDataManager.instance.UpdateUsedCard(_card.GetComponent<CardPrototype>());

        SendToUsableLayoutGroup();
    }

    /// <summary>
    /// 重刷新Layout组件 (暂时不用)
    /// </summary>
    public void ReflashLayoutGroup()
    {
        //LayoutRebuilder.ForceRebuildLayoutImmediate(recTran_layoutGroup);
    }
    public void ReflashTempLayoutGroup()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(recTran_TempLayoutGroup);
    }

    // 载入所有卡牌信息
    public void LoadCardLibrary()
    {
        foreach(var i in Resources.LoadAll<CardBasicInfomation>("CardInfomation"))
        {
            if(i.belongner != CharacterType.CharacterTag.Common)
                cardLibrary.Add(i.id, i);
        }

        foreach (var i in Resources.LoadAll<CardBasicInfomation>("CardInfomation"))
        {
            if (i.belongner == CharacterType.CharacterTag.Common)
                cardLibrary_Common.Add(i.id, i);
        }
    }

    // 读取所有卡牌实例并放入字典中以作备用
    public void LoadCardInstance()
    {
        foreach (var i in Resources.LoadAll<GameObject>("CardInstances"))
        {
            instanceCardLibrary.Add(i.GetComponent<CardPrototype>().GetID(),i);
        }
    }



    /// <summary>
    /// 将卡牌库中的所有卡牌载入到卡牌展示界面中
    /// </summary>
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

    /// <summary>
    /// 将某个角色的卡牌载入到卡牌展示界面中
    /// </summary>
    /// <param name="_charId"></param>
    public void LoadSecificCharCardIntoCardList(int _charId)
    {
        GUIManager.instance.ClearCardList();

        if (_charId == -1)
        {
            foreach (var i in cardLibrary_Common.Values)
            {
                GUIManager.instance.LoadCardIntoList(i);
            }
            return;
        }


        foreach (var i in cardLibrary.Values)
        {
            if(((int)i.belongner) == _charId)
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
            tempCardList.Add(cardLibrary_Common[cardLibrary_Selected[i].id]);
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
            SendToUsableLayoutGroup();
        }
    }

    /// <summary>
    /// 初始化所有卡牌-开始新游戏时调用 (读取后使用)
    /// </summary>
    public void InitializeAllCards()
    {
        // 初始化角色卡牌
        foreach(var i in cardLibrary.Values)
        {
            i.InitilizeCard();
        }
        // 初始化剧情卡牌
        foreach(var i in cardLibrary_Common.Values)
        {
            i.LockCard();
        }
    }

    // 将所有剧情卡牌载入到未选中容器中
    public void LoadAllCardIntoUnselectedList()
    {
        // 清除卡牌操作由PlayerManager执行 此处临时使用
        GUIManager.instance.ClearUnselectedCardList();
        ClearSelectedCard();

        int temp = 0;

        // 应遍历通用卡牌库
        foreach (var i in cardLibrary_Common.Values)
        {
            if(i.level != 0)
            {
                GUIManager.instance.AddUnselectedCard(i);
                temp++;
            }
        }

        if(temp == 0)
        {
            image_Tip.gameObject.SetActive(true);
        }
        else
        {
            image_Tip.gameObject.SetActive(false);
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

    // 载入所有已选择的卡牌——战斗场景时使用
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
            SendToUsableLayoutGroup();
        }
    }

    /// <summary>
    ///  将该卡牌送入非激活卡牌组 无法再次被抽取
    /// </summary>
    /// <param name="_card"></param>
    public void SendToDisabledCardGroup(GameObject _card)
    {
        _card.transform.parent = discardedCardLayoutGroup;
        cur_Card--;

        if (cardQueue.Count == 0)
        {
            // 可使用卡牌队列已空 打乱弃牌队列顺序并重新放入等待队列中
            while (cardQueue_Waitting.Count != 0)
            {
                int index = Random.Range(0, cardQueue_Waitting.Count);
                cardQueue.Add(cardQueue_Waitting[index]);
                cardQueue_Waitting.RemoveAt(index);
            }
        }

        // ***** 尝试在这里代理发送使用过卡牌信号 *****
        BattleDataManager.instance.UpdateUsedCard(_card.GetComponent<CardPrototype>());

        SendToUsableLayoutGroup();


    }


    /// <summary>
    /// 清除场上所有卡牌实体以及卡牌队列及其弃牌队列
    /// </summary>
    public void ClearAllActivatedCard()
    {
        cardQueue.Clear();
        cardQueue_Waitting.Clear();

        var tempList = GetAllActivatedCard();
        for (int i = 0; i < tempList.Count; i++)
        {
            Destroy(tempList[i]);
        }

        cur_Card = 0;
    }

    /// <summary>
    /// 获得场上所有的卡牌游戏对象 参数用于是否获得已经使用过的一次性卡牌
    /// </summary>
    /// <returns></returns>

    public List<GameObject> GetAllActivatedCard(bool _includeDisabledCard = true)
    {
        List<GameObject> temp = new List<GameObject>();

        for (int i = 0; i < layoutGroup.childCount; i++)
        {
            temp.Add(layoutGroup.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < tempLayoutGroup.childCount; i++)
        {
            temp.Add(tempLayoutGroup.transform.GetChild(i).gameObject);
        }
        if (_includeDisabledCard)
        {
            for (int i = 0; i < discardedCardLayoutGroup.childCount; i++)
            {
                temp.Add(discardedCardLayoutGroup.GetChild(i).gameObject);
            }
        }


        return temp;
    }

    /// <summary>
    /// 获得手上所有卡牌的信息
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetAllUsableCard()
    {
        List<GameObject> temp = new List<GameObject>();

        for (int i = 0; i < layoutGroup.childCount; i++)
        {
            temp.Add(layoutGroup.transform.GetChild(i).gameObject);
        }

        return temp;
    }

    /// <summary>
    /// 获得符合相对概率的所有角色随机N个卡牌
    /// </summary>
    /// <param name="_count">获得的卡牌数量</param>
    /// <param name="_includeCommonCard">是否包括剧情卡</param>
    /// <returns></returns>
    public List<CardBasicInfomation> GetCardsRandomly(int _count,bool _includeCommonCard = false)
    {
        int time = _count;

        List<CardBasicInfomation> result = new List<CardBasicInfomation>();

        List<CardBasicInfomation> card_L1 = new List<CardBasicInfomation>();
        List<CardBasicInfomation> card_L2 = new List<CardBasicInfomation>();
        List<CardBasicInfomation> card_L3 = new List<CardBasicInfomation>();
        List<CardBasicInfomation> card_L4 = new List<CardBasicInfomation>();

        // 将玩家当全所选角色对应卡牌进行品阶分类
        foreach(var i in cardLibrary.Values)
        {
            if(i.belongner == PlayerManager.instance.cur_Character)
            {
                switch (i.rarity)
                {
                    case 1:
                        {
                            card_L1.Add(i);
                            break;
                        }
                    case 2:
                        {
                            card_L2.Add(i);
                            break;
                        }
                    case 3:
                        {
                            card_L3.Add(i);
                            break;
                        }
                    case 4:
                        {
                            card_L4.Add(i);
                            break;
                        }
                    default:break;
                }
            }
        }

        if (!_includeCommonCard)
        {
            // 1-4 分别为普通 稀有 史诗 传说
            float level1 = cardCommonData.probability[0];
            float level2 = cardCommonData.probability[1];
            float level3 = cardCommonData.probability[2];
            float level4 = cardCommonData.probability[3];

            // 重复抽卡
            while(time > 0)
            {
                float v = Random.Range(0, 1f);


                if (CheckInRange(v, 0, level1)) // 普通卡
                {
                    result.Add(card_L1[Random.Range(0, card_L1.Count)]);
                }else if (CheckInRange(v, level1, level1 + level2)) // 稀有卡
                {
                    result.Add(card_L2[Random.Range(0, card_L2.Count)]);
                }
                else if (CheckInRange(v, level1 + level2, level1 + level2 + level3)) // 史诗卡
                {
                    result.Add(card_L3[Random.Range(0, card_L3.Count)]);
                }
                else if (CheckInRange(v, level1 + level2 + level3, 1)) // 传说卡
                {
                    result.Add(card_L4[Random.Range(0, card_L4.Count)]);
                }

                time--;
            }

        }
        else // 包含剧情卡牌的抽卡
        {

        }

        return result.OrderBy(d => d.rarity).ToList();
    }

    // 判断数是否在一个范围内
    public bool CheckInRange(float _cv,float min,float max)
    {
        return min <= _cv && _cv < max;
    }

    /// <summary>
    /// 随机丢弃卡牌
    /// </summary>
    /// <param name="_count">丢弃数量</param>
    public void DiscardCardRandomly(int _count)
    {
        StartCoroutine(Timer_DiscardCards(_count));
    }

    IEnumerator Timer_DiscardCards(int _count)
    {
        int count = Mathf.Clamp(_count, 0, 6);

        List<GameObject> tempCardList = GetAllUsableCard();
        while (count-- > 0)
        {
            int index = Random.Range(0, tempCardList.Count);

            if (tempCardList[index] == null)
                break;

            SendToDiscardedCardGroup(tempCardList[index]);
            tempCardList.Remove(tempCardList[index]);
            yield return new WaitForSeconds(0.1f);
        }

        // 刷新所有卡牌的初始位置
        foreach (var i in GetAllUsableCard())
        {
            i.GetComponent<CardPrototype>().ReflashOriginPos();
            //i.GetComponent<CardPrototype>().ReturnToPos();
        }
    }

    /// <summary>
    /// 锁定卡牌
    /// </summary>
    /// <param name="_time">锁定时间</param>
    public void LockCards(float _time)
    {
        if (lockingCards)
        {
            StopCoroutine(timer_LockCards);
            timer_LockCards = StartCoroutine(Timer_LockCards(_time));
        }
        else
        {
            lockingCards = true;
            timer_LockCards = StartCoroutine(Timer_LockCards(_time));
        }
    }
    public IEnumerator Timer_LockCards(float _time)
    {
        flap_LayoutGroup.SetActive(true);
        yield return new WaitForSeconds(_time);
        flap_LayoutGroup.SetActive(false);
        lockingCards = false;
    }
}
