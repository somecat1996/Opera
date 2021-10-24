using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emperor : EnemyStatus, SummonEnemy, BossInterface
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

    // ÕÙ»½´ó³¼
    [Header("Summon Minister")]
    public GameObject ministerPrefab;
    public float ministerSummonTime = 5f;
    private float ministerSummonTimer;

    // ÈÓ³öÊ¥Ö¼
    [Header("Throw Imperial Decree")]
    public float imperialDecreeThrowTime = 10f;
    private float imperialDecreeThrowTimer;
    public AudioClip imperialDecreeSound;

    // »Ó½£
    [Header("Swing Sword")]
    public float swordAttackTime = 5f;
    public float swordAttackIncrease = 2f;
    private float swordAttackTimer;
    private bool swordAttack;
    public AudioClip swordSound;

    // »­¾íprefab
    [Header("Picture Prefab")]
    public GameObject picturePrefab;
    private Picture picture;

    // ¶Ô»°¿ò
    public GameObject lineTextPrefab;
    public float speakTime = 20f;
    private float speakTimer;
    public string[] stage1Lines;
    public string stage1To2Line;
    public string stage2To3Line;

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
        ministerSummonTimer = ministerSummonTime;
        imperialDecreeThrowTimer = imperialDecreeThrowTime;
        swordAttackTimer = swordAttackTime;
        swordAttack = false;

        currentStage = 1;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();

        countHurt = false;
        hurtCounter = 0;
        douETimer = 0;

        speakTimer = speakTime;

        shieldTimer = shieldTime;

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
        animator.SetTrigger("Hurt");
        shadowAnimator.SetTrigger("Hurt");
        base.Hurt(damage, shieldBreak, damageIncrease, type);
        if (shieldScript && shield <= 0)
        {
            shieldScript.DestoryObject();
            shieldScript = null;
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
        SummonPicture();
        Speak(stage1To2Line);
        BattleDataManager.instance.UpdateStage(2);
    }

    private void Stage3Start()
    {
        currentStage = 3;
        PictureChange();
        EnemyManager.instance.RemoveMinions();
        Speak(stage2To3Line);
        BattleDataManager.instance.UpdateStage(3);
    }

    private void Stage1()
    {
        NormalAttack();
        BananaAttack();
        AddShield();
        SummonMinister();

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
        SummonMinister();
        ThrowImperialDecree();
    }

    private void Stage3()
    {
        NormalAttack();
        BananaAttack();
        AddShield();
        SwingSword();
    }

    private void SummonMinister()
    {
        ministerSummonTimer -= Time.deltaTime;
        if (ministerSummonTimer <= 0)
        {
            ministerSummonTimer = ministerSummonTime;
            SummonMinion(ministerPrefab);
        }
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
        float damage = normalAttackDamage[normalAttackTime - normalAttackTimeMin] * EnemyManager.instance.EnemyAttackCoefficient();
        if (normalAttackTimer <= 0)
        {
            normalAttackTime = NormalAttackTime();
            normalAttackTimer = normalAttackTime;
            if (swordAttack)
            {
                swordAttack = false;
                player.Hurt(damage * swordAttackIncrease);
            }
            else
                player.Hurt(damage);
        }
    }

    private int NormalAttackTime()
    {
        return Random.Range(normalAttackTimeMin, normalAttackTimeMax + 1);
    }

    private void BananaAttack()
    {
        animator.SetTrigger("Banana");
        shadowAnimator.SetTrigger("Banana");
        bananaAttackTimer -= Time.deltaTime;
        if (bananaAttackTimer <= 0)
        {
            bananaAttackTimer = bananaAttackTime;
            // player.Hurt(bananaAttackDamage);
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
            animator.SetBool("Shield", true);
            shadowAnimator.SetBool("Shield", true);
            AudioManager.instance.PlaySound(shieldSound);
            shieldTimer = shieldTime;
            if (shield <= 0)
                shieldScript = EffectsManager.instance.CreateEffectFollow(1, Mathf.Infinity, transform, new Vector3(0, 0.48f, 0.25f)).GetComponent<EffectFollow>();
            AddShield(shieldValue, Mathf.Infinity);
        }
    }

    private void ThrowImperialDecree()
    {
        imperialDecreeThrowTimer -= Time.deltaTime;
        if (imperialDecreeThrowTimer <= 0)
        {
            imperialDecreeThrowTimer = imperialDecreeThrowTime;
            AudioManager.instance.PlaySound(imperialDecreeSound);
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Minister minister = enemy.GetComponent<Minister>();
                if (minister)
                    minister.SummonMinion(minister.soldierPrefab, 4);
            }
        }
    }

    private void SummonPicture()
    {
        GameObject tmp = EnemyManager.instance.SummonInMiddle(picturePrefab);
        picture = tmp.GetComponent<Picture>();
    }

    private void PictureChange()
    {
        picture.Change();
    }

    private void SwingSword()
    {
        animator.SetTrigger("Sword");
        shadowAnimator.SetTrigger("Sword");
        swordAttackTimer -= Time.deltaTime;
        if (swordAttackTimer <= 0)
        {
            AudioManager.instance.PlaySound(swordSound);
            swordAttackTimer = swordAttackTime;
            swordAttack = true;
        }
    }

    public override void Die()
    {
        animator.SetTrigger("Die");
        if (picture)
            Destroy(picture.gameObject);
        EnemyManager.instance.FinishLevel(true);
        base.Die();
    }

    public override void Kill()
    {
        animator.SetTrigger("Die");
        if (picture)
            Destroy(picture.gameObject);
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
