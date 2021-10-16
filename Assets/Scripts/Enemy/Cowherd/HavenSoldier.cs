using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HavenSoldier : EnemyStatus, ReducePower
{
    public float reduceRate = 0.1f;
    public float rebornTime = 1f;

    private bool status;
    private float rebornTimer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        status = true;
        rebornTimer = 0;
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

    private void Reborn()
    {

    }

    public override void Stun(float time)
    {
        // ѣ�νӿ�
        // ����ѣ��ʱ��
        if (!stunImmunity)
        {
            stun = true;
            stunTimer = time * stunEffect;
            // ֹͣ���������ظ�
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
                // �ָ����������ظ�
                StartReducing();
            }
        }
    }
}
