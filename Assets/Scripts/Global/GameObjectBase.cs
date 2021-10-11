using System.Collections;
using System.Collections.Generic;
using UnityEngine;


interface GameObjectInterface
{
    public void SetMaxHealth(float m);
    // �˺��ӿ�
    // ����damage�˺���ֵ��poison�Ƿ������Ĭ��false����shieldBreak�Ƿ�Ի������ˣ�Ĭ��false����damageIncrease���˱�����Ĭ��1��
    public void Hurt(float damage, bool shieldBreak, float damageIncrease);
    // ����CardPrototype
    public void Hurt(CardPrototype cardPrototype);
    // ���Ѫ�������˺�
    public void PercentHurt(float percent, float max);
    // �ж��ӿڣ�ÿ�ε���ˢ���ж�ʱ�䡢�����ж�����
    public void Poisoning();
    // ��Ѫ�ӿڣ�ÿ�ε���ˢ��5���ж�
    public void Bleeding();
    // ѣ�νӿڣ����ú�ѣ��timeʱ�� 
    public void Stun(float time);
    // �����Ƿ�ѣ��
    public bool IsStun();
    // ��������
    public void StunImmunity(float time);
    // �����˺�
    public void ImmunityByTime(int num);
    public void ImmunityByDuration(float time);
    // ������Ч��
    public void Invisible(float time);
    // ˲�����ƽӿڣ��ظ�healingValueѪ��
    public void InstantHealing(float healingValue);
    // �������ƽӿ�
    // time����ʱ�䣬tickle����ʱ�䣬valueÿ�ν���ظ�Ѫ��
    public void ContinuousHealing(float time, float tickle, float value);
    // ���ӻ���
    // shieldValue����ֵ��time���ܳ���ʱ��
    public void AddShield(float shieldValue, float time);

    public float CurrentHealth();
    public float MaxHealth();
}

public class GameObjectBase : MonoBehaviour, GameObjectInterface
{
    [Header("Ѫ������")]
    public float maxHealth;
    public Vector3 offsetPos;
    public GameObject healthBarPrefab;
    public GameObject damageTextPrefab;
    [Header("Ѫ������")]
    // �ж�����ʱ��
    public int poisonTime = 5;
    // �ж��˺�
    public int poisonDamage = 5;
    // �ж�����ʱ��
    public int poisonDamageTime = 1;

    protected float curHealth;
    protected HealthBarManager healthBarManager;
    // �ж�����ʱ���ʱ��
    protected float poisonTotalTimer;
    // �ж��˺������ʱ��
    protected float poisonTimer;
    // �ж�����
    protected int poisonLevel;
    // ѣ��
    protected float stunTimer;
    protected bool stun;
    [Header("��Ѫ����")]
    public float stunEffect = 1;
    // ��Ѫ
    protected float bleedingTimer;
    protected float bleedingTickleTimer;
    public float bleedingTime = 5f;
    public float bleedingTickleTime = 1f;
    public int bleedingDamage = 5;
    // ����ѣ��
    protected float stunImmunityTimer;
    protected bool stunImmunity;
    [Header("�����˺�����")]
    // �����˺�
    protected int immunityTime;
    protected float immunityTimer;
    [Header("��������")]
    // ����
    protected float invisibleTimer;
    [Header("��������")]
    // ����
    protected float shield;
    protected float shieldTimer;
    [Header("�����ظ�����")]
    // �����ظ�
    protected float healingTotalTimer;
    protected float healingTimer;
    protected float healingTime;
    protected float healingValue;
    // ��ҡ����˻���

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
    }

    protected virtual void Start()
    {
        // ��������ʼ��Ѫ��
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
        HandlingBleeding();
    }

    public void SetMaxHealth(float m)
    {
        maxHealth = m;
        curHealth = m;
    }

    public virtual void Hurt(CardPrototype cardPrototype)
    {

    }

    public virtual void Hurt(float damage, bool shieldBreak=false, float damageIncrease=1)
    {
        // ���˽ӿ�
        // ����damage�˺���ֵ��shieldBreak�Ƿ�Ի������ˣ�damageIncrease���˱���
        float trueDamage;
        if (shield > 0)
        {
            // ������ʵ�˺�
            if (shieldBreak)
            {
                trueDamage = damageIncrease * damage;
            }
            else
            {
                trueDamage = damage;
            }

            // ������ʵ�˺�
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
            trueDamage = damage;
            curHealth -= damage;
        }
        if (GlobalValue.poisonAttack)
            Poisoning();

        BattleDataManager.instance.UpdateDamage(trueDamage);
        healthBarManager.UpdateHealth(curHealth / maxHealth);

        DamageText damageText = Instantiate(damageTextPrefab, GameObject.FindGameObjectWithTag("DamageCanvas").transform).GetComponent<DamageText>();
        damageText.Init(trueDamage,transform); 
    }

    public void PercentHurt(float percent, float max = Mathf.Infinity)
    {
        float trueDamage = percent * maxHealth;
        if (max <= trueDamage)
            Hurt(max);
        else
            Hurt(trueDamage);
    }

    public void Poisoning()
    {
        // �ж��ӿ�
        // ÿ�ε�������һ���ж���ˢ���ж�ʱ��
        poisonLevel += 1;
        poisonTotalTimer = poisonTime;
        poisonTimer = poisonDamageTime;
    }

    public void Bleeding()
    {
        bleedingTimer = bleedingTime;
        bleedingTickleTimer = bleedingTickleTime;
    }

    public void Stun(float time)
    {
        // ѣ�νӿ�
        // ����ѣ��ʱ��
        if (!stunImmunity)
        {
            stun = true;
            stunTimer = time * stunEffect;
        }
    }

    public bool IsStun()
    {
        return stun;
    }

    public void StunImmunity(float time)
    {
        // ����ѣ��time��
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
    // ��������ظ���ʱ��
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
    // �����ܼ�ʱ��
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
    // ����ѣ�μ�ʱ��
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
    // �����ж���ʱ��
    protected void HandlingPoison()
    {
        if (poisonLevel > 0)
        {
            poisonTimer -= Time.deltaTime;
            poisonTotalTimer -= Time.deltaTime;

            if (poisonTimer <= 0)
            {
                // �����˺���ˢ�½����ʱ��
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

    public float CurrentHealth()
    {
        return curHealth;
    }

    public float MaxHealth()
    {
        return maxHealth;
    }
}
