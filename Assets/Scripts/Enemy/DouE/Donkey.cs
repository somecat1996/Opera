using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Donkey : EnemyStatus, SummonEnemy, BossInterface
{
    [Header("Stage Configuration")]
    public float stage2Start = 0.7f;
    public float stage3Start = 0.3f;

    // ��ͨ����
    [Header("Normal Attack")]
    public int normalAttackTime = 1;
    public int normalAttackDamage = 1;
    private float normalAttackTimer;

    // �㽶Ƥ����
    [Header("Banana Attack")]
    public float bananaAttackTime = 10f;
    public float bananaAttackDamage = 3f;
    private float bananaAttackTimer;

    // ����
    [Header("Shield")]
    public float shieldTime = 30f;
    public float shieldValue = 300;

    // ��ˮ����
    [Header("Dirty Water Attack")]
    public float dirtyWaterAttackTime = 10f;
    public float dirtyWaterAttackDamage = 3f;
    private float dirtyWaterAttackTimer;

    // �о�С��Prefab
    [Header("Minions")]
    public GameObject xiaominPrefab;
    public GameObject xianguanPrefab;

    // ��ҩprefab
    [Header("Medicine Prefab")]
    public GameObject medicinePrefab;
    private Medicine medicine;

    public Vector3 pushPosition;
    public float pushDamage = 3f;
    public float pushTime = 10f;
    private float pushTimer;

    // �Ի���
    public GameObject lineTextPrefab;
    public float speakTime = 20f;
    private float speakTimer;
    public string[] stage1Lines;
    public string stage1To2Line;
    public string stage2To3Line;

    // ��ԩ�ӿ�
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
        base.Hurt(damage, shieldBreak, damageIncrease, type);
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
    }

    private void Stage3Start()
    {
        currentStage = 3;
        Speak(stage2To3Line);
        SummonXianguan();
        MedicineChange();
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

    private void DirtyWaterAttack()
    {
        dirtyWaterAttackTimer -= Time.deltaTime;
        if (dirtyWaterAttackTimer <= 0)
        {
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
}
