using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    // ��������λ�ã�3*4���󣬴��ϵ��´���������
    // 9  6  3  0
    // 10 7  4  1
    // 11 8  5  2
    public List<Transform> generationPoint;
    // ��ͼ����λ��
    public Transform middleGenerationPoint;
    // ���Ե�����
    public GameObject enemyPrefab;

    // ��¼״̬
    private EnemyStatus [] generationPointStatus;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        generationPointStatus = new EnemyStatus[generationPoint.Count];

        // ��������������
        int ministerPosition = SummonOne();
        Minister minister = generationPointStatus[ministerPosition].GetComponent<Minister>();
        minister.SummonMinion(minister.soldierPrefab, 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int SummonOne()
    {
        int p = -1;
        List<int> avaliablePosition = new List<int>();
        for (int i = 0; i < generationPoint.Count; i++)
        {
            if (!generationPointStatus[i])
                avaliablePosition.Add(i);
        }
        if (avaliablePosition.Count > 0)
        {
            Debug.Log(avaliablePosition.Count);
            p = avaliablePosition[Random.Range(0, avaliablePosition.Count - 1)];
            Debug.Log(p);

            GameObject tmp = Instantiate(enemyPrefab, generationPoint[p]);
            tmp.transform.localPosition = new Vector3(0, 0, 0);
            generationPointStatus[p] = tmp.GetComponent<EnemyStatus>();
            BattleDataManager.instance.AddEnemyData(generationPointStatus[p]);
            generationPointStatus[p].position = p;
        }
        return p;
    }

    // �ٻ�С�֣�λ�ò������޷��ٻ�
    public int SummonMinion(GameObject prefab)
    {
        int p = -1;
        List<int> avaliablePosition = new List<int>();
        for (int i = 0; i < generationPoint.Count; i++)
        {
            if (!generationPointStatus[i])
                avaliablePosition.Add(i);
        }
        if (avaliablePosition.Count > 0)
        {
            p = avaliablePosition[Random.Range(0, avaliablePosition.Count - 1)];

            GameObject tmp = Instantiate(prefab, generationPoint[p]);
            tmp.transform.localPosition = new Vector3(0, 0, 0);
            generationPointStatus[p] = tmp.GetComponent<EnemyStatus>();
            BattleDataManager.instance.AddEnemyData(generationPointStatus[p]);
            generationPointStatus[p].position = p;
        }
        return p;
    }

    // �ٻ�Boss���ٻ���λ��4
    public int SummonBoss(GameObject prefab)
    {
        int p = -1;
        if (!generationPointStatus[4])
        {
            p = 4;
            GameObject tmp = Instantiate(prefab, generationPoint[p]);
            tmp.transform.localPosition = new Vector3(0, 0, 0);
            generationPointStatus[p] = tmp.GetComponent<EnemyStatus>();
            BattleDataManager.instance.AddEnemyData(generationPointStatus[p]);
            generationPointStatus[p].position = p;
        }
        return p;
    }

    // ������λ���ٻ�
    public void SummonInMiddle(GameObject prefab)
    {
        GameObject tmp = Instantiate(prefab, middleGenerationPoint);
        tmp.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void Die(int p)
    {
        BattleDataManager.instance.RemoveEnemyData(generationPointStatus[p]);
        generationPointStatus[p] = null;
        // �����ã���һ����һ��
        SummonOne();
    }
}
