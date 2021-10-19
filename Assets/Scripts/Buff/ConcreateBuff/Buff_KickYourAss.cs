using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_KickYourAss : BuffPrototype   
{
    public float duration = 10f;
    public float probability = 0.08f;

    public GameObjectBase lastEnemy = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lastEnemy != BattleDataManager.instance.lastTargetEnemy)
        {
            lastEnemy = BattleDataManager.instance.lastTargetEnemy;

            if(Random.Range(0,1f) < GetTrueMainValue())
            {
                // ²¹³äÕÙ»½Î×¶¾ÍÞÍÞµÄ´úÂë

            }
        }
    }

    public override float GetTrueMainValue()
    {
        return GlobalValue.GetTrueProbaility(probability);

    }

    private void OnEnable()
    {
        lastEnemy = BattleDataManager.instance.lastTargetEnemy;
    }

    private void OnDisable()
    {
        
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
