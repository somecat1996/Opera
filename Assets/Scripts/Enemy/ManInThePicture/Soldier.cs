using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : EnemyStatus, ReducePower
{
    public float reduceRate;

    private PlayerManager playerManager;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManager>();
        StartReducing();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void StartReducing()
    {
        playerManager.ChangeRecoverySpeed_PowerPoint(reduceRate);
    }

    public void StopReducing()
    {
        playerManager.ChangeRecoverySpeed_PowerPoint(-reduceRate);
    }

    public override void Die()
    {
        StopReducing();
        base.Die();
    }
}
