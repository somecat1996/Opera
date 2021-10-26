using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Donkey : EnemyStatus, SummonEnemy, BossInterface
{
    [Header("Stage Configuration")]
    public float stage2Start = 0.7f;
    public float stage3Start = 0.3f;

    // ÆÕÍ¨¹¥»÷
    [Header("Normal Attack")]
    public int normalAttackTimeMin = 1;
    public int normalAttackTimeMax = 5;
    public int[] normalAttackDamage;
    private float normalAttackTimer;
    private int normalAttackTime;

    // Ïã½¶Æ¤¹¥»÷
    [Header("Banana Attack")]
    public float bananaAttackTime = 10f;
    public float bananaAttackDamage = 3f;
    private float bananaAttackTimer;

    // »¤¶Ü
    [Header("Shield")]
    public float shieldTime = 30f;
    public float shieldValue = 300;
    private EffectFollow shieldScript;
    public AudioClip shieldSound;

    // ÎÛË®¹¥»÷
    [Header("Dirty Water Attack")]
    public float dirtyWaterAttackTime = 10f;
    public float dirtyWaterAttackDamage = 3f;
    private float dirtyWaterAttackTimer;
    public AudioClip dirtyWaterSound;

    // ÊÐ¾®Ð¡ÃñPrefab
    [Header("Minions")]
    public GameObject xiaominPrefab;
    public GameObject xianguanPrefab;

    // ½âÒ©prefab
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

    // ¶Ô»°¿ò
    public GameObject lineTextPrefab;
    public float speakTime = 20f;
    private float speakTimer;
    public string[] stage1Lines;
    public string stage1To2Line;
    public string stage2To3Line;

    public GameObject doutianzhang;
    public Vector3 offset;
    private GameObject dou;

    // ñ¼¶ðÔ©½Ó¿Ú
    private bool countHurt;
    private int hurtCounter;
    private float douETimer;
    public float douETime = 10f;
    public float douECoefficient = 2f;

    private int currentStage;
    private PlayerStatus player;
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
        dirtyWaterAttackTimer = dirtyWaterAttackTime;
        pushTimer = pushTime;

        BattleDataManager.instance.UpdateStage(1);
        GUIManager.instance.UpdateBossHealthPoint(1, curHealth);
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
        base.Hurt(damage, shieldBreak, damageIncrease, type);
        if (shieldScript && shield <= 0)
        {
            shieldScript.DestoryObject();
            shieldScript = null;
        }
        BattleDataManager.instance.UpdateBossHP(curHealth / maxHealth, Mathf.Max(0, curHealth));
        if (countHurt)
            hurtCounter += 1;
        if (curHealth <= stage2Start * maxHealth && currentStage == 1)
        {
            BattleDataManager.instance.UpdateStage(2);
            currentStage = 2;
            Curtain.instance.SetCallbackFun_Open(Stage2Start);
        }
        if (curHealth <= stage3Start * maxHealth && currentStage == 2)
        {
            BattleDataManager.instance.UpdateStage(3);
            currentStage = 3;
            Curtain.instance.SetCallbackFun_Open(Stage3Start);
        }
    }

    private void Stage2Start()
    {
        Speak(stage1To2Line);
        SummonMedicine();
    }

    private void Stage3Start()
    {
        Speak(stage2To3Line);
        SummonXianguan();
        MedicineChange();
        Curtain.instance.SetCallbackFun_Open(null);
    }

    private void Stage1()
    {
        NormalAttack();
        BananaAttack();
        AddShield();
        DirtyWaterAttack();

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
        DirtyWaterAttack();
        PushAttack();
    }

    private void Stage3()
    {
        NormalAttack();
        BananaAttack();
        AddShield();
        DirtyWaterAttack();
        PushAttack();

        if (stage3TimeLimit > 0)
        {
            stage3TimeLimit -= Time.deltaTime;
            if (stage3TimeLimit <= 0)
                dou = Instantiate(doutianzhang, pushPosition + offset, Quaternion.identity);
        }
    }

    public void SummonMinion(GameObject minion, int number = 1)
    {
        animator.SetTrigger("Summon");
        shadowAnimator.SetTrigger("Summon");
        for (int i = 0; i < number; i++)
        {
            EnemyManager.instance.SummonMinion(minion);
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
            animator.SetTrigger("Shield");
            shadowAnimator.SetTrigger("Shield");
            AudioManager.instance.PlaySound(shieldSound);
            shieldTimer = shieldTime;
            if (shield <= 0)
                shieldScript = EffectsManager.instance.CreateEffectFollow(1, Mathf.Infinity, transform, new Vector3(0, 0.99f, -0.47f)).GetComponent<EffectFollow>();
            AddShield(shieldValue, Mathf.Infinity);
        }
    }

    private void DirtyWaterAttack()
    {
        dirtyWaterAttackTimer -= Time.deltaTime;
        if (dirtyWaterAttackTimer <= 0)
        {
            animator.SetTrigger("DirtyWater");
            shadowAnimator.SetTrigger("DirtyWater");
            AudioManager.instance.PlaySound(dirtyWaterSound);
            dirtyWaterAttackTimer = dirtyWaterAttackTime;
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            Vector3 position = new Vector3(Random.Range(playerMovement.moveAera[0].position.x, playerMovement.moveAera[1].position.x), 0, Random.Range(playerMovement.moveAera[0].position.z, playerMovement.moveAera[1].position.z));
            SummonedObjectManager.instance.SummonDirtyWater(position, this);
        }
    }

    private void PushAttack()
    {
        pushTimer -= Time.deltaTime;
        if (pushTimer <= 0)
        {
            animator.SetTrigger("Push");
            shadowAnimator.SetTrigger("Push");
            pushTimer = pushTime;
            player.PushTo(pushPosition, pushDamage * EnemyManager.instance.EnemyAttackCoefficient());
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
        EnemyManager.instance.FinishLevel(true);
        Kill();
    }

    public override void Kill()
    {
        if (medicine)
            Destroy(medicine.gameObject);
        if (dou)
            Destroy(dou);
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
}
