using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minister : EnemyStatus, ReducePower, SummonEnemy
{
    public float reduceRate;
    public GameObject soldierPrefab;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        StartReducing();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void StartReducing()
    {
        PlayerManager.instance.ChangeRecoverySpeed_PowerPoint(reduceRate);
    }

    public void StopReducing()
    {
        PlayerManager.instance.ChangeRecoverySpeed_PowerPoint(-reduceRate);
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
            EnemyManager.instance.SummonMinion(soldierPrefab);
        }
    }
}
