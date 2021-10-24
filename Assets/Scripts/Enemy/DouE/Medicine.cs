using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medicine : GameObjectBase, LevelItemInterface
{
    private int currentStage;

    public float stage1ImmunityTime = 5f;
    public int stage1WalkNumber = 5;
    private int stage1WalkCounter;
    private float stage1ImmunityTimer;
    public float stage1DamageIncrease = 0.3f;

    public float stage2CoolingTime = 20f;
    public float stage2Time = 8f;
    public int stage2Num = 6;
    private bool sunny;
    public float stage2HeartIncreaseRate = 1f;
    public float stage2DamageIncreaseRate = 0.5f;
    private CardPrototype lastUsedCard;
    private float stage2CoolingTimer;
    public float stage2SnowyTimer;
    public float stage2SunnyTimer;
    private int stage2Counter;

    public GameObject medicine;
    public GameObject doll;

    private PlayerStatus player;
    // Start is called before the first frame update
    protected override void Start()
    {
        // 不创建血条
        currentStage = 1;

        stage1WalkCounter = stage1WalkNumber;
        stage1ImmunityTimer = 0;
        stage2CoolingTimer = 0;
        stage2SnowyTimer = 0;
        stage2SunnyTimer = 0;
        stage2Counter = stage2Num;
        sunny = true;

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

        // test
        if (Input.GetKeyDown(KeyCode.M))
        {
            Stage2Attack();
        }
    }

    public void Change()
    {
        currentStage = 2;
        medicine.SetActive(false);
        doll.SetActive(true);
        lastUsedCard = BattleDataManager.instance.lastUsedCard;
        if (stage1ImmunityTimer > 0)
        {
            GlobalValue.damageIncrement_General -= stage1DamageIncrease;
        }
    }

    private void Stage1()
    {
        if (stage1ImmunityTimer > 0)
        {
            stage1ImmunityTimer -= Time.deltaTime;
            if (stage1ImmunityTimer <= 0)
            {
                GlobalValue.damageIncrement_General -= stage1DamageIncrease;
            }
        }
    }

    private void Stage2()
    {
        if (stage2CoolingTimer > 0)
            stage2CoolingTimer -= Time.deltaTime;
        if (stage2CoolingTimer <= 0)
        {
            if (!lastUsedCard || !BattleDataManager.instance.lastUsedCard || lastUsedCard != BattleDataManager.instance.lastUsedCard)
            {
                lastUsedCard = BattleDataManager.instance.lastUsedCard;
                if (lastUsedCard && !lastUsedCard.CheckIfDamageCard())
                {
                    stage2Counter -= 1;
                    Debug.Log(stage2Counter);
                    if (stage2Counter <= 0)
                    {
                        Stage2Attack();
                        stage2Counter = stage2Num;
                        stage2CoolingTimer = stage2CoolingTime;
                    }
                }
            }
        }

        if (stage2SunnyTimer > 0)
        {
            stage2SunnyTimer -= Time.deltaTime;
            if (stage2SunnyTimer <= 0)
            {
                stage2SunnyTimer = 0;
                StopIncreasing();

                Debug.Log("晴天结束！");
            }
        }

        if (stage2SnowyTimer > 0)
        {
            stage2SnowyTimer -= Time.deltaTime;
            foreach (GameObject tmp in GameObject.FindGameObjectsWithTag("SummonedObject"))
            {
                if (tmp.GetComponent<DirtyWater>())
                    Destroy(tmp);
            }
            if (stage2SnowyTimer <= 0)
            {
                stage2SnowyTimer = 0;

                Debug.Log("雪天结束！");
            }
        }
    }

    public override void Hurt(float damage, bool shieldBreak = false, float damageIncrease = 1, HurtType type = HurtType.None)
    {

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
        player.ImmunityByDuration(stage1ImmunityTime);
        stage1ImmunityTimer = stage1ImmunityTime;
        GlobalValue.damageIncrement_General += stage1DamageIncrease;

        Debug.Log("解药！");
    }

    private void Stage2Attack()
    {
        if (sunny)
        {
            sunny = false;
            StartIncreasing();
            stage2SunnyTimer = stage2Time;

            Debug.Log("晴天！");
        }
        else
        {
            sunny = true;
            stage2SnowyTimer = stage2Time;

            Debug.Log("雪天！");
        }
    }

    public void Walk() 
    {
        if (currentStage == 1)
        {
            stage1WalkCounter -= 1;
            if (stage1WalkCounter <= 0)
            {
                stage1WalkCounter = stage1WalkNumber;
                Stage1Attack();
            }
        }
    }

    public void StartIncreasing()
    {
        PlayerManager.instance.ChangeRecoverySpeed_PowerPoint(stage2HeartIncreaseRate);
        GlobalValue.damageIncrement_General += stage2DamageIncreaseRate;
    }

    public void StopIncreasing()
    {
        PlayerManager.instance.ChangeRecoverySpeed_PowerPoint(-stage2HeartIncreaseRate);
        GlobalValue.damageIncrement_General -= stage2DamageIncreaseRate;
    }

    private void OnDestroy()
    {
        if (stage2SunnyTimer > 0)
            StopIncreasing();
    }
}
