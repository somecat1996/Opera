using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    // 敌人生成位置，3*4方阵，从上到下从左到右排列
    // 9  6  3  0
    // 10 7  4  1
    // 11 8  5  2
    public List<Transform> generationPoint;
    // 测试敌人用
    public GameObject enemyPrefab;

    // 记录状态
    private EnemyStatus [] generationPointStatus;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        generationPointStatus = new EnemyStatus[generationPoint.Count];

        // 先招满，测试用
        for (int i = 0; i < generationPoint.Count; i++)
            SummonOne();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SummonOne()
    {
        List<int> avaliablePosition = new List<int>();
        for (int i = 0; i < generationPoint.Count; i++)
        {
            if (!generationPointStatus[i])
                avaliablePosition.Add(i);
        }
        if (avaliablePosition.Count > 0)
        {
            Debug.Log(avaliablePosition.Count);
            int p = avaliablePosition[Random.Range(0, avaliablePosition.Count - 1)];
            Debug.Log(p);

            GameObject tmp = Instantiate(enemyPrefab, generationPoint[p]);
            tmp.transform.localPosition = new Vector3(0, 0, 0);
            generationPointStatus[p] = tmp.GetComponent<EnemyStatus>();
            generationPointStatus[p].position = p;
        }
    }

    // 召唤小怪，位置不足则无法召唤
    public void SummonMinion(GameObject prefab)
    {
        List<int> avaliablePosition = new List<int>();
        for (int i = 0; i < generationPoint.Count; i++)
        {
            if (!generationPointStatus[i])
                avaliablePosition.Add(i);
        }
        if (avaliablePosition.Count > 0)
        {
            int p = avaliablePosition[Random.Range(0, avaliablePosition.Count - 1)];

            GameObject tmp = Instantiate(prefab, generationPoint[p]);
            tmp.transform.localPosition = new Vector3(0, 0, 0);
            generationPointStatus[p] = tmp.GetComponent<EnemyStatus>();
            BattleDataManager.instance.AddEnemyData(generationPointStatus[p]);
            generationPointStatus[p].position = p;
        }
    }

    // 召唤Boss，召唤至位置4
    public void SummonBoss(GameObject prefab)
    {
        if (!generationPointStatus[4])
        {
            GameObject tmp = Instantiate(prefab, generationPoint[4]);
            tmp.transform.localPosition = new Vector3(0, 0, 0);
            generationPointStatus[4] = tmp.GetComponent<EnemyStatus>();
            BattleDataManager.instance.AddEnemyData(generationPointStatus[4]);
            generationPointStatus[4].position = 4;
        }
    }

    public void Die(int p)
    {
        BattleDataManager.instance.AddEnemyData(generationPointStatus[p]);
        generationPointStatus[p] = null;
        // 测试用，死一个招一个
        SummonOne();
    }
}
