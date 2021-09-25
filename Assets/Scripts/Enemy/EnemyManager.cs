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

    // ��¼״̬
    private EnemyStatus [] generationPointStatus;
    // Start is called before the first frame update
    void Start()
    {
        generationPointStatus = new EnemyStatus[generationPoint.Count];
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
            if (generationPointStatus[i])
                avaliablePosition.Add(i);
        }
        int p = avaliablePosition[Random.Range(0, avaliablePosition.Count)];

    }

    public void Die(int p)
    {
        generationPointStatus[p] = null;
    }
}
