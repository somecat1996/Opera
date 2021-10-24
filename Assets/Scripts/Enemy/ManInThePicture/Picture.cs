using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picture : GameObjectBase, LevelItemInterface
{
    private int currentStage;

    public float stage1CoolingTime = 20f;
    public float stage1Damage = 100;
    private float stage1CoolingTimer;
    public int stage1CardNumber = 5;
    private int stage1CardCounter;
    public AudioClip stage1Sound;

    public int stage2DamageNumber = 5;
    private int stage2DamageCounter;
    private CardPrototype lastUsedCard;
    public float stage2PercentDamage = 0.05f;
    public AudioClip stage2Sound;

    public GameObject picture;
    public GameObject clothes;

    private PlayerStatus player;
    // Start is called before the first frame update
    protected override void Start()
    {
        // 不创建血条
        currentStage = 1;

        stage1CoolingTimer = 0;
        stage1CardCounter = stage1CardNumber;
        stage2DamageCounter = stage2DamageNumber;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!EnemyManager.instance.pause)
        {
            if (currentStage == 1)
                Stage1();
            else if (currentStage == 2)
                Stage2();
        }
    }

    public void Change()
    {
        currentStage = 2;
        picture.SetActive(false);
        clothes.SetActive(true);
        lastUsedCard = BattleDataManager.instance.lastUsedCard;
    }

    private void Stage1()
    {
        AttackCardCheck();
        if (stage1CoolingTimer > 0)
        {
            stage1CoolingTimer -= Time.deltaTime;
            if (stage1CoolingTimer <= 0)
            {
                stage1CoolingTimer = 0;
            }
        }
    }

    private void Stage2()
    {

    }

    private void AttackCardCheck()
    {
        if (stage1CoolingTimer <= 0)
        {
            if (!lastUsedCard || !BattleDataManager.instance.lastUsedCard || lastUsedCard != BattleDataManager.instance.lastUsedCard)
            {
                lastUsedCard = BattleDataManager.instance.lastUsedCard;
                if (lastUsedCard && lastUsedCard.CheckIfDamageCard())
                {
                    stage1CardCounter -= 1;
                    if (stage1CardCounter <= 0)
                    {
                        stage1CardCounter = stage1CardNumber;
                        stage1CoolingTimer = stage1CoolingTime;
                        Stage1Attack();
                    }
                }
            }
        }
    }

    public override void Hurt(float damage, bool shieldBreak = false, float damageIncrease = 1, HurtType type = HurtType.None)
    {
        if (currentStage == 2)
        {
            stage2DamageCounter -= 1;
            if (stage2DamageCounter <= 0)
            {
                stage2DamageCounter = stage2DamageNumber;
                Stage2Attack();
            }
        }
    }

    public void Activate()
    {
        if (currentStage == 1)
            Stage1Attack();
        else if (currentStage == 2)
            Stage2Attack();
    }


    private void Stage1Attack()
    {
        AudioManager.instance.PlaySound(stage1Sound);
        picture.GetComponent<Animator>().SetTrigger("Open");
        EnemyManager.instance.HurtAll(stage1Damage);
    }

    private void Stage2Attack()
    {
        AudioManager.instance.PlaySound(stage2Sound);
        EnemyManager.instance.RemoveShieldAll();
        EnemyManager.instance.PercentHurtAll(stage2PercentDamage);
        if (player)
        {
            player.RemoveShield();
            player.PercentHurt(stage2PercentDamage);
        }
    }

    public void Walk() { }
}
