using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalValue
{
    // 额外伤害
    public static float extraDamage = 0;

    // 三种伤害增量 皆以小数形式表示
    public static float damageIncrement_General = 0;
    public static float damageIncrement_Physics = 0;
    public static float damageIncrement_Magic = 0;

    // 暴击增量 以及 暴击概率[0,1]
    public static float critIncrement_Magic = 0;
    public static float probability_Crit_Magic = 0;

    public static float critIncrement_Physics = 0;
    public static float probability_Crit_Physics = 0;

    // 事件概率增幅
    public static float probabilityIncrement_Event = 0;

    // 对玩家伤害衰减率 [非负数]
    public static float damageDecrement_Player = 0;

    // 奖赏增幅
    public static float lootIncrement = 0;
    public static float rewardIncrement = 0;

    // 特殊伤害增量及其心流值阈值条件
    public static int costThreshold = -1;
    public static float damageIncrement_Special = 0;

    // 玩家攻击带毒 标志位
    public static bool poisonAttack = false;

    public static void ResetAllData()
    {
        damageIncrement_General = 0;
        damageIncrement_Physics = 0;
        damageIncrement_Magic = 0;

        critIncrement_Magic = 0;
        probability_Crit_Magic = 0;

        probabilityIncrement_Event = 0;
        damageDecrement_Player = 0;

        lootIncrement = 0; // 战利品 增量
        rewardIncrement = 0; // 观众奖赏 增量
    }

    /// <summary>
    /// 返回真实物理伤害
    /// </summary>
    /// <param name="_damage">卡牌原本伤害</param>
    /// <param name="_cost">传入卡牌心流值用于计算特殊加成 默认不考虑特殊加成</param>
    /// <returns></returns>
    public static float GetTruePhysicsDamage_ToEnemy(float _damage,int _cost = 100)
    {
        // 该卡牌符合阈值 计算特殊伤害
        if(_cost <= costThreshold)
        {
            float trueDamage = _damage;

            trueDamage = trueDamage * (1 + damageIncrement_General) * (1 + damageIncrement_Physics) * (1 + damageIncrement_Special) + extraDamage * (1 + damageIncrement_General);
            
            /*
            // 判断是否暴击
            if (Random.Range(0, 1.0f) < probability_Crit_Physics)
            {
                trueDamage *= (1 + critIncrement_Physics);
            }
            */

            return trueDamage;
        }
        else
        {
            float trueDamage = _damage;

            trueDamage = trueDamage * (1 + damageIncrement_General) * (1 + damageIncrement_Physics) + extraDamage * (1 + damageIncrement_General);

            /*
            // 判断是否暴击
            if (Random.Range(0, 1.0f) < probability_Crit_Physics)
            {
                trueDamage *= (1 + critIncrement_Physics);
            }
            */

            return trueDamage;
        }


    }
    /// <summary>
    /// 返回真实魔法伤害
    /// </summary>
    /// <param name="_damage">卡牌原本伤害</param>
    /// <param name="_cost">传入卡牌心流值用于计算特殊加成 默认不考虑特殊加成</param>
    /// <returns></returns>
    public static float GetTrueMagicDamage_ToEnemy(float _damage,int _cost = 100)
    {
        // 该卡牌符合阈值 计算特殊伤害
        if (_cost <= costThreshold)
        {
            float trueDamage = _damage;

            trueDamage = trueDamage * (1 + damageIncrement_General) * (1 + damageIncrement_Magic) * (1 + damageIncrement_Special) + extraDamage * (1 + damageIncrement_General);

            /*
            // 判断是否暴击
            if (Random.Range(0, 1.0f) < probability_Crit_Magic)
            {
                trueDamage *= (1 + critIncrement_Magic);
            }
            */
            return trueDamage;

        }
        else
        {
            float trueDamage = _damage;

            trueDamage = trueDamage * (1 + damageIncrement_General) * (1 + damageIncrement_Magic) + extraDamage * (1 + damageIncrement_General);

            /*
            // 判断是否暴击
            if (Random.Range(0, 1.0f) < probability_Crit_Magic)
            {
                trueDamage *= (1 + critIncrement_Magic);
            }
            */

            return trueDamage;
        }


    }

    /// <summary>
    /// 获得对玩家真实伤害
    /// </summary>
    /// <param name="_damage">初始伤害</param>
    /// <returns></returns>
    public static float GetTrueDamage_ToPlayer(float _damage)
    {
        float factor = (1 - damageDecrement_Player);
        factor = Mathf.Clamp(factor, 0, Mathf.Infinity);
        return _damage * factor;
    }

    /// <summary>
    /// 获得真实概率
    /// </summary>
    /// <param name="_origin"></param>
    /// <returns></returns>
    public static float GetTrueProbaility(float _origin)
    {
        float temp = _origin + probabilityIncrement_Event;
        temp = Mathf.Clamp(temp, 0, 1.0f);

        return temp;
    }

    /// <summary>
    /// 获得真实关卡结算金币值
    /// </summary>
    /// <returns></returns>
    public static int GetTrueLoot(float _v)
    {
        float factor = 1 + lootIncrement;
        return (int)(_v * factor);
    }

    /// <summary>
    /// 获得真实阶段性奖赏 喝彩值和赏钱
    /// </summary>
    /// <param name="_v"></param>
    /// <returns></returns>
    public static int GetTrueReward(float _v)
    {
        float factor = 1 + rewardIncrement;
        return (int)(_v * factor);
    }
}
