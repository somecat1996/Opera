using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : GameObjectBase, LevelItemInterface
{
    private int currentStage;

    public float stage1CoolingTime = 20f;
    public float stage1Damage = 200;
    private float stage1CoolingTimer;
    public int stage1CardNumber = 5;
    private int stage1CardCounter;
    public AudioClip stage1Sound;

    public int stage2CardNumber = 8;
    private int stage2CardCounter;
    private CardPrototype lastUsedCard;
    private WesternQueen westernQueen;
    public AudioClip stage2Sound;

    public GameObject cow;
    public GameObject magpie;

    private PlayerStatus player;
    // Start is called before the first frame update
    protected override void Start()
    {
        // 不创建血条
        currentStage = 1;

        stage1CoolingTimer = 0;
        stage1CardCounter = stage1CardNumber;
        stage2CardCounter = stage2CardNumber;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
    }

    public void Instantiate(WesternQueen q)
    {
        westernQueen = q;
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
        cow.SetActive(false);
        magpie.SetActive(true);
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
        if (!lastUsedCard || !BattleDataManager.instance.lastUsedCard || lastUsedCard != BattleDataManager.instance.lastUsedCard)
        {
            lastUsedCard = BattleDataManager.instance.lastUsedCard;
            if (lastUsedCard)
            {
                stage2CardCounter -= 1;
                if (stage2CardCounter <= 0)
                {
                    stage2CardCounter = stage2CardNumber;
                    Stage2Attack();
                }
            }
        }
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
        GameObject tmp = EnemyManager.instance.RandomChoose();
        tmp.GetComponent<EnemyStatus>().Hurt(stage1Damage);
    }

    private void Stage2Attack()
    {
        AudioManager.instance.PlaySound(stage2Sound);
        westernQueen.StopSoliderReborn();
    }

    public void Walk() { }
}
