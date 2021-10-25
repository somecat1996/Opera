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
    public int max_Total_Card = 30; // �ƿ����ɳ��п�����
    public int min_Total_Card = 15; // �ƿ���С��������
    public int max_Cur_Card = 5; // ��ǰ����ʹ�ÿ�����

    public LayerMask groundLayer;

    [Header("Real-Time Setting")]
    public int total_Card; // ��ǰ������
    public int cur_Card; // ��ǰ�ɲ���������

    public List<GameObject> cardQueue = new List<GameObject>(); // δ�ϳ���δʹ�ù����ƶ���
    public List<GameObject> cardQueue_Waitting= new List<GameObject>(); // ���̳�����ʹ�ù��Ŀ����б�

    public Dictionary<int, CardBasicInfomation> cardLibrary = new Dictionary<int, CardBasicInfomation>(); // ���ƿ� ����������ɫ��صĿ���
    public Dictionary<int, CardBasicInfomation> cardLibrary_Common = new Dictionary<int, CardBasicInfomation>(); // ���⿨�ƿ�
    public Dictionary<int, GameObject> instanceCardLibrary = new Dictionary<int, GameObject>(); // ������Ϣ��Ӧʵ������

    public List<CardBasicInfomation> cardLibrary_Selected = new List<CardBasicInfomation>(); // ���ѡ��Ŀ���
    [Space]
    public bool lockingCards = false;

    [Header("Objects")]
    public CardCommonDatas cardCommonData;
    [Space]
    public Transform slotLayoutGroup; // �������
    public Transform layoutGroup; // ��ǰ������������
    public RectTransform recTran_LayoutGroup;
    public Transform tempLayoutGroup; // ��ʱ��������
    public RectTransform recTran_TempLayoutGroup;
    public Transform discardedCardLayoutGroup; // ���ڷ���һ���Կ�������
    [Space]
    // �����������
    public GameObject flap_LayoutGroup;
    public Coroutine timer_LockCards;
    [Space]
    public GameObject image_Tip;
    [Header("Optimization Objects")]
    public Transform[] slotPos = new Transform[6]; // �洢�������
    public float cardMoveTime = 1f;

    [Header("Temp")]
    public Vector3 test = Vector3.zero;

    private void Awake()
    {
        instance = this;

        // �洢���ֲ�۵�����
        for(int i = 0; i < slotLayoutGroup.childCount; i++)
        {
            slotPos[i] = slotLayoutGroup.GetChild(i).transform;
        }

        LoadCardLibrary(); // �����п�����Ϣ���뵽����
        LoadCardInstance(); // �����п��ƶ�Ӧ��ʵ�����뵽�ֵ����Ա�ʹ��
    }

    void Start()
    {
        // �����á�������UI��ť�ؼ����ͻ *****���з�������ʱ ��Ҫʹ��UI�еĲ��ְ�ť*****

        //InitializeAllCards(); // ��ʼ��������ɫ���п��� �ȼ��趨Ϊ 1 ͬʱ��������ͨ�ÿ���

        LoadAllCardIntoUnselectedList(); // �����ƿ����Ϣ���뵽ѡ���б���
        LoadAllCardIntoCardList();// ����չʾ ����
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
        // ������ �鿴����Ӱ����뷶Χ
        if (true)
        {
            // �˴���ʱ��TriggerEffect�������ݵ�������
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


        // ������ ����ս�����濨��
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
    /// ���ȴ������еĿ���������Ϸ����
    /// </summary>
    private void SendToUsableLayoutGroup()
    {
        GameObject go = cardQueue[0];
        cardQueue.RemoveAt(0);
        go.transform.parent = layoutGroup;  
        cur_Card++;

        // ˢ�����п��Ƶĳ�ʼλ��
        foreach (var i in GetAllUsableCard())
        {
            i.GetComponent<CardPrototype>().ReflashOriginPos();
        }

        // ˢ�����Ƶ�λ��
        ReflashUsableCardPosition();

        if(cardQueue.Count != 0)
            // ����һ�ſ����ö�
            cardQueue[0].transform.SetSiblingIndex(tempLayoutGroup.transform.childCount - 1);
    }

    /// <summary>
    /// ˢ������ʵ���λ��
    /// </summary>
    public void ReflashUsableCardPosition()
    {
        // ˢ�¿��ÿ��Ƶ�λ��
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
    /// ��ʹ�ù��Ŀ����ͳ����沢�������ƶ�����
    /// </summary>
    /// <param name="_card"></param>
    public void SendToDiscardedCardGroup(GameObject _card)
    {
        _card.transform.parent = tempLayoutGroup;
        cardQueue_Waitting.Add(_card);

        _card.transform.localScale = Vector3.one;

        // ��ʱʹ�� ˢ�����ƶ���
        ReflashTempLayoutGroup();

        cur_Card--;

        if (cardQueue.Count == 0)
        {
            // ��ʹ�ÿ��ƶ����ѿ� �������ƶ���˳�����·���ȴ�������
            while (cardQueue_Waitting.Count != 0)
            {
                int index = Random.Range(0, cardQueue_Waitting.Count);
                cardQueue.Add(cardQueue_Waitting[index]);
                cardQueue_Waitting.RemoveAt(index);
            }
        }

        // ����һ�ſ��Ʒ����ڶ���
        cardQueue[0].transform.SetSiblingIndex(tempLayoutGroup.transform.childCount - 1);

        // ***** ���������������ʹ�ù������ź� *****
        BattleDataManager.instance.UpdateUsedCard(_card.GetComponent<CardPrototype>());

        SendToUsableLayoutGroup();
    }

    /// <summary>
    /// ��ˢ��Layout��� (��ʱ����)
    /// </summary>
    public void ReflashLayoutGroup()
    {
        //LayoutRebuilder.ForceRebuildLayoutImmediate(recTran_layoutGroup);
    }
    public void ReflashTempLayoutGroup()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(recTran_TempLayoutGroup);
    }

    // �������п�����Ϣ
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

    // ��ȡ���п���ʵ���������ֵ�����������
    public void LoadCardInstance()
    {
        foreach (var i in Resources.LoadAll<GameObject>("CardInstances"))
        {
            instanceCardLibrary.Add(i.GetComponent<CardPrototype>().GetID(),i);
        }
    }



    /// <summary>
    /// �����ƿ��е����п������뵽����չʾ������
    /// </summary>
    public void LoadAllCardIntoCardList()
    {
        GUIManager.instance.ClearCardList();
        foreach (var i in cardLibrary.Values)
        {
            GUIManager.instance.LoadCardIntoList(i);
        }

        // ͨ�ÿ���
        foreach(var i in cardLibrary_Common.Values)
        {
            GUIManager.instance.LoadCardIntoList(i);
        }
    }

    /// <summary>
    /// ��ĳ����ɫ�Ŀ������뵽����չʾ������
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


    // �������в����������ѡ��Ŀ��ơ���***��ʼս������ʱʹ��***
    public void RealignAndLoadCards()
    {
        List<CardBasicInfomation> tempCardList = new List<CardBasicInfomation>();

        // �����ɫ��
        foreach(var i in cardLibrary.Values)
        {
            if(i.belongner == PlayerManager.instance.cur_Character)
            {
                tempCardList.Add(i);
            }
        }

        // ������ѡ���ͨ�ÿ�
        for(int i = 0; i < cardLibrary_Selected.Count; i++)
        {
            tempCardList.Add(cardLibrary_Common[cardLibrary_Selected[i].id]);
        }

        // ��������
        int count = tempCardList.Count - 1;

        while (count != 0)
        {
            int index = Random.Range(0, count + 1);
            
            CardBasicInfomation temp = tempCardList[count];
            tempCardList[count] = tempCardList[index];
            tempCardList[index] = temp;

            count --;
        }

        // ��ѡ���ƽ���ѹ��ȴ�����
        // ���ڽ�����������
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
    /// ��ʼ�����п���-��ʼ����Ϸʱ���� (��ȡ��ʹ��)
    /// </summary>
    public void InitializeAllCards()
    {
        // ��ʼ����ɫ����
        foreach(var i in cardLibrary.Values)
        {
            i.InitilizeCard();
        }
        // ��ʼ�����鿨��
        foreach(var i in cardLibrary_Common.Values)
        {
            i.LockCard();
        }
    }

    // �����о��鿨�����뵽δѡ��������
    public void LoadAllCardIntoUnselectedList()
    {
        // ������Ʋ�����PlayerManagerִ�� �˴���ʱʹ��
        GUIManager.instance.ClearUnselectedCardList();
        ClearSelectedCard();

        int temp = 0;

        // Ӧ����ͨ�ÿ��ƿ�
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
    // ���������ѡ�еĿ���
    public void ClearSelectedCard()
    {
        cardLibrary_Selected.Clear();

        total_Card = 0;
    }
    // ѡ����
    public void SelectCard(CardBasicInfomation _card)
    {
        cardLibrary_Selected.Add(_card);

        total_Card++;
    }
    // �Ƴ�ָ����ѡ�еĿ���
    public void RemoveSelectedCard(CardBasicInfomation _card)
    {
        cardLibrary_Selected.Remove(_card);

        total_Card--;
    }

    // ����������ѡ��Ŀ��ơ���ս������ʱʹ��
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
    ///  ���ÿ�������Ǽ������ �޷��ٴα���ȡ
    /// </summary>
    /// <param name="_card"></param>
    public void SendToDisabledCardGroup(GameObject _card)
    {
        _card.transform.parent = discardedCardLayoutGroup;
        cur_Card--;

        if (cardQueue.Count == 0)
        {
            // ��ʹ�ÿ��ƶ����ѿ� �������ƶ���˳�����·���ȴ�������
            while (cardQueue_Waitting.Count != 0)
            {
                int index = Random.Range(0, cardQueue_Waitting.Count);
                cardQueue.Add(cardQueue_Waitting[index]);
                cardQueue_Waitting.RemoveAt(index);
            }
        }

        // ***** ���������������ʹ�ù������ź� *****
        BattleDataManager.instance.UpdateUsedCard(_card.GetComponent<CardPrototype>());

        SendToUsableLayoutGroup();


    }


    /// <summary>
    /// ����������п���ʵ���Լ����ƶ��м������ƶ���
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
    /// ��ó������еĿ�����Ϸ���� ���������Ƿ����Ѿ�ʹ�ù���һ���Կ���
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
    /// ����������п��Ƶ���Ϣ
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
    /// ��÷�����Ը��ʵ����н�ɫ���N������
    /// </summary>
    /// <param name="_count">��õĿ�������</param>
    /// <param name="_includeCommonCard">�Ƿ�������鿨</param>
    /// <returns></returns>
    public List<CardBasicInfomation> GetCardsRandomly(int _count,bool _includeCommonCard = false)
    {
        int time = _count;

        List<CardBasicInfomation> result = new List<CardBasicInfomation>();

        List<CardBasicInfomation> card_L1 = new List<CardBasicInfomation>();
        List<CardBasicInfomation> card_L2 = new List<CardBasicInfomation>();
        List<CardBasicInfomation> card_L3 = new List<CardBasicInfomation>();
        List<CardBasicInfomation> card_L4 = new List<CardBasicInfomation>();

        // ����ҵ�ȫ��ѡ��ɫ��Ӧ���ƽ���Ʒ�׷���
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
            // 1-4 �ֱ�Ϊ��ͨ ϡ�� ʷʫ ��˵
            float level1 = cardCommonData.probability[0];
            float level2 = cardCommonData.probability[1];
            float level3 = cardCommonData.probability[2];
            float level4 = cardCommonData.probability[3];

            // �ظ��鿨
            while(time > 0)
            {
                float v = Random.Range(0, 1f);


                if (CheckInRange(v, 0, level1)) // ��ͨ��
                {
                    result.Add(card_L1[Random.Range(0, card_L1.Count)]);
                }else if (CheckInRange(v, level1, level1 + level2)) // ϡ�п�
                {
                    result.Add(card_L2[Random.Range(0, card_L2.Count)]);
                }
                else if (CheckInRange(v, level1 + level2, level1 + level2 + level3)) // ʷʫ��
                {
                    result.Add(card_L3[Random.Range(0, card_L3.Count)]);
                }
                else if (CheckInRange(v, level1 + level2 + level3, 1)) // ��˵��
                {
                    result.Add(card_L4[Random.Range(0, card_L4.Count)]);
                }

                time--;
            }

        }
        else // �������鿨�Ƶĳ鿨
        {

        }

        return result.OrderBy(d => d.rarity).ToList();
    }

    // �ж����Ƿ���һ����Χ��
    public bool CheckInRange(float _cv,float min,float max)
    {
        return min <= _cv && _cv < max;
    }

    /// <summary>
    /// �����������
    /// </summary>
    /// <param name="_count">��������</param>
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

        // ˢ�����п��Ƶĳ�ʼλ��
        foreach (var i in GetAllUsableCard())
        {
            i.GetComponent<CardPrototype>().ReflashOriginPos();
            //i.GetComponent<CardPrototype>().ReturnToPos();
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="_time">����ʱ��</param>
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
