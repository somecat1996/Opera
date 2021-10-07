using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emperor : EnemyStatus, SummonEnemy
{
    [Header("Stage Configuration")]
    public float stage2Start = 0.7f;
    public float stage3Start = 0.3f;

    // ��ͨ����
    [Header("Normal Attack")]
    public float normalAttackTime = 1f;
    public float normalAttackDamage = 1f;
    private float normalAttackTimer;

    // �㽶Ƥ����
    [Header("Banana Attack")]
    public float bananaAttackTime = 10f;
    public float bananaAttackDamage = 3f;
    private float bananaAttackTimer;

    // �ٻ���
    [Header("Summon Minister")]
    public GameObject ministerPrefab;
    public float ministerSummonTime = 5f;
    private float ministerSummonTimer;

    // �ӳ�ʥּ
    [Header("Throw Imperial Decree")]
    public float imperialDecreeThrowTime = 10f;

    // �ӽ�
    //[Header("Swing Sword")]

    // ����prefab
    //[Header("Picture Prefab")]
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        normalAttackTimer = normalAttackTime;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (curHealth > maxHealth * stage2Start)
            Stage1();
        else if (curHealth > maxHealth * stage3Start)
            Stage2();
        else 
            Stage3();
    }

    public void SummonMinion(GameObject minion, int number = 1)
    {
        for (int i = 0; i < number; i++)
        {
            EnemyManager.instance.SummonMinion(minion);
        }
    }

    private void Stage1()
    {

    }

    private void Stage2()
    {

    }

    private void Stage3()
    {

    }
}
