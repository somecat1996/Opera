using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalValue
{
    // �����˺�����
    public static float damageIncrement_General = 0;
    public static float damageIncrement_Physics = 0;
    public static float damageIncrement_Magic = 0;

    // �������� �Լ� ��������[0,1]
    public static float critIncrement = 0;
    public static float probability_Crit = 0;

    // �¼���������
    public static float probabilityIncrement_Event = 0;

    // �˺�˥����[
    public static float damageDecrement_Player = 0;

    // ��������
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

    // ������ʵ�����˺�
    public static float GetTruePhysicsDamage_ToEnemy(float _damage)
    {
        float trueDamage = _damage;

        trueDamage = trueDamage + (1 * damageIncrement_General) * (1 * damageIncrement_Physics);

        // �ж��Ƿ񱩻�
        if (Random.Range(0,1.0f) < probability_Crit)
        {
            trueDamage *= (1 + critIncrement);
        }

        return trueDamage;
    }
    // ������ʵħ���˺�
    public static float GetTrueMagicDamage_ToEnemy(float _damage)
    {
        float trueDamage = _damage;

        trueDamage = trueDamage + (1 * damageIncrement_General) * (1 * damageIncrement_Magic);

        // �ж��Ƿ񱩻�
        if (Random.Range(0, 1.0f) < probability_Crit)
        {
            trueDamage *= (1 + critIncrement);
        }

        return trueDamage;
    }


}
