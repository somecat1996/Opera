using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    [Header("Configuration")]
    public int max_total_card = 10; // �ƿ����ɳ��п�����
    public int max_cur_card = 6; // ��ǰ����ʹ�ÿ�����

    [Header("Real-Time Setting")]
    public int total_card; // ��ǰ������
    public int cur_card; // ��ǰ�ɲ���������

    public Queue<GameObject> cardQueue = new Queue<GameObject>(); // δ�ϳ����ƶ���

    [Header("Objects")]
    public Transform layoutGroup; // ��ǰ������������
    public RectTransform recTran_layoutGroup;
    public Transform tempLayoutGroup; // ��ʱ��������

    [Header("Temp")]
    public GameObject card_Attack;
    public Dictionary<int, CardBasicInfomation> dic = new Dictionary<int, CardBasicInfomation>();

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // ***��̬��ȡ����***
        int j = 10;
        foreach (var i in Resources.LoadAll<CardBasicInfomation>("CardInfomation")) {
            Debug.Log(i.id);
            i.level = j;
            j += j;
        }

        // ������
        // ��ʽ��ʼ��Ϸǰ ��Ҫ����ѡ���ƽ���ѹ����в���
        // ���ڽ�����������
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

    // ���ȴ������еĿ���������Ϸ����
    public void SendToLayoutGroup()
    {
        if (cardQueue.Count != 0)
        {
            GameObject go = cardQueue.Dequeue();
            go.transform.parent = layoutGroup;
            cur_card++;
        }
    }
    // ��ʹ�ù��Ŀ����ͳ����沢����ȴ�����
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
