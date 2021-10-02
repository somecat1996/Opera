using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    [Header("Configuration")]
    public int max_Total_Card = 30; // �ƿ����ɳ��п�����
    public int min_Total_Card = 15; // �ƿ���С��������
    public int max_Cur_Card = 6; // ��ǰ����ʹ�ÿ�����

    [Header("Real-Time Setting")]
    public int total_Card; // ��ǰ������
    public int cur_Card; // ��ǰ�ɲ���������

    public List<GameObject> cardQueue = new List<GameObject>(); // δ�ϳ����ƶ���
    public List<GameObject> cardQueue_Discarded= new List<GameObject>(); // ��ʹ�õĿ���

    public Dictionary<int, CardBasicInfomation> cardLibrary = new Dictionary<int, CardBasicInfomation>(); // ���ƿ� ����������ɫ��صĿ���
    public Dictionary<int, CardBasicInfomation> cardLibrary_Common = new Dictionary<int, CardBasicInfomation>(); // ���⿨�ƿ�
    public Dictionary<int, GameObject> instanceCardLibrary = new Dictionary<int, GameObject>(); // ������Ϣ��Ӧʵ������

    public List<CardBasicInfomation> cardLibrary_Selected = new List<CardBasicInfomation>(); // ���ѡ��Ŀ���

    [Header("Objects")]
    public CardCommonDatas cardCommonData;

    public Transform layoutGroup; // ��ǰ������������
    public RectTransform recTran_layoutGroup;
    public Transform tempLayoutGroup; // ��ʱ��������

    [Header("Temp")]
    public GameObject card_Attack;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

        // �����á�������UI��ť�ؼ����ͻ *****���з�������ʱ ��Ҫʹ��UI�еĲ��ְ�ť*****

        LoadCardLibrary(); // ��ָ����ɫ�������뵽��

        //InitializeAllCards(); // ��ʼ��������ɫ���п��� �ȼ��趨Ϊ 1 ͬʱ��������ͨ�ÿ���
        LoadCommonCardLibrary(); // ��ͨ�ÿ������뵽�б����Ա�ʹ��
        LoadCardInstance(); // �����п��ƶ�Ӧ��ʵ�����뵽�ֵ����Ա�ʹ��
        LoadAllCardIntoUnselectedList(); // �����ƿ����Ϣ���뵽ѡ���б���
        LoadAllCardIntoCardList();// ����չʾ ����

    }

    void Update()
    {
        // ������ ����ս�����濨��
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


    // ���ȴ������еĿ���������Ϸ����
    public void SendToLayoutGroup()
    {
        GameObject go = cardQueue[0];
        cardQueue.RemoveAt(0);
        go.transform.parent = layoutGroup;  
        cur_Card++;
    }
    // ��ʹ�ù��Ŀ����ͳ����沢�������ƶ�����
    public void SendToDiscardedCardGroup(GameObject _card)
    {
        _card.transform.parent = tempLayoutGroup;
        cardQueue_Discarded.Add(_card);

        cur_Card--;

        if (cardQueue.Count == 0)
        {
            // ��ʹ�ÿ��ƶ����ѿ� �������ƶ���˳�����·���ȴ�������
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

    // ���ݲ�ͬ��ɫ��ȡ��Ӧ��ɫ���ƿ�
    public void LoadCardLibrary()
    {
        // Ŀǰ��ʱֻ���뵩�Ŀ���

        //... ��������ѡ��ɫ
        // if(xx == CharacterType.Character.Dan) ...

        foreach(var i in Resources.LoadAll<CardBasicInfomation>("CardInfomation/Dan"))
        {
            cardLibrary.Add(i.id, i);
        }
    }

    // ��ȡ���⿨�ƿ�
    public void LoadCommonCardLibrary()
    {

    }

    // ��ȡ����ʵ���������ֵ�����������
    public void LoadCardInstance()
    {
        // �жϽ�ɫ����
        foreach (var i in Resources.LoadAll<GameObject>("CardInstances/Dan"))
        {
            instanceCardLibrary.Add(i.GetComponent<CardPrototype>().GetID(),i);
        }
    }

    

    // �����ƿ��е����п������뵽����չʾ������
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
            tempCardList.Add(cardLibrary[cardLibrary_Selected[i].id]);
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
            SendToLayoutGroup();
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

    // �����п������뵽δѡ��������
    public void LoadAllCardIntoUnselectedList()
    {
        // ������Ʋ�����PlayerManagerִ�� �˴���ʱʹ��
       // GUIManager.instance.ClearUnselectedCardList();
        //ClearSelectedCard();

        // Ӧ����ͨ�ÿ��ƿ�
        foreach (var i in cardLibrary_Common.Values)
        {
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

    // ����������ѡ��Ŀ���-ս������ʱʹ��
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

    // ������ж����еĿ��Ƽ���ʵ��
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
