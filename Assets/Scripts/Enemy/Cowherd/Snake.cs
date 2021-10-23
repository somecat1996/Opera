using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : EnemyStatus, ReducePower
{
    public float reduceRate = -0.02f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        StartReducing();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!EnemyManager.instance.pause)
        {
            base.Update();
        }
    }

    public void StartReducing()
    {
        PlayerManager.instance.ChangeDecrement_RecoverySpeed_PowerPoint(reduceRate);
    }

    public void StopReducing()
    {
        PlayerManager.instance.ChangeDecrement_RecoverySpeed_PowerPoint(-reduceRate);
    }

    public override void Die()
    {
        StopReducing();
        base.Die();
    }

    public override void Stun(float time)
    {
        // 眩晕接口
        // 传入眩晕时间
        if (!stunImmunity)
        {
            stun = true;
            stunTimer = time * stunEffect;
            // 停止减少心流回复
            StopReducing();
        }
    }

    protected override void HandlingStun()
    {
        if (stunImmunity)
        {
            stunImmunityTimer -= Time.deltaTime;
            if (stunImmunityTimer <= 0)
            {
                stunImmunityTimer = 0;
                stunImmunity = false;
            }
        }
        else if (stun)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                stunTimer = 0;
                stun = false;
                // 恢复减少心流回复
                StartReducing();
            }
        }
    }
}
