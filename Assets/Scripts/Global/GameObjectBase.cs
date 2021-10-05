using System.Collections;
using System.Collections.Generic;
using UnityEngine;


interface GameObjectInterface
{
    // �˺��ӿ�
    // ����damage�˺���ֵ��shieldBreak�Ƿ�Ի������ˣ�damageIncrease���˱���
    public void Hurt(float damage, bool shieldBreak, float damageIncrease);
    public void Hurt(CardPrototype cardPrototype);
    // �ж��ӿڣ�ÿ�ε���ˢ���ж�ʱ�䡢�����ж�����
    public void Poisoning();
    // ѣ�νӿڣ����ú�ѣ��timeʱ�� 
    public void Stun(float time);
    // �����Ƿ�ѣ��
    public bool IsStun();
    // ��������
    public void StunImmunity(float time);
    // ˲�����ƽӿڣ��ظ�healingValueѪ��
    public void InstantHealing(float healingValue);
    // �������ƽӿ�
    // time����ʱ�䣬tickle����ʱ�䣬valueÿ�ν���ظ�Ѫ��
    public void ContinuousHealing(float time, float tickle, float value);
    // ���ӻ���
    // shieldValue����ֵ��time���ܳ���ʱ��
    public void AddShield(float shieldValue, float time);
}

public class GameObjectBase : MonoBehaviour, GameObjectInterface
{
    public float maxHealth;
    public Vector3 offsetPos;
    public GameObject healthBarPrefab;
    // �ж�����ʱ��
    public int poisonTime = 5;
    // �ж��˺�
    public int poisonDamage = 5;
    // �ж�����ʱ��
    public int poisonDamageTime = 1;

    protected float curHealth;
    protected HealthBarManager healthBarManager;
    // �ж�����ʱ���ʱ��
    private float poisonTotalTimer;
    // �ж��˺������ʱ��
    private float poisonTimer;
    // �ж�����
    private int poisonLevel;
    // ѣ��
    private float stunTimer;
    private bool stun;
    // ����ѣ��
    private float stunImmunityTimer;
    private bool stunImmunity;
    // ����
    private float shield;
    private float shieldTimer;
    // �����ظ�
    private float healingTotalTimer;
    private float healingTimer;
    private float healingTime;
    private float healingValue;
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
    }

    public virtual void Hurt(CardPrototype cardPrototype)
    {

    }

    public virtual void Hurt(float damage, bool shieldBreak=false, float damageIncrease=0)
    {
        // ���˽ӿ�
        // ����damage�˺���ֵ��shieldBreak�Ƿ�Ի������ˣ�damageIncrease���˱���
        if (shield > 0)
        {
            // ������ʵ�˺�
            float trueDamage;
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
            curHealth -= damage;
        }
        healthBarManager.UpdateHealth(curHealth / maxHealth);
    }

    public void Poisoning()
    {
        // �ж��ӿ�
        // ÿ�ε�������һ���ж���ˢ���ж�ʱ��
        poisonLevel += 1;
        poisonTotalTimer = poisonTime;
        poisonTimer = poisonDamageTime;
    }

    public void Stun(float time)
    {
        // ѣ�νӿ�
        // ����ѣ��ʱ��
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
        // ����ѣ��time��
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
