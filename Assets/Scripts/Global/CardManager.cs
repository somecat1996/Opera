using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    [Header("Configuration")]
    public int max_Total_Card = 30; // �ƿ����ɳ��п�����
    public int min_Total_Card = 15; // �ƿ���С��������
    public int max_Cur_Card = 6; // ��ǰ����ʹ�ÿ�����

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

    [Header("Objects")]
    public CardCommonDatas cardCommonData;

    public Transform slotLayoutGroup; // �������
    public Transform layoutGroup; // ��ǰ������������
    public RectTransform recTran_layoutGroup;
    public Transform tempLayoutGroup; // ��ʱ��������
    public Transform discardedCardLayoutGroup; // ���ڷ���һ���Կ�������

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

        // �����á�������UI��ť�ؼ����ͻ *****���з�������ʱ ��Ҫʹ��UI�еĲ��ְ�ť*****

        LoadCardLibrary(); // �����п�����Ϣ���뵽����

        //InitializeAllCards(); // ��ʼ��������ɫ���п��� �ȼ��趨Ϊ 1 ͬʱ��������ͨ�ÿ���
        LoadCardInstance(); // �����п��ƶ�Ӧ��ʵ�����뵽�ֵ����Ա�ʹ��
        LoadAllCardIntoUnselectedList(); // �����ƿ����Ϣ���뵽ѡ���б���
        LoadAllCardIntoCardList();// ����չʾ ����
    }

    void Start()
    {

    }

    private void OnDrawGizmos()
    {
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


    }

    void Update()
    {
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
                    if(i.transform.tag == "Enemy")
                        Debug.Log(i.transform.parent.name);
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
        for(int i = 0; i < layoutGroup.transform.childCount; i++)
        {
            Transform card = layoutGroup.transform.GetChild(i);

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

    // ��ʼ�����п���-��ʼ����Ϸʱ���� (��ȡ��ʹ��)
    public void InitializeAllCards()
    {
        // ��ʼ����ɫ����
        foreach(var i in cardLibrary.Values)
        {
            i.InitilizeCard();
        }
        // ��ʼ�����⿨��
        foreach(var i in cardLibrary_Common.Values)
        {
            i.LockCard();
        }
    }

    // �����о��鿨�����뵽δѡ��������
    public void LoadAllCardIntoUnselectedList()
    {
        // ������Ʋ�����PlayerManagerִ�� �˴���ʱʹ��
       // GUIManager.instance.ClearUnselectedCardList();
        //ClearSelectedCard();

        // Ӧ����ͨ�ÿ��ƿ�
        foreach (var i in cardLibrary_Common.Values)
        {
            if(i.level != 0)
                GUIManager.instance.AddUnselectedCard(i);
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
}
