using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NorthGeneral : EnemyStatus
{
    public float damage = 1f;
    public float attackTime = 5f;

    public float hurtReduce = 0.3f;

    private float attackTimer;
    private PlayerStatus playerStatus;
    private WesternQueen westernQueen;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        attackTimer = attackTime;

        playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
    }

    protected override void Update()
    {
        if (!EnemyManager.instance.pause)
        {
            base.Update();
            Attack();
        }
    }

    public override void Hurt(float damage, bool shieldBreak = false, float damageIncrease = 1, HurtType type = HurtType.None)
    {
        animator.SetTrigger("Hurt");
        shadowAnimator.SetTrigger("Hurt");
        base.Hurt(damage, shieldBreak, damageIncrease, type);
    }

    private void Attack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            attackTimer = attackTime;
            playerStatus.Hurt(damage * EnemyManager.instance.EnemyAttackCoefficient());
        }
    }

    public void StartReducing()
    {
        if (westernQueen)
            westernQueen.ChangeHurtCoefficient(-hurtReduce);
    }

    public void StopReducing()
    {
        if (westernQueen)
            westernQueen.ChangeHurtCoefficient(hurtReduce);
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

    public void Instantiate(WesternQueen queen)
    {
        westernQueen = queen;
        westernQueen.ChangeHurtCoefficient(-hurtReduce);
    }
}
