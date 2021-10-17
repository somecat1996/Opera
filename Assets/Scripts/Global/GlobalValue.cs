using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalValue
{
    // �����˺�
    public static float extraDamage = 0;

    // �����˺����� ����С����ʽ��ʾ
    public static float damageIncrement_General = 0;
    public static float damageIncrement_Physics = 0;
    public static float damageIncrement_Magic = 0;

    // �������� �Լ� ��������[0,1]
    public static float critIncrement_Magic = 0;
    public static float probability_Crit_Magic = 0;

    public static float critIncrement_Physics = 0;
    public static float probability_Crit_Physics = 0;

    // �¼���������
    public static float probabilityIncrement_Event = 0;

    // ������˺�˥���� [�Ǹ���]
    public static float damageDecrement_Player = 0;

    // ��������
    public static float lootIncrement = 0;
    public static float rewardIncrement = 0;

    // �����˺�������������ֵ��ֵ����
    public static int costThreshold = -1;
    public static float damageIncrement_Special = 0;

    // ��ҹ������� ��־λ
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

        lootIncrement = 0; // ս��Ʒ ����
        rewardIncrement = 0; // ���ڽ��� ����
    }

    /// <summary>
    /// ������ʵ�����˺�
    /// </summary>
    /// <param name="_damage">����ԭ���˺�</param>
    /// <param name="_cost">���뿨������ֵ���ڼ�������ӳ� Ĭ�ϲ���������ӳ�</param>
    /// <returns></returns>
    public static float GetTruePhysicsDamage_ToEnemy(float _damage,int _cost = 100)
    {
        // �ÿ��Ʒ�����ֵ ���������˺�
        if(_cost <= costThreshold)
        {
            float trueDamage = _damage;

            trueDamage = trueDamage * (1 + damageIncrement_General) * (1 + damageIncrement_Physics) * (1 + damageIncrement_Special) + extraDamage * (1 + damageIncrement_General);
            
            /*
            // �ж��Ƿ񱩻�
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
            // �ж��Ƿ񱩻�
            if (Random.Range(0, 1.0f) < probability_Crit_Physics)
            {
                trueDamage *= (1 + critIncrement_Physics);
            }
            */

            return trueDamage;
        }


    }
    /// <summary>
    /// ������ʵħ���˺�
    /// </summary>
    /// <param name="_damage">����ԭ���˺�</param>
    /// <param name="_cost">���뿨������ֵ���ڼ�������ӳ� Ĭ�ϲ���������ӳ�</param>
    /// <returns></returns>
    public static float GetTrueMagicDamage_ToEnemy(float _damage,int _cost = 100)
    {
        // �ÿ��Ʒ�����ֵ ���������˺�
        if (_cost <= costThreshold)
        {
            float trueDamage = _damage;

            trueDamage = trueDamage * (1 + damageIncrement_General) * (1 + damageIncrement_Magic) * (1 + damageIncrement_Special) + extraDamage * (1 + damageIncrement_General);

            /*
            // �ж��Ƿ񱩻�
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
            // �ж��Ƿ񱩻�
            if (Random.Range(0, 1.0f) < probability_Crit_Magic)
            {
                trueDamage *= (1 + critIncrement_Magic);
            }
            */

            return trueDamage;
        }


    }

    /// <summary>
    /// ��ö������ʵ�˺�
    /// </summary>
    /// <param name="_damage">��ʼ�˺�</param>
    /// <returns></returns>
    public static float GetTrueDamage_ToPlayer(float _damage)
    {
        float factor = (1 - damageDecrement_Player);
        factor = Mathf.Clamp(factor, 0, Mathf.Infinity);
        return _damage * factor;
    }

    /// <summary>
    /// �����ʵ����
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
    /// �����ʵ�ؿ�������ֵ
    /// </summary>
    /// <returns></returns>
    public static int GetTrueLoot(float _v)
    {
        float factor = 1 + lootIncrement;
        return (int)(_v * factor);
    }

    /// <summary>
    /// �����ʵ�׶��Խ��� �Ȳ�ֵ����Ǯ
    /// </summary>
    /// <param name="_v"></param>
    /// <returns></returns>
    public static int GetTrueReward(float _v)
    {
        float factor = 1 + rewardIncrement;
        return (int)(_v * factor);
    }
}
