using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : GameObjectBase
{
    public int position;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // 记录出生点
    public void SetPosition(int p)
    {
        position = p;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Hurt(float damage, bool shieldBreak, float damageIncrease)
    {
        base.Hurt(damage, shieldBreak, damageIncrease);
        if (curHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        EnemyManager.instance.Die(position);
        Destroy(healthBarManager.gameObject);
        Destroy(gameObject);
    }
}
