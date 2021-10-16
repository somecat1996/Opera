using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HavenSoldier : EnemyStatus, ReducePower
{
    public float reduceRate = 0.1f;
    public float rebornTime = 1f;

    private bool alive;
    private float rebornTimer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        alive = true;
        rebornTimer = 0;
        StartReducing();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!EnemyManager.instance.pause)
        {
            if (alive)
            {
                base.Update();
            }
            else
            {
                rebornTimer -= Time.deltaTime;
                if (rebornTimer <= 0)
                {
                    rebornTimer = 0;
                    Reborn();
                }
            }
        }
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
        alive = false;
        rebornTimer = rebornTime;
    }

    private void Reborn()
    {
        StartReducing();
        alive = true;
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
