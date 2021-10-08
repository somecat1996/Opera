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

    public CardTag.Tag cardTag = CardTag.Tag.None;
    public CardTag.Type cardType = CardTag.Type.None;
    public CharacterType.CharacterTag belongner; // ����������ɫ

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

            // ��ȷ��������ȷ������
            if (quantity >= CardManager.instance.cardCommonData.upgrade_Demanded[level])
            {
                if (PlayerManager.instance.ChangeMoney(-CardManager.instance.cardCommonData.upgrade_MoneyCost[level]))
                {
                    quantity -= CardManager.instance.cardCommonData.upgrade_Demanded[level];
                    mainValue_Cur *= 1.1f;
                    Debug.Log("Upgraded successfully!");
                    level++;

                    GUIManager.instance.UpdateMoneyText(PlayerManager.instance.data.money);
                    return true;
                }
                else
                {
                    Debug.Log("Insufficient money");
                }
            }
            else
            {
                Debug.Log("Insufficient card");
            }

            return false;
        }
    }

    // ��������ʼ������
    public void InitilizeCard()
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

    // ��ü��ܽ���
    public string GetDesc()
    {
        string text = null;
        
        float mainvalue = mainValue_Cur;

        // ֻ���˺����ƲŻ��мӳ�ֵ����
        if(cardType == CardTag.Type.Magic)
        {
            mainvalue = GlobalValue.GetTrueMagicDamage_ToEnemy(mainvalue,cost);
        }
        else if(cardType == CardTag.Type.Physics)
        {
            mainvalue = GlobalValue.GetTruePhysicsDamage_ToEnemy(mainvalue, cost);
        }   

        if (description.Contains("#MainValue"))
        {
            text = description.Replace("#MainValue",mainValue_Cur.ToString() + (mainvalue == mainValue_Cur ? "" : "(+" + (mainvalue-mainValue_Cur) + "�ӳ�)"));
        }else if (description.Contains("%MainValue"))
        {
            text = description.Replace("%MainValue",(mainValue_Cur * 100).ToString()+'%');
        }

        // ��ʱ�������˺�
        if (description.Contains("#Duration"))
        {


            text = text.Replace("#Duration", duration.ToString());
        }
            
        
        return text;
    }

    // ��ñ�ǩ
    public string GetTag()
    {
        return CardTag.TagToString(cardTag);
    }
}
