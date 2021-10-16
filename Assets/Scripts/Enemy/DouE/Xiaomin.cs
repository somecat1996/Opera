using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Xiaomin : EnemyStatus, ReduceDamage
{
    public float reduceCoolingTime = 10f;
    public float reduceTime = 3f;
    public float reduceRate = -0.1f;

    private float reduceCoolingTimer;
    private float reduceTimer;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        reduceCoolingTimer = reduceCoolingTime;
        reduceTimer = 0;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!EnemyManager.instance.pause)
        {
            base.Update();

            reduceCoolingTime -= Time.deltaTime;
            if (reduceCoolingTime <= 0)
                StartReducing();
            if (reduceTimer > 0)
            {
                reduceTimer -= Time.deltaTime;
                if (reduceTimer <= 0)
                    StopReducing();
            }
        }
    }

    public void StartReducing()
    {
        GlobalValue.damageIncrement_General += reduceRate;
        reduceCoolingTimer = reduceCoolingTime;
        reduceTimer = reduceTime;
    }

    public void StopReducing()
    {
        GlobalValue.damageIncrement_General -= reduceRate;
        reduceTimer = 0;
    }

    public override void Die()
    {
        StopReducing();
        base.Die();
    }
}
