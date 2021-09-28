using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Card/Create CardInfomation")]
public class CardBasicInfomation : ScriptableObject
{
    [Header("���ƻ�����Ϣ")]
    public int id;
    public string cardName;
    [TextArea]
    public string description;
    [TextArea]
    public string story;
    public Sprite illustration;
    [Header("1-���� 2-ϡ�� 3-ʫʷ 4-��˵")]
    [Range(1,4)]
    public int rarity = 1;  

    public CharacterType.Character belongner; // ����������ɫ

    [Header("����Ч���������")]
    public float mainValue_Origin; // ��ʼ��Ҫ����-�����������򹥻���
    public int cost;
    public float duration;
    public float radius;

    [Header("��浵����")]
    public float mainValue_Cur; // ��Ҫ����-����������
    public int quantity; // ��ǰ��������
    public int level = 0; // 0 ��������δ����

    // ���ݿ��Ƶȼ������ν�������
    // ���������Ƿ��ʼ����ֵ-Ĭ��Ϊ ��
    public bool UpgradeMainValue(bool _initial = false)
    {
        if (_initial)
        {
            mainValue_Cur = mainValue_Origin;
            return false;
        }
        else
        {
            if (level == CardManager.instance.cardCommonData.max_Level)
            {
                Debug.Log("Max Level!");
                return false;
            }

            if (quantity >= CardManager.instance.cardCommonData.upgrade_Demanded[level])
            {
                quantity -= CardManager.instance.cardCommonData.upgrade_Demanded[level];
                mainValue_Cur *= 1.1f;
                Debug.Log("Upgraded successfully!");
                level++;
                return true;
            }

            Debug.Log("Insufficient card");

            return false;
        }
    }

    // ��������
    public void UnlockCard()
    {
        level = 1;
        UpgradeMainValue(true);
        quantity = 0;
    }
    // �������Ʋ���ʼ��
    public void LockCard()
    {
        level = 0;
        quantity = 0;
    }
}
