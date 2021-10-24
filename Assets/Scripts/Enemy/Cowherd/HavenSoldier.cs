using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HavenSoldier : EnemyStatus, ReducePower
{
    public float reduceRate = 0.1f;
    public float rebornTime = 1f;

    private bool alive;
    private float rebornTimer;
    private bool block;

    private GameObject birdBridge;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        birdBridge = transform.Find("BirdBridge").gameObject;

        alive = true;
        block = false;
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
            else if (!block)
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

    public override void Hurt(float damage, bool shieldBreak = false, float damageIncrease = 1, HurtType type = HurtType.None)
    {
        if (alive)
        {
            animator.SetTrigger("Hurt");
            shadowAnimator.SetTrigger("Hurt");
            base.Hurt(damage, shieldBreak, damageIncrease, type);
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
        alive = false;
        rebornTimer = rebornTime;
        animator.gameObject.SetActive(false);
        shadowAnimator.gameObject.SetActive(false);
    }

    private void Reborn()
    {
        StartReducing();
        alive = true;
        animator.gameObject.SetActive(true);
        shadowAnimator.gameObject.SetActive(true);

        curHealth = maxHealth;
        healthBarManager.UpdateHealth(curHealth / maxHealth);
    }

    public void StopReborn()
    {
        block = true;
        birdBridge.SetActive(true);
        animator.gameObject.SetActive(false);
        shadowAnimator.gameObject.SetActive(false);

        Kill();
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

            EffectsManager.instance.CreateEffectFollow(4, time * stunEffect, transform, Vector3.zero);
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

    public void Block()
    {
        block = true;
    }
}
