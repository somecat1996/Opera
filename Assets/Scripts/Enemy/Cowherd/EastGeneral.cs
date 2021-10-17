using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EastGeneral : EnemyStatus
{
    public float damage = 1f;
    public float attackTime = 5f;

    public float healingTime = 1f;
    public float healing = 1f;

    private float attackTimer;
    private float healingTimer;
    private PlayerStatus playerStatus;
    private WesternQueen westernQueen;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        attackTimer = attackTime;
        healingTimer = healingTime;

        playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
    }

    protected override void Update()
    {
        if (!EnemyManager.instance.pause)
        {
            base.Update();
            Attack();
            Heal();
        }
    }

    public override void Hurt(float damage, bool shieldBreak = false, float damageIncrease = 1, HurtType type = HurtType.None)
    {
        base.Hurt(damage, shieldBreak, damageIncrease, type);
    }

    private void Attack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            attackTimer = attackTime;
            playerStatus.Hurt(damage);
        }
    }

    private void Heal()
    {
        healingTimer -= Time.deltaTime;
        if (healingTimer <= 0)
        {
            healingTimer = healingTime;
            westernQueen.InstantHealing(healingValue);
        }
    }

    public void Instantiate(WesternQueen queen)
    {
        westernQueen = queen;
    }
}
