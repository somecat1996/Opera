using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoodooDoll : SummonedObjectPrototype
{
    private EnemyStatus linkedEnemy;
    private float damage;
    // Start is called before the first frame update
    void Start()
    {
        damage = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // �����ӵĵ�������������
        if (!linkedEnemy)
        {
            Destroy(gameObject);
        }
    }
}
