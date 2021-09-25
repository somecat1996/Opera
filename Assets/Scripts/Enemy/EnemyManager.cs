using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // ��������λ�ã�3*3���󣬴��ϵ��´���������
    // 6 3 0
    // 7 4 1
    // 8 5 2
    public List<Transform> generationPoint;
    public GameObject enemyPrefab;

    // ��¼״̬
    private EnemyStatus [] generationPointStatus;
    // Start is called before the first frame update
    void Start()
    {
        generationPointStatus = new EnemyStatus[generationPoint.Count];

        // ������
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

    public void Die(int p)
    {
        generationPointStatus[p] = null;
        // �����ã���һ����һ��
        SummonOne();
    }
}
