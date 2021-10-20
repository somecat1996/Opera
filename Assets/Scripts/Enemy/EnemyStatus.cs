using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : GameObjectBase
{
    public int position;

    protected Animator animator;
    protected Animator shadowAnimator;
    // Start is called before the first frame update
    protected override void Start()
    {
        maxHealth *= EnemyManager.instance.EnemyHealthCoefficient();
        curHealth = maxHealth;

        base.Start();

        animator = transform.Find("Sprite").GetComponent<Animator>();
        shadowAnimator = transform.Find("Shadow").GetComponent<Animator>();
    }

    // 记录出生点
    public void SetPosition(int p)
    {
        position = p;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!EnemyManager.instance.pause)
        {
            HandlingVoodoo();
            HandlingPoison();
            HandlingStun();
            HandlingHealing();
            HandlingBleeding();
        }
    }

    public override void Hurt(float damage, bool shieldBreak = false, float damageIncrease = 1, HurtType type = HurtType.None)
    {
        base.Hurt(damage, shieldBreak, damageIncrease, type);
        if (curHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        EnemyManager.instance.Die(position);
        if (healthBarManager)
            Destroy(healthBarManager.gameObject);
        Destroy(gameObject);
    }

    public virtual void Kill()
    {
        EnemyManager.instance.Die(position);
        if (healthBarManager)
            Destroy(healthBarManager.gameObject);
        Destroy(gameObject);
    }
}
