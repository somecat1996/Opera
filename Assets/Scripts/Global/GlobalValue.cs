using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalValue
{
    // 三种伤害增量
    public static float damageIncrement_General = 0;
    public static float damageIncrement_Physics = 0;
    public static float damageIncrement_Magic = 0;

    // 暴击增量 以及 暴击概率[0,1]
    public static float critIncrement = 0;
    public static float probability_Crit = 0;

    // 事件概率增幅
    public static float probabilityIncrement_Event = 0;

    // 伤害衰减率[
    public static float damageDecrement_Player = 0;

    // 奖赏增幅
    public static float lootIncrement = 0;
    public static float rewardIncrement = 0;

    public static void ResetAllData()
    {
        damageIncrement_General = 0;
        damageIncrement_Physics = 0;
        damageIncrement_Magic = 0;

        critIncrement = 0;
        probability_Crit = 0;

        probabilityIncrement_Event = 0;
        damageDecrement_Player = 0;

        lootIncrement = 0;
        rewardIncrement = 0;
    }

    // 返回真实物理伤害
    public static float GetTruePhysicsDamage_ToEnemy(float _damage)
    {
        float trueDamage = _damage;

        trueDamage = trueDamage + (1 * damageIncrement_General) * (1 * damageIncrement_Physics);

        // 判断是否暴击
        if (Random.Range(0,1.0f) < probability_Crit)
        {
            trueDamage *= (1 + critIncrement);
        }

        return trueDamage;
    }
    // 返回真实魔法伤害
    public static float GetTrueMagicDamage_ToEnemy(float _damage)
    {
        float trueDamage = _damage;

        trueDamage = trueDamage + (1 * damageIncrement_General) * (1 * damageIncrement_Magic);

        // 判断是否暴击
        if (Random.Range(0, 1.0f) < probability_Crit)
        {
            trueDamage *= (1 + critIncrement);
        }

        return trueDamage;
    }


}
