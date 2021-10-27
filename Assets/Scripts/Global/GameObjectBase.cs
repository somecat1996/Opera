using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

interface GameObjectInterface
{
    public void SetMaxHealth(float m);
    // �˺��ӿ�
    // ����damage�˺���ֵ��poison�Ƿ������Ĭ��false����shieldBreak�Ƿ�Ի������ˣ�Ĭ��false����damageIncrease���˱�����Ĭ��1��
    public void Hurt(float damage, bool shieldBreak, float damageIncrease, HurtType type);
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

public enum HurtType
{
    None = 0,
    Physic = 1,
    Magic = 2
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

    public float curHealth;
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
    public float shield;
    protected float shieldTimer;
    [Header("�����ظ�����")]
    // �����ظ�
    protected float healingTotalTimer;
    protected float healingTimer;
    protected float healingTime;
    protected float healingValue;
    // �׶�����
    protected float voodooProbability;
    protected float voodooCoolingTimer;
    protected float voodooTimer;
    protected float voodooHurt;
    public float voodooTime = 10f;
    public float voodoocoolingTime = 15f;

    protected MaterialController materialController;

    protected bool pause;
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

        pause = false;

        materialController = gameObject.GetComponent<MaterialController>();
    }

    protected virtual void Start()
    {
        // ��������ʼ��Ѫ��
        GameObject healthBar = Instantiate(healthBarPrefab, GameObject.FindGameObjectWithTag("HealthBarCanvas").transform);
        healthBarManager = healthBar.GetComponent<HealthBarManager>();

        healthBarManager.Init(transform, offsetPos);

        // ��ʼ��buff
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
        // ���˽ӿڣ�0-���ˣ�1-����2-ħ��
        // ����damage�˺���ֵ��shieldBreak�Ƿ�Ի������ˣ�damageIncrease���˱���
        float trueDamage;
        if (shield > 0 && shieldBreak)
            trueDamage = damageIncrease * damage;
        else
            trueDamage = damage;

        bool critical = false;
        // ���㱩��
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
        // �ж��ӿ�
        // ÿ�ε�������һ���ж���ˢ���ж�ʱ��
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
        // ѣ�νӿ�
        // ����ѣ��ʱ��
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
