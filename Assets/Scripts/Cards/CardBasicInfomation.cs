using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Card/Create CardInfomation")]
public class CardBasicInfomation : ScriptableObject
{
    [Header("卡牌基本信息")]
    public int id;
    public string cardName;
    [TextArea]
    public string description;
    [TextArea]
    public string story;
    public Sprite illustration;
    public int animationID = -1;
    [Header("1-基础 2-稀有 3-诗史 4-传说")]
    [Range(1,4)]
    public int rarity = 1;

    public CardTag.Tag cardTag = CardTag.Tag.None;
    public CardTag.Type cardType = CardTag.Type.None;
    public CharacterType.CharacterTag belongner; // 卡牌所属角色

    [Header("卡牌效果相关数据")]
    public float mainValue_Origin; // 初始主要参数-用于治疗量或攻击量
    public int cost;
    public float duration;
    public float radius;

    [Header("需存档数据")]
    public float mainValue_Cur; // 主要参数-进行升级后
    public int quantity; // 当前持有数量
    public int level = 0; // 0 级代表卡牌未解锁

    // 根据卡牌等级对主参进行修正
    // 参数用于是否初始化数值-默认为 否
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
                GUIManager.instance.SpawnSystemText("卡牌等级已满!");
                return false;
            }

            // 先确定数量后确定货币
            if (quantity >= CardManager.instance.cardCommonData.upgrade_Demanded[level])
            {
                if (PlayerManager.instance.ChangeMoney(-CardManager.instance.cardCommonData.upgrade_MoneyCost[level]))
                {
                    quantity -= CardManager.instance.cardCommonData.upgrade_Demanded[level];
                    mainValue_Cur *= 1.1f;
                    GUIManager.instance.SpawnSystemText("升级成功!");
                    level++;

                    GUIManager.instance.UpdateMoneyText(PlayerManager.instance.data.money);
                    return true;
                }
                else
                {
                    GUIManager.instance.SpawnSystemText("金币不足!");
                }
            }
            else
            {
                GUIManager.instance.SpawnSystemText("卡牌数量不足!");
            }

            return false;
        }
    }

    // 解锁并初始化卡牌
    public void InitilizeCard()
    {
        level = 1;
        UpgradeMainValue(true);
        quantity = 0;
    }
    // 锁定卡牌并初始化
    public void LockCard()
    {
        level = 0;
        quantity = 0;
        UpgradeMainValue(true);
    }

    // 获得技能解释
    public string GetDesc()
    {
        string text = description;
        
        float mainvalue = mainValue_Cur;

        // 只有伤害卡牌才会有加成值计算
        if(cardType == CardTag.Type.Magic)
        {
            mainvalue = GlobalValue.GetTrueMagicDamage_ToEnemy(mainvalue,cost);
        }
        else if(cardType == CardTag.Type.Physics)
        {
            mainvalue = GlobalValue.GetTruePhysicsDamage_ToEnemy(mainvalue, cost);
        }   

        // 主要参数转换
        if (description.Contains("#MainValue"))
        {
            text = description.Replace("#MainValue",mainValue_Cur.ToString("0.0") + (mainvalue == mainValue_Cur ? "" : "(+" + (mainvalue-mainValue_Cur).ToString("0.0") + "加成)"));
        }
        
        // 小数转百分数
        if (description.Contains("%MainValue"))
        {
            text = description.Replace("%MainValue",(mainValue_Cur * 100).ToString("0.0") +'%');
        }

        // 乘数替换 duration当作倍数
        if (description.Contains("@MainValue"))
        {
            text = text.Replace("@MainValue", (mainValue_Cur*duration).ToString("0.0") + (mainvalue * duration == mainValue_Cur * duration ? "" : "(+" + ((mainvalue - mainValue_Cur) * duration).ToString("0.0") + "加成)"));
        }
            
        
        return text;
    }

    // 获得标签
    public string GetTag()
    {
        return CardTag.TagToString(cardTag);
    }
}
