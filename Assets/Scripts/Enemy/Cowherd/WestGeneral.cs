using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WestGeneral : EnemyStatus
{
    public float damage = 1f;
    public float attackTime = 5f;

    private float attackTimer;
    private PlayerStatus playerStatus;

    public float summonTime = 10f;
    public GameObject snakePrefab;

    private float summonTimer;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        summonTimer = summonTime;
        attackTimer = attackTime;

        playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
    }

    protected override void Update()
    {
        if (!EnemyManager.instance.pause)
        {
            base.Update();
            Attack();
            SummonSnake();
        }
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

    public override void Die()
    {
        base.Die();
    }

    public void SummonSnake()
    {
        summonTimer -= Time.deltaTime;
        if (summonTimer <= 0)
        {
            animator.SetTrigger("Buff");
            shadowAnimator.SetTrigger("Buff");
            summonTimer = summonTime;
            EnemyManager.instance.SummonMinion(snakePrefab);
        }
    }

    public override void Hurt(float damage, bool shieldBreak = false, float damageIncrease = 1, HurtType type = HurtType.None)
    {
        animator.SetTrigger("Hurt");
        shadowAnimator.SetTrigger("Hurt");
        base.Hurt(damage, shieldBreak, damageIncrease, type);
    }
}
