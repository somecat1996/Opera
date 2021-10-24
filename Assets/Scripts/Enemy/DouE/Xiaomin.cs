using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Xiaomin : EnemyStatus, ReduceDamage
{
    public float reduceCoolingTime = 10f;
    public float reduceTime = 3f;
    public float reduceRate = 0.1f;

    private float reduceCoolingTimer;
    private float reduceTimer;
    private bool reducing;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        reduceCoolingTimer = reduceCoolingTime;
        reduceTimer = 0;
        reducing = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!EnemyManager.instance.pause)
        {
            base.Update();

            reduceCoolingTimer -= Time.deltaTime;
            if (reduceCoolingTimer <= 0)
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
        if (!reducing)
        {
            EffectsManager.instance.CreateEffectFollow(7, reduceTime, transform, Vector3.zero);
            GlobalValue.damageDecrement_Player += reduceRate;
            reduceCoolingTimer = reduceCoolingTime;
            reduceTimer = reduceTime;
            reducing = true;
        }
    }

    public void StopReducing()
    {
        if (reducing)
        {
            GlobalValue.damageDecrement_Player -= reduceRate;
            reduceTimer = 0;
            reducing = false;
        }
    }

    public override void Die()
    {
        StopReducing();
        base.Die();
    }
}
