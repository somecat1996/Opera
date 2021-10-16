using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Xianguan : EnemyStatus
{
    public float attackTime = 5f;
    public float attackDamage = 1f;

    private float attackTimer;
    private PlayerStatus player;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        attackTimer = attackTime;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!EnemyManager.instance.pause)
        {
            base.Update();

            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
                NormalAttack();
        }
    }

    private void NormalAttack()
    {
        player.Hurt(attackDamage);
        attackTimer = attackTime;
    }
}
