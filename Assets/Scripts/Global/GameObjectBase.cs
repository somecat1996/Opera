using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

interface GameObjectInterface
{
    public void SetMaxHealth(float m);
    // 伤害接口
    // 传入damage伤害数值，poison是否带毒（默认false），shieldBreak是否对护盾增伤（默认false），damageIncrease增伤比例（默认1）
    public void Hurt(float damage, bool shieldBreak, float damageIncrease, HurtType type);
    // 传入CardPrototype
    public void Hurt(CardPrototype cardPrototype);
    // 最大血量比例伤害
    public void PercentHurt(float percent, float max);
    // 中毒接口，每次调用刷新中毒时间、增加中毒层数
    public void Poisoning();
    // 流血接口，每次调用刷新5秒中毒
    public void Bleeding();
    // 眩晕接口，调用后眩晕time时间 
    public void Stun(float time);
    // 返回是否眩晕
    public bool IsStun();
    // 控制免疫
    public void StunImmunity(float time);
    // 免疫伤害
    public void ImmunityByTime(int num);
    public void ImmunityByDuration(float time);
    // 隐身动画效果
    public void Invisible(float time);
    // 瞬间治疗接口，回复healingValue血量
    public void InstantHealing(float healingValue);
    // 持续治疗接口
    // time持续时间，tickle结算时间，value每次结算回复血量
    public void ContinuousHealing(float time, float tickle, float value);
    // 增加护盾
    // shieldValue护盾值，time护盾持续时间
    public void AddShield(float shieldValue, float time);

    public float CurrentHealth();
    public float MaxHealth();
}

public enum HurtType
{
    None = 0,
    Physic = 1,
    Magic = 2
}

public class GameObjectBase : MonoBehaviour, GameObjectInterface
{
    [Header("血量设置")]
    public float maxHealth;
    public Vector3 offsetPos;
    public GameObject healthBarPrefab;
    public GameObject damageTextPrefab;
    [Header("血量设置")]
    // 中毒持续时间
    public int poisonTime = 5;
    // 中毒伤害
    public int poisonDamage = 5;
    // 中毒结算时间
    public int poisonDamageTime = 1;

    public float curHealth;
    protected HealthBarManager healthBarManager;
    // 中毒持续时间计时器
    protected float poisonTotalTimer;
    // 中毒伤害结算计时器
    protected float poisonTimer;
    // 中毒层数
    protected int poisonLevel;
    // 眩晕
    protected float stunTimer;
    protected bool stun;
    [Header("流血设置")]
    public float stunEffect = 1;
    // 流血
    protected float bleedingTimer;
    protected float bleedingTickleTimer;
    public float bleedingTime = 5f;
    public float bleedingTickleTime = 1f;
    public int bleedingDamage = 5;
    // 免疫眩晕
    protected float stunImmunityTimer;
    protected bool stunImmunity;
    [Header("免疫伤害设置")]
    // 免疫伤害
    protected int immunityTime;
    protected float immunityTimer;
    [Header("隐身设置")]
    // 隐身
    protected float invisibleTimer;
    [Header("护盾设置")]
    // 护盾
    public float shield;
    protected float shieldTimer;
    [Header("持续回复设置")]
    // 持续回复
    protected float healingTotalTimer;
    protected float healingTimer;
    protected float healingTime;
    protected float healingValue;
    // 巫毒娃娃
    protected float voodooProbability;
    protected float voodooCoolingTimer;
    protected float voodooTimer;
    protected float voodooHurt;
    public float voodooTime = 10f;
    public float voodoocoolingTime = 15f;

    protected MaterialController materialController;

    protected bool pause;
    // 玩家、敌人基类

    protected virtual void Awake()
    {
        poisonTotalTimer = 0;
        poisonTimer = 0f;
        poisonLevel = 0;
        stunTimer = 0f;
        stun = false;
        stunImmunityTimer = 0f;
        stunImmunity = false;

        immunityTime = 0;
        immunityTimer = 0;

        invisibleTimer = 0;

        bleedingTimer = 0f;
        bleedingTickleTimer = 0f;

        curHealth = maxHealth;

        shield = 0f;
        shieldTimer = 0f;

        healingTime = 0;
        healingTimer = 0;
        healingTotalTimer = 0;
        healingValue = 0;

        pause = false;

        materialController = gameObject.GetComponent<MaterialController>();
    }

    protected virtual void Start()
    {
        // 创建并初始化血条
        GameObject healthBar = Instantiate(healthBarPrefab, GameObject.FindGameObjectWithTag("HealthBarCanvas").transform);
        healthBarManager = healthBar.GetComponent<HealthBarManager>();

        healthBarManager.Init(transform, offsetPos);

        // 初始化buff
        voodooProbability = BuffManager.instance.GetProbability_SpawnVoodoo();
        voodooCoolingTimer = 0;
        voodooTimer = 0;
    }

    protected virtual void Update()
    {
        if (!EnemyManager.instance.pause)
        {
            HandlingVoodoo();
            HandlingPoison();
            HandlingStun();
            HandlingShield();
            HandlingHealing();
            HandlingBleeding();
        }
    }

    public void SetMaxHealth(float m)
    {
        maxHealth = m;
        curHealth = m;
    }

    public virtual void Hurt(CardPrototype cardPrototype)
    {

    }


    public virtual void Hurt(float damage, bool shieldBreak=false, float damageIncrease=1, HurtType type=HurtType.None)
    {
        // 受伤接口，0-真伤，1-物理，2-魔法
        // 传入damage伤害数值，shieldBreak是否对护盾增伤，damageIncrease增伤比例
        float trueDamage;
        if (shield > 0 && shieldBreak)
            trueDamage = damageIncrease * damage;
        else
            trueDamage = damage;

        bool critical = false;
        // 计算暴击
        switch (type)
        {
            case HurtType.Physic:
                if (Random.Range(0, 1f) < GlobalValue.probability_Crit_Physics)
                {
                    trueDamage *= 1 + GlobalValue.critIncrement_Physics;
                    critical = true;
                }
                break;
            case HurtType.Magic:
                if (Random.Range(0, 1f) < GlobalValue.probability_Crit_Magic)
                {
                    trueDamage *= 1 + GlobalValue.critIncrement_Magic;
                    critical = true;
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

        BattleDataManager.instance.UpdateDamage(trueDamage);
        if (healthBarManager.gameObject.activeInHierarchy)
            healthBarManager.UpdateHealth(curHealth / maxHealth);

        var col = gameObject.GetComponent<Collider>();
        var topAhcor = new Vector3(col.bounds.center.x, col.bounds.max.y, col.bounds.center.z);
        DamageText damageText = Instantiate(damageTextPrefab, GameObject.FindGameObjectWithTag("DamageCanvas").transform).GetComponent<DamageText>();
        damageText.Init(trueDamage, topAhcor, critical); 
    }

    public void PercentHurt(float percent, float max = Mathf.Infinity)
    {
        float trueDamage = percent * maxHealth;
        if (max <= trueDamage)
            Hurt(max);
        else
            Hurt(trueDamage);
    }

    public void RemoveShield()
    {
        shield = 0;
    }

    private void Voodoo()
    {
        if (voodooProbability > 0 && voodooCoolingTimer <= 0)
        {
            voodooCoolingTimer = voodoocoolingTime;
            voodooTimer = voodooTime;
            voodooHurt = 0;

            EffectsManager.instance.CreateEffectFollow(20, voodooTime, transform, Vector3.zero);
        }
    }

    public void Poisoning()
    {
        // 中毒接口
        // 每次调用增加一层中毒并刷新中毒时间
        if (!stunImmunity)
        {
            poisonLevel += 1;
            poisonTotalTimer = poisonTime;
            poisonTimer = poisonDamageTime;
            Voodoo();
            if (materialController)
                materialController.SetEnableChangeColor_Poison(true, poisonTime);
        }
    }

    public void Bleeding()
    {
        if (!stunImmunity)
        {
            bleedingTimer = bleedingTime;
            bleedingTickleTimer = bleedingTickleTime;
        }
    }

    public virtual void Stun(float time)
    {
        // 眩晕接口
        // 传入眩晕时间
        if (!stunImmunity)
        {
            stun = true;
            stunTimer = time * stunEffect;

            EffectsManager.instance.CreateEffectFollow(4, time * stunEffect, transform, Vector3.zero);
        }
    }

    public bool IsStun()
    {
        return stun;
    }

    public void StunImmunity(float time)
    {
        // 免疫眩晕time秒
        stunImmunity = true;
        stunImmunityTimer = time;
        stun = false;
        stunTimer = 0f;
    }

    public void ImmunityByTime(int num)
    {
        immunityTime += num;
    }

    public void ImmunityByDuration(float time)
    {
        immunityTimer += time;
    }

    public void Invisible(float time)
    {
        invisibleTimer += time;
    }

    public virtual void InstantHealing(float healingValue)
    {

        curHealth += healingValue;
        if (curHealth > maxHealth)
            curHealth = maxHealth;
        if (healthBarManager.gameObject.activeInHierarchy)
            healthBarManager.UpdateHealth(curHealth / maxHealth);


        var col = gameObject.GetComponent<Collider>();
        var topAhcor = new Vector3(col.bounds.center.x, col.bounds.max.y, col.bounds.center.z);
        DamageText damageText = Instantiate(damageTextPrefab, GameObject.FindGameObjectWithTag("DamageCanvas").transform).GetComponent<DamageText>();
        damageText.Init(healingValue, topAhcor);
        damageText.text.GetComponent<TMP_Text>().color = Color.green;
    }

    public void ContinuousHealing(float time, float tickle, float value)
    {
        healingTotalTimer = time;
        healingTime = tickle;
        healingTimer = healingTime;
        healingValue = value;
    }

    public virtual void AddShield(float shieldValue, float time)
    {
        shield = shieldValue;
        shieldTimer = time;
    }
    // 处理持续回复计时器
    protected void HandlingHealing()
    {
        if (healingTotalTimer > 0)
        {
            healingTotalTimer -= Time.deltaTime;
            healingTimer -= Time.deltaTime;
            if (healingTimer <= 0)
            {
                InstantHealing(healingValue);
                healingTimer = healingTime;
            }
            if (healingTotalTimer <= 0)
            {
                healingTime = 0;
                healingTimer = 0;
                healingTotalTimer = 0;
                healingValue = 0;
            }
        }
    }
    // 处理护盾计时器
    protected void HandlingShield()
    {
        if (shieldTimer > 0)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0)
            {
                shield = 0;
                shieldTimer = 0;
            }
        }
    }
    // 处理眩晕计时器
    protected virtual void HandlingStun()
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
            }
        }
    }
    // 处理中毒计时器
    protected void HandlingPoison()
    {
        if (poisonLevel > 0)
        {
            poisonTimer -= Time.deltaTime;
            poisonTotalTimer -= Time.deltaTime;

            if (poisonTimer <= 0)
            {
                // 结算伤害并刷新结算计时器
                Hurt(poisonDamage * poisonLevel);
                poisonTimer = poisonDamageTime;
            }
            if (poisonTotalTimer <= 0)
            {
                poisonLevel = 0;
                poisonTimer = 0;
                poisonTotalTimer = 0;
            }

        }
    }

    protected void HandlingBleeding()
    {
        if (bleedingTimer > 0)
        {
            bleedingTimer -= Time.deltaTime;
            bleedingTickleTimer -= Time.deltaTime;

            if (bleedingTickleTimer <= 0)
            {
                Hurt(bleedingDamage);
                bleedingTickleTimer = bleedingTickleTime;
            }
            if (bleedingTimer <= 0)
            {
                bleedingTimer = 0;
                bleedingTickleTimer = 0;
            }
        }
    }

    protected void HandlingImmunity()
    {
        if (immunityTimer > 0)
        {
            immunityTimer -= Time.deltaTime;
            if (immunityTimer <= 0)
            {
                immunityTimer = 0;
            }
        }
    }

    protected void HandlingInvisible()
    {
        if (invisibleTimer > 0)
        {
            invisibleTimer -= Time.deltaTime;
            if (invisibleTimer <= 0)
            {
                invisibleTimer = 0;
            }
        }
    }

    protected void HandlingVoodoo()
    {
        if (voodooProbability > 0)
        {
            if (voodooTimer > 0)
            {
                voodooTimer -= Time.deltaTime;
                if (voodooTimer <= 0)
                {
                    Hurt(voodooHurt * 0.8f);
                    voodooTimer = 0;
                }
            }
            if (voodooCoolingTimer > 0)
            {
                voodooCoolingTimer -= Time.deltaTime;
                if (voodooCoolingTimer <= 0)
                    voodooCoolingTimer = 0;
            }
        }
    }

    public float CurrentHealth()
    {
        return curHealth;
    }

    public float MaxHealth()
    {
        return maxHealth;
    }

    public bool IsStunImmunity()
    {
        return stunImmunity;
    }
}
