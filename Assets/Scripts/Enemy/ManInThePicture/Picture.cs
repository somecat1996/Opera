using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picture : GameObjectBase
{
    private int currentStage;

    public float stage1CoolingTime = 20f;
    public float stage1Damage = 100;
    private float stage1CoolingTimer;
    public int stage1CardNumber = 5;
    private int stage1CardCounter;

    public int stage2DamageNumber = 5;
    private int stage2DamageCounter;
    private CardPrototype lastUsedCard;
    public float stage2PlayerDamage = 2;
    public float stage2EnemyDamage = 200;

    private PlayerStatus player;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        currentStage = 1;

        stage1CoolingTimer = 0;
        stage1CardCounter = stage1CardNumber;
        stage2DamageCounter = stage2DamageNumber;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (currentStage == 1)
            Stage1();
        else if (currentStage == 2)
            Stage2();
    }

    public void Change()
    {
        currentStage = 2;
        lastUsedCard = BattleDataManager.instance.lastUsedCard;
    }

    private void Stage1()
    {
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

    public void UseAttackCard()
    {
        if (stage1CoolingTimer <= 0)
        {
            if (lastUsedCard.cardInfo.id != BattleDataManager.instance.lastUsedCard.cardInfo.id)
            {
                lastUsedCard = BattleDataManager.instance.lastUsedCard;
                if (lastUsedCard.CheckIfDamageCard())
                {
                    stage1CardCounter -= 1;
                    if (stage1CardCounter <= 0)
                    {
                        stage1CardCounter = stage1CardNumber;
                        stage1CoolingTimer = stage1CoolingTime;
                        EnemyManager.instance.HurtAll(stage1Damage);
                    }
                }
            }
        }
    }

    public override void Hurt(float damage, bool shieldBreak = false, float damageIncrease = 1)
    {
        if (currentStage == 2)
        {
            stage2DamageCounter -= 1;
            if (stage2DamageCounter <= 0)
            {
                stage2DamageCounter = stage2DamageNumber;
                EnemyManager.instance.HurtAll(stage2EnemyDamage);
                player.Hurt(stage2PlayerDamage);
            }
        }
    }
}
