using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : GameObjectBase
{
    public PlayerMovement playerMovement;

    private float allDamage;
    private float magicDamage;
    private float physicDamage;

    private bool started;

    protected override void Awake()
    {
        base.Awake();
        allDamage = 0f;
        magicDamage = 0f;
        physicDamage = 0f;

        started = false;
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        PlayerManager.instance.SetPlayer(gameObject.GetComponent<GameObjectBase>());
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (started && !EnemyManager.instance.pause)
        {
            base.Update();
            HandlingImmunity();
            HandlingInvisible();
        }
    }

    public void StartPlaying()
    {
        started = true;
        playerMovement.StartMoving();
    }

    public void StopPlaying()
    {
        started = false;
        playerMovement.StopMoving();
    }

    public void RestartPlaying()
    {
        started = true;
        playerMovement.StartMovingAt();
        curHealth = maxHealth;
        healthBarManager.UpdateHealth(curHealth / maxHealth);
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        PlayerManager.instance.SetCurrentHealthPoint(curHealth);
    }

    public override void Hurt(float damage, bool shieldBreak = false, float damageIncrease = 1, HurtType type = HurtType.None)
    {
        if (immunityTime > 0)
        {
            immunityTime -= 1;
            return;
        }
        else if (immunityTimer > 0)
            return;

        // 测试
        //Debug.Log(damage);
        //Debug.Log(curHealth);

        // 受伤接口，0-真伤，1-物理，2-魔法
        // 传入damage伤害数值，shieldBreak是否对护盾增伤，damageIncrease增伤比例
        float trueDamage;
        if (shield > 0 && shieldBreak)
            trueDamage = damageIncrease * damage;
        else
            trueDamage = damage;

        // 计算暴击
        switch (type)
        {
            case HurtType.Physic:
                if (Random.Range(0, 1f) < GlobalValue.probability_Crit_Physics)
                {
                    trueDamage *= 1 + GlobalValue.critIncrement_Physics;
                }
                break;
            case HurtType.Magic:
                if (Random.Range(0, 1f) < GlobalValue.probability_Crit_Magic)
                {
                    trueDamage *= 1 + GlobalValue.critIncrement_Magic;
                }
                break;
            default:
                break;
        }

        // 结算真实伤害
        if (shield > trueDamage)
        {
            shield -= trueDamage;
        }
        else
        {
            curHealth -= trueDamage - shield;
            shield = 0;
        }

        if (GlobalValue.poisonAttack)
            Poisoning();

        if (voodooTimer > 0)
            voodooHurt += trueDamage;

        healthBarManager.UpdateHealth(curHealth / maxHealth);

        var col = gameObject.GetComponent<Collider>();
        var topAhcor = new Vector3(col.bounds.center.x, col.bounds.max.y, col.bounds.center.z);
        DamageText damageText = Instantiate(damageTextPrefab, GameObject.FindGameObjectWithTag("DamageCanvas").transform).GetComponent<DamageText>();
        damageText.Init(trueDamage, topAhcor);

        UpdateHealth();

        if (curHealth <= 0)
        {
            EnemyManager.instance.FinishLevel(false);
        }
    }

    public override void InstantHealing(float healingValue)
    {
        base.InstantHealing(healingValue);
        UpdateHealth();
    }

    public float MagicDamageIncrease()
    {
        return 1 + allDamage + magicDamage;
    }

    public float PhysicDamageIncrease()
    {
        return 1 + allDamage + physicDamage;
    }

    public void StartMoving()
    {
        BattleDataManager.instance.UpdatePlayerMovingStatus(true);
    }

    public void StopMoving()
    {
        BattleDataManager.instance.UpdatePlayerMovingStatus(false);
    }

    public void PushTo(Vector3 position, float damage)
    {
        Hurt(damage);
        transform.position = position;
    }
}
