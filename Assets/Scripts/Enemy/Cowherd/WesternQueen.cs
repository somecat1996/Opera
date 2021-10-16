using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WesternQueen : EnemyStatus, SummonEnemy, BossInterface
{
    [Header("Stage Configuration")]
    public float stage2Start = 0.7f;
    public float stage3Start = 0.3f;

    // 普通攻击
    [Header("Normal Attack")]
    public int normalAttackTime = 1;
    public int normalAttackDamage = 1;
    private float normalAttackTimer;

    // 香蕉皮攻击
    [Header("Banana Attack")]
    public float bananaAttackTime = 10f;
    public float bananaAttackDamage = 3f;
    private float bananaAttackTimer;

    // 护盾
    [Header("Shield")]
    public float shieldTime = 30f;
    public float shieldValue = 300;

    // 雷劫攻击
    [Header("Thunder Attack")]
    public float dirtyWaterAttackTime = 15f;
    public float dirtyWaterAttackDamage = 3f;
    private float dirtyWaterAttackTimer;

    // 市井小民Prefab
    [Header("Minions")]
    public GameObject xiaominPrefab;
    public GameObject xianguanPrefab;

    // 解药prefab
    [Header("Medicine Prefab")]
    public GameObject medicinePrefab;
    private Medicine medicine;

    [Header("Push Attack")]
    public Vector3 pushPosition;
    public float pushDamage = 3f;
    public float pushTime = 10f;
    private float pushTimer;

    [Header("Stage 3 Time Limit")]
    public float stage3TimeLimit = 60f;

    // 对话框
    public GameObject lineTextPrefab;
    public float speakTime = 20f;
    private float speakTimer;
    public string[] stage1Lines;
    public string stage1To2Line;
    public string stage2To3Line;

    // 窦娥冤接口
    private bool countHurt;
    private int hurtCounter;
    private float douETimer;
    public float douETime = 10f;
    public float douECoefficient = 2f;

    private int currentStage;
    private PlayerStatus player;
    private float hurtCoefficient;
    private float damageCoefficient;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        normalAttackTimer = normalAttackTime;
        bananaAttackTimer = bananaAttackTime;

        currentStage = 1;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();

        countHurt = false;
        hurtCounter = 0;
        douETimer = 0;

        speakTimer = speakTime;

        shieldTimer = shieldTime;
        dirtyWaterAttackTimer = dirtyWaterAttackTime;
        pushTimer = pushTime;

        hurtCoefficient = 1;
        damageCoefficient = 1;

        BattleDataManager.instance.UpdateStage(1);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!EnemyManager.instance.pause)
        {
            base.Update();

            if (!stun)
            {
                if (countHurt && douETimer > 0)
                {
                    douETimer -= Time.deltaTime;
                    if (douETimer <= 0)
                    {
                        countHurt = false;
                        hurtCounter = 0;
                        douETimer = 0;
                        DouEAttack();
                    }
                }

                if (currentStage == 1)
                    Stage1();
                else if (currentStage == 2)
                    Stage2();
                else if (currentStage == 3)
                    Stage3();
            }
        }
    }

    public override void Hurt(float damage, bool shieldBreak = false, float damageIncrease = 1, HurtType type = HurtType.None)
    {
        //animator.SetTrigger("Hurt");
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

        trueDamage *= hurtCoefficient;

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
        healthBarManager.UpdateHealth(curHealth / maxHealth);

        var col = gameObject.GetComponent<Collider>();
        var topAhcor = new Vector3(col.bounds.center.x, col.bounds.max.y, col.bounds.center.z);
        DamageText damageText = Instantiate(damageTextPrefab, GameObject.FindGameObjectWithTag("DamageCanvas").transform).GetComponent<DamageText>();
        damageText.Init(trueDamage, topAhcor);
        base.Hurt(damage, shieldBreak, damageIncrease, type);
        if (curHealth <= 0)
        {
            Die();
        }
        BattleDataManager.instance.UpdateBossHP(curHealth / maxHealth);
        if (countHurt)
            hurtCounter += 1;
        if (curHealth <= stage2Start * maxHealth && currentStage == 1)
            Stage2Start();
        if (curHealth <= stage3Start * maxHealth && currentStage == 2)
            Stage3Start();
    }

    private void Stage2Start()
    {
        currentStage = 2;
        Speak(stage1To2Line);
        SummonMedicine();
        BattleDataManager.instance.UpdateStage(2);
    }

    private void Stage3Start()
    {
        currentStage = 3;
        Speak(stage2To3Line);
        SummonXianguan();
        MedicineChange();
        BattleDataManager.instance.UpdateStage(3);
    }

    private void Stage1()
    {
        NormalAttack();
        BananaAttack();
        AddShield();

        speakTimer -= Time.deltaTime;
        if (speakTimer <= 0)
        {
            speakTimer = speakTime;
            Speak(stage1Lines[Random.Range(0, stage1Lines.Length - 1)]);
        }
    }

    private void Stage2()
    {
        NormalAttack();
        BananaAttack();
        AddShield();
        PushAttack();
    }

    private void Stage3()
    {
        NormalAttack();
        BananaAttack();
        AddShield();
        PushAttack();

        stage3TimeLimit -= Time.deltaTime;
        if (stage3TimeLimit <= 0)
            Die();
    }

    public void SummonMinion(GameObject minion, int number = 1)
    {
        for (int i = 0; i < number; i++)
        {
            EnemyManager.instance.SummonMinion(minion);
        }
    }

    private void NormalAttack()
    {
        normalAttackTimer -= Time.deltaTime;
        if (normalAttackTimer <= 0)
        {
            normalAttackTimer = normalAttackTime;
            player.Hurt(normalAttackDamage);
        }
    }

    private void BananaAttack()
    {
        //animator.SetTrigger("Banana");
        bananaAttackTimer -= Time.deltaTime;
        if (bananaAttackTimer <= 0)
        {
            bananaAttackTimer = bananaAttackTime;
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            Vector3 position = new Vector3(Random.Range(playerMovement.moveAera[0].position.x, playerMovement.moveAera[1].position.x), 0, Random.Range(playerMovement.moveAera[0].position.z, playerMovement.moveAera[1].position.z));
            SummonedObjectManager.instance.SummonBanana(position, bananaAttackDamage);
        }
    }

    private void AddShield()
    {
        shieldTimer -= Time.deltaTime;
        if (shieldTimer <= 0)
        {
            shieldTimer = shieldTime;
            AddShield(shieldValue, Mathf.Infinity);
        }
    }

    private void PushAttack()
    {
        pushTimer -= Time.deltaTime;
        if (pushTimer <= 0)
        {
            pushTimer = pushTime;
            player.PushTo(pushPosition, pushDamage);
        }
    }

    public void SummonXiaomin()
    {
        EnemyManager.instance.SummonMinion(xiaominPrefab);
    }

    public void SummonXianguan()
    {
        EnemyManager.instance.SummonMinion(xianguanPrefab);
    }

    private void SummonMedicine()
    {
        GameObject tmp = EnemyManager.instance.SummonInMiddle(medicinePrefab);
        medicine = tmp.GetComponent<Medicine>();
    }

    private void MedicineChange()
    {
        medicine.Change();
    }

    public override void Die()
    {
        //animator.SetTrigger("Die");
        if (medicine)
            Destroy(medicine.gameObject);
        EnemyManager.instance.FinishLevel(true);
        base.Die();
    }

    public void DouE()
    {
        countHurt = true;
        hurtCounter = 0;
        douETimer = douETime;
    }

    private void DouEAttack()
    {
        Hurt(douECoefficient * hurtCounter);
    }

    private void Speak(string line)
    {
        var col = gameObject.GetComponent<Collider>();
        var topAhcor = new Vector3(col.bounds.center.x, col.bounds.max.y, col.bounds.center.z);
        Line lineText = Instantiate(lineTextPrefab, GameObject.FindGameObjectWithTag("LineCanvas").transform).GetComponent<Line>();
        lineText.Init(line, topAhcor);
    }

    public void ChangeHurtCoefficient(float value)
    {
        hurtCoefficient += value;
    }

    public void ChangeDamageCoefficient(float value)
    {
        damageCoefficient += value;
    }
}
