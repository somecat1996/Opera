using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minister : EnemyStatus, ReducePower, SummonEnemy
{
    public float reduceRate;
    public GameObject soldierPrefab;

    private PlayerManager playerManager;
    private EnemyManager enemyManager;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManager>();
        StartReducing();

        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void StartReducing()
    {
        playerManager.ChangeRecoverySpeed_PowerPoint(reduceRate);
    }

    public void StopReducing()
    {
        playerManager.ChangeRecoverySpeed_PowerPoint(-reduceRate);
    }

    public override void Die()
    {
        StopReducing();
        base.Die();
    }

    public void Summon(int number)
    {
        for (int i = 0; i < number; i++)
        {
            enemyManager.SummonMinion(soldierPrefab);
        }
    }
}
