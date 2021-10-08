using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emperor : EnemyStatus, SummonEnemy
{
    [Header("Stage Configuration")]
    public float stage2Start = 0.7f;
    public float stage3Start = 0.3f;

    // ÆÕÍ¨¹¥»÷
    [Header("Normal Attack")]
    public float normalAttackTime = 1f;
    public float normalAttackDamage = 1f;
    private float normalAttackTimer;

    // Ïã½¶Æ¤¹¥»÷
    [Header("Banana Attack")]
    public float bananaAttackTime = 10f;
    public float bananaAttackDamage = 3f;
    private float bananaAttackTimer;

    // ÕÙ»½´ó³¼
    [Header("Summon Minister")]
    public GameObject ministerPrefab;
    public float ministerSummonTime = 5f;
    private float ministerSummonTimer;

    // ÈÓ³öÊ¥Ö¼
    [Header("Throw Imperial Decree")]
    public float imperialDecreeThrowTime = 10f;
    private float imperialDecreeThrowTimer;

    // »Ó½£
    [Header("Swing Sword")]
    public float swordAttackTime = 5f;
    public float swordAttackDamage = 3f;
    private float swordAttackTimer;

    // »­¾íprefab
    [Header("Picture Prefab")]
    public GameObject picturePrefab;
    private Picture picture;

    // ¶Ô»°¿ò

    private int currentStage;
    private PlayerStatus player;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        normalAttackTimer = normalAttackTime;
        bananaAttackTimer = bananaAttackTime;
        ministerSummonTimer = ministerSummonTime;
        imperialDecreeThrowTimer = imperialDecreeThrowTime;
        swordAttackTimer = swordAttackTime;

        currentStage = 1;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (currentStage == 1)
            Stage1();
        else if (currentStage == 2)
            Stage2();
        else if (currentStage == 3)
            Stage3();
    }

    public override void Hurt(float damage, bool shieldBreak, float damageIncrease)
    {
        base.Hurt(damage, shieldBreak, damageIncrease);
        if (curHealth <= stage2Start * maxHealth && currentStage == 1)
            Stage2Start();
        if (curHealth <= stage3Start * maxHealth && currentStage == 2)
            Stage3Start();
    }

    private void Stage2Start()
    {
        currentStage = 2;
        SummonPicture();
    }

    private void Stage3Start()
    {
        currentStage = 3;
        PictureChange();
        EnemyManager.instance.RemoveMinions();
    }

    private void Stage1()
    {
        NormalAttack();
        BananaAttack();
        SummonMinister();
    }

    private void Stage2()
    {
        NormalAttack();
        BananaAttack();
        SummonMinister();
        ThrowImperialDecree();
    }

    private void Stage3()
    {
        NormalAttack();
        BananaAttack();
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
        if (normalAttackTimer <= 0)
        {
            normalAttackTimer = normalAttackTime;
            player.Hurt(normalAttackDamage);
        }
    }

    private void BananaAttack()
    {
        bananaAttackTimer -= Time.deltaTime;
        if (bananaAttackTimer <= 0)
        {
            bananaAttackTimer = bananaAttackTime;
            player.Hurt(bananaAttackDamage);
        }
    }

    private void ThrowImperialDecree()
    {
        imperialDecreeThrowTimer -= Time.deltaTime;
        if (imperialDecreeThrowTimer <= 0)
        {
            imperialDecreeThrowTimer = imperialDecreeThrowTime;
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
        swordAttackTimer -= Time.deltaTime;
        if (swordAttackTimer <= 0)
        {
            swordAttackTimer = swordAttackTime;
            player.Hurt(swordAttackDamage);
        }
    }

    public override void Die()
    {
        Destroy(picture.gameObject);
        base.Die();
    }
}
