using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : GameObjectBase
{
    private float allDamage;
    private float magicDamage;
    private float physicDamage;

    protected override void Awake()
    {
        base.Awake();
        allDamage = 0f;
        magicDamage = 0f;
        physicDamage = 0f;
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
        base.Update();
    }

    private void UpdateHealth()
    {
        PlayerManager.instance.SetCurrentHealthPoint(curHealth);
    }

    public void SetMaxHealth(float m)
    {
        maxHealth = m;
        curHealth = m;
    }

    public override void Hurt(float damage, bool shieldBreak, float damageIncrease)
    {
        base.Hurt(damage, shieldBreak, damageIncrease);
        UpdateHealth();
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
