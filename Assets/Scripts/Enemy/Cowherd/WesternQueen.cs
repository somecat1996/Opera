using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WesternQueen : EnemyStatus, BossInterface
{
    [Header("Stage Configuration")]
    public float stage2Start = 0.7f;
    public float stage3Start = 0.3f;

    // 普通攻击
    [Header("Normal Attack")]
    public int normalAttackTimeMin = 1;
    public int normalAttackTimeMax = 5;
    public int[] normalAttackDamage;
    private float normalAttackTimer;
    private int normalAttackTime;

    // 香蕉皮攻击
    [Header("Banana Attack")]
    public float bananaAttackTime = 10f;
    public float bananaAttackDamage = 3f;
    private float bananaAttackTimer;

    // 护盾
    [Header("Shield")]
    public float shieldTime = 30f;
    public float shieldValue = 300;
    private EffectFollow shieldScript;
    public AudioClip shieldSound;

    // 雷劫攻击
    [Header("Thunder Attack")]
    public float thunderAttackTime = 15f;
    public float thunderTickTime = 0.5f;
    public int thunderCount = 10;
    public float thunderAttackDamage = 0.2f;
    public float thunderLockTime = 0.75f;
    public int thunderDiscardNum = 2;
    public float thunderHeartDamage = 1;
    private float thunderAttackTimer;
    private float thunderTickTimer;
    private int thunderCounter;
    public AudioClip thunderSound;

    // 天王Prefab
    [Header("Generals")]
    public float summonChance = 0.1f;
    public GameObject[] generalPrefabs;
    private List<int> summonedGeneral;
    private List<int> generalPosition;
    public GameObject heavenSolider;
    private List<int> heavenSoliderPosition;
    private List<int> aliveHeavenSoliderPosition;

    // 牛prefab
    [Header("Cow Prefab")]
    public GameObject cowPrefab;
    private Cow cow;

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

    private List<GameObject> effects;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        healthBarManager.gameObject.SetActive(false);

        normalAttackTime = NormalAttackTime();
        normalAttackTimer = normalAttackTime;
        bananaAttackTimer = bananaAttackTime;

        currentStage = 1;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();

        countHurt = false;
        hurtCounter = 0;
        douETimer = 0;

        speakTimer = speakTime;

        shieldTimer = shieldTime;
        thunderAttackTimer = thunderAttackTime;
        thunderTickTimer = 0;
        thunderCounter = 0;

        summonedGeneral = new List<int>() { 0, 1, 2, 3 };
        generalPosition = new List<int> { 0, 1, 2, 3, 5 };

        heavenSoliderPosition = new List<int> { 6, 7, 8, 9, 10, 11 };
        aliveHeavenSoliderPosition = new List<int>();

        hurtCoefficient = 1;
        damageCoefficient = 1;

        BattleDataManager.instance.UpdateStage(1);
        effects = new List<GameObject>();
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
        if (0.5f > Random.Range(0f, 1f))
        {
            animator.SetTrigger("Hurt1");
            shadowAnimator.SetTrigger("Hurt1");
        }
        else
        {
            animator.SetTrigger("Hurt2");
            shadowAnimator.SetTrigger("Hurt2");
        }
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
            animator.SetBool("Shield", false);
            shadowAnimator.SetBool("Shield", false);
        }

        SummonGeneral();

        if (GlobalValue.poisonAttack)
            Poisoning();

        if (voodooTimer > 0)
            voodooHurt += trueDamage;

        if (shieldScript && shield <= 0)
        {
            shieldScript.DestoryObject();
            shieldScript = null;
        }

        BattleDataManager.instance.UpdateDamage(trueDamage);
        if (healthBarManager.gameObject.activeInHierarchy)
            healthBarManager.UpdateHealth(curHealth / maxHealth);

        var col = gameObject.GetComponent<Collider>();
        var topAhcor = new Vector3(col.bounds.center.x, col.bounds.max.y, col.bounds.center.z);
        DamageText damageText = Instantiate(damageTextPrefab, GameObject.FindGameObjectWithTag("DamageCanvas").transform, critical).GetComponent<DamageText>();
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

    private void SkillAnimation()
    {
        if (0.5f > Random.Range(0f, 1f))
        {
            animator.SetTrigger("Skill1");
            shadowAnimator.SetTrigger("Skill1");
        }
        else
        {
            animator.SetTrigger("Skill2");
            shadowAnimator.SetTrigger("Skill2");
        }
    }

    private void Stage2Start()
    {
        currentStage = 2;
        Speak(stage1To2Line);
        SummonCow();
        BattleDataManager.instance.UpdateStage(2);
    }

    private void Stage3Start()
    {
        currentStage = 3;
        Speak(stage2To3Line);
        CowChange();
        SummonSolider();
        BattleDataManager.instance.UpdateStage(3);
    }

    private void Stage1()
    {
        NormalAttack();
        BananaAttack();
        AddShield();
        ThunderAttack();

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
        ThunderAttack();
    }

    private void Stage3()
    {
        NormalAttack();
        BananaAttack();
        AddShield();
        ThunderAttack();
    }

    public void SummonSolider()
    {
        foreach (int i in heavenSoliderPosition)
        {
            SkillAnimation();
            EnemyManager.instance.SummonMinionAt(heavenSolider, i);
            aliveHeavenSoliderPosition.Add(i);
        }
    }

    public void StopSoliderReborn()
    {
        if (aliveHeavenSoliderPosition.Count > 0)
        {
            int index = Random.Range(0, aliveHeavenSoliderPosition.Count / 3);
            int offset = aliveHeavenSoliderPosition.Count / 2;

            effects.Add(EffectsManager.instance.CreateEffect(16, Mathf.Infinity, (EnemyManager.instance.generationPoint[aliveHeavenSoliderPosition[index * 2 + offset]].position + EnemyManager.instance.generationPoint[aliveHeavenSoliderPosition[index * 2]].position) / 2, Vector3.zero).gameObject);
            
            EnemyManager.instance.StopRebornAt(aliveHeavenSoliderPosition[index * 2 + offset]);
            aliveHeavenSoliderPosition.RemoveAt(index * 2 + offset);
            EnemyManager.instance.StopRebornAt(aliveHeavenSoliderPosition[index * 2]);
            aliveHeavenSoliderPosition.RemoveAt(index * 2);
        }
    }

    private void SummonGeneral()
    {
        if (currentStage == 2 && summonedGeneral.Count > 0 && summonChance > Random.Range(0f, 1f))
        {
            SkillAnimation();
            int index = Random.Range(0, summonedGeneral.Count);
            int positionIndex = Random.Range(0, generalPosition.Count);
            EnemyManager.instance.SummonMinionAt(generalPrefabs[summonedGeneral[index]], generalPosition[positionIndex]);
            summonedGeneral.RemoveAt(index);
            generalPosition.RemoveAt(positionIndex);
        }
    }

    private void NormalAttack()
    {
        normalAttackTimer -= Time.deltaTime;
        float damage = normalAttackDamage[normalAttackTime - normalAttackTimeMin] * EnemyManager.instance.EnemyAttackCoefficient();
        if (normalAttackTimer <= 0)
        {
            normalAttackTime = NormalAttackTime();
            normalAttackTimer = normalAttackTime;
            player.Hurt(damage);
        }
    }

    private int NormalAttackTime()
    {
        return Random.Range(normalAttackTimeMin, normalAttackTimeMax + 1);
    }

    private void BananaAttack()
    {
        //animator.SetTrigger("Banana");
        bananaAttackTimer -= Time.deltaTime;
        if (bananaAttackTimer <= 0)
        {
            animator.SetTrigger("Banana");
            shadowAnimator.SetTrigger("Banana");
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
            SkillAnimation();
            animator.SetBool("Shield", true);
            shadowAnimator.SetBool("Shield", true);
            AudioManager.instance.PlaySound(shieldSound);
            shieldTimer = shieldTime;
            if (shield <= 0)
                shieldScript = EffectsManager.instance.CreateEffectFollow(1, Mathf.Infinity, transform, new Vector3(0, 1.28f, -0.42f)).GetComponent<EffectFollow>();
            AddShield(shieldValue, Mathf.Infinity);
        }
    }

    private void ThunderAttack()
    {
        thunderAttackTimer -= Time.deltaTime;
        if (thunderAttackTimer <= 0)
        {
            EffectsManager.instance.CreateEffectFollowPlayer(3, thunderCount * thunderTickTime, Vector3.zero);
            AudioManager.instance.PlaySound(thunderSound);
            SkillAnimation();
            thunderAttackTimer = thunderAttackTime;
            thunderTickTimer = thunderTickTime;
            thunderCounter = thunderCount;
            if (0.5 > Random.Range(0f, 1f))
            {
                CardManager.instance.LockCards(thunderLockTime);
            }
            else
            {
                CardManager.instance.DiscardCardRandomly(thunderDiscardNum);
                PlayerManager.instance.ChangePowerPoint(thunderHeartDamage);
            }
        }

        if (thunderCounter > 0)
        {
            thunderTickTimer -= Time.deltaTime;
            if (thunderTickTimer <= 0)
            {
                thunderCounter -= 1;
                thunderTickTimer = thunderTickTime;
                player.Hurt(thunderAttackDamage * EnemyManager.instance.EnemyAttackCoefficient());
            }
        }
    }

    private void SummonCow()
    {
        GameObject tmp = EnemyManager.instance.SummonInMiddle(cowPrefab);
        cow = tmp.GetComponent<Cow>();
        cow.Instantiate(this);
    }

    private void CowChange()
    {
        cow.Change();
    }

    public override void Die()
    {
        //animator.SetTrigger("Die");
        if (cow)
            Destroy(cow.gameObject);
        EnemyManager.instance.FinishLevel(true);
        foreach (GameObject tmp in effects)
            tmp.GetComponent<EffectFollow>().DestoryObject();
        base.Die();
    }

    public override void Kill()
    {
        if (cow)
            Destroy(cow.gameObject);
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
