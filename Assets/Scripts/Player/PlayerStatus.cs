using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : GameObjectBase
{
    public PlayerMovement playerMovement;

    private float allDamage;
    private float magicDamage;
    private float physicDamage;

    private bool started;

    protected override void Awake()
    {
        base.Awake();
        allDamage = 0f;
        magicDamage = 0f;
        physicDamage = 0f;

        started = false;
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        PlayerManager.instance.SetPlayer(gameObject.GetComponent<GameObjectBase>());
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (started && !EnemyManager.instance.pause)
        {
            base.Update();
            HandlingImmunity();
            HandlingInvisible();
        }
    }

    public void StartPlaying()
    {
        started = true;
        playerMovement.StartMoving();
    }

    public void StopPlaying()
    {
        started = false;
        playerMovement.StopMoving();
    }

    public void RestartPlaying()
    {
        started = true;
        playerMovement.StartMovingAt();
        curHealth = maxHealth;
        healthBarManager.UpdateHealth(curHealth / maxHealth);
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        PlayerManager.instance.SetCurrentHealthPoint(curHealth);
    }

    public override void Hurt(float damage, bool shieldBreak = false, float damageIncrease = 1, HurtType type = HurtType.None)
    {
        if (immunityTime > 0)
        {
            immunityTime -= 1;
            return;
        }
        else if (immunityTimer > 0)
            return;

        // ≤‚ ‘
        Debug.Log(damage);
        Debug.Log(curHealth);

        base.Hurt(damage, shieldBreak, damageIncrease, type);
        UpdateHealth();

        if (curHealth <= 0)
        {
            StopPlaying();
            BattleDataManager.instance.EvaluateGameResult(false);
        }
    }

    public override void InstantHealing(float healingValue)
    {
        base.InstantHealing(healingValue);
        UpdateHealth();
    }

    public float MagicDamageIncrease()
    {
        return 1 + allDamage + magicDamage;
    }

    public float PhysicDamageIncrease()
    {
        return 1 + allDamage + physicDamage;
    }

    public void StartMoving()
    {
        BattleDataManager.instance.UpdatePlayerMovingStatus(true);
    }

    public void StopMoving()
    {
        BattleDataManager.instance.UpdatePlayerMovingStatus(false);
    }
}
