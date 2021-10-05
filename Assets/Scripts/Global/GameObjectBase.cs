using System.Collections;
using System.Collections.Generic;
using UnityEngine;


interface GameObjectInterface
{
    // 伤害接口
    // 传入damage伤害数值，shieldBreak是否对护盾增伤，damageIncrease增伤比例
    public void Hurt(float damage, bool shieldBreak, float damageIncrease);
    public void Hurt(CardPrototype cardPrototype);
    // 中毒接口，每次调用刷新中毒时间、增加中毒层数
    public void Poisoning();
    // 眩晕接口，调用后眩晕time时间 
    public void Stun(float time);
    // 返回是否眩晕
    public bool IsStun();
    // 控制免疫
    public void StunImmunity(float time);
    // 瞬间治疗接口，回复healingValue血量
    public void InstantHealing(float healingValue);
    // 持续治疗接口
    // time持续时间，tickle结算时间，value每次结算回复血量
    public void ContinuousHealing(float time, float tickle, float value);
    // 增加护盾
    // shieldValue护盾值，time护盾持续时间
    public void AddShield(float shieldValue, float time);
}

public class GameObjectBase : MonoBehaviour, GameObjectInterface
{
    public float maxHealth;
    public Vector3 offsetPos;
    public GameObject healthBarPrefab;
    // 中毒持续时间
    public int poisonTime = 5;
    // 中毒伤害
    public int poisonDamage = 5;
    // 中毒结算时间
    public int poisonDamageTime = 1;

    protected float curHealth;
    protected HealthBarManager healthBarManager;
    // 中毒持续时间计时器
    private float poisonTotalTimer;
    // 中毒伤害结算计时器
    private float poisonTimer;
    // 中毒层数
    private int poisonLevel;
    // 眩晕
    private float stunTimer;
    private bool stun;
    // 免疫眩晕
    private float stunImmunityTimer;
    private bool stunImmunity;
    // 护盾
    private float shield;
    private float shieldTimer;
    // 持续回复
    private float healingTotalTimer;
    private float healingTimer;
    private float healingTime;
    private float healingValue;
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

        curHealth = maxHealth;

        shield = 0f;
        shieldTimer = 0f;

        healingTime = 0;
        healingTimer = 0;
        healingTotalTimer = 0;
        healingValue = 0;
    }

    protected virtual void Start()
    {
        // 创建并初始化血条
        GameObject healthBar = Instantiate(healthBarPrefab, GameObject.FindGameObjectWithTag("HealthBarCanvas").transform);
        healthBarManager = healthBar.GetComponent<HealthBarManager>();

        healthBarManager.Init(transform, offsetPos);
    }

    protected virtual void Update()
    {
        HandlingPoison();
        HandlingStun();
        HandlingShield();
        HandlingHealing();
    }

    public virtual void Hurt(CardPrototype cardPrototype)
    {

    }

    public virtual void Hurt(float damage, bool shieldBreak=false, float damageIncrease=0)
    {
        // 受伤接口
        // 传入damage伤害数值，shieldBreak是否对护盾增伤，damageIncrease增伤比例
        if (shield > 0)
        {
            // 计算真实伤害
            float trueDamage;
            if (shieldBreak)
            {
                trueDamage = damageIncrease * damage;
            }
            else
            {
                trueDamage = damage;
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
        }
        else
        {
            curHealth -= damage;
        }
        healthBarManager.UpdateHealth(curHealth / maxHealth);
    }

    public void Poisoning()
    {
        // 中毒接口
        // 每次调用增加一层中毒并刷新中毒时间
        poisonLevel += 1;
        poisonTotalTimer = poisonTime;
        poisonTimer = poisonDamageTime;
    }

    public void Stun(float time)
    {
        // 眩晕接口
        // 传入眩晕时间
        if (!stunImmunity)
        {
            stun = true;
            stunTimer = time;
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

    public void InstantHealing(float healingValue)
    {

        curHealth += healingValue;
        if (curHealth > maxHealth)
            curHealth = maxHealth;
        healthBarManager.UpdateHealth(curHealth / maxHealth);
    }

    public void ContinuousHealing(float time, float tickle, float value)
    {
        healingTotalTimer = time;
        healingTime = tickle;
        healingTimer = healingTime;
        healingValue = value;
    }

    public void AddShield(float shieldValue, float time)
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
    protected void HandlingStun()
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
                Hurt(poisonDamage * poisonLevel, false, 0);
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
}
