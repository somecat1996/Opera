using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardTag 
{
    public enum Tag
    {
        None = 0,
        Aoe,
        Soe,
        Heal,
        Control,
        Burst,
        Buff,
        Shield,
        Rd,
        Posion,
        Common
    }
    public enum Type
    {
        None = 0,
        Physics,
        Magic
    }

    public static string TypeToString(Type _type)
    {
        switch (_type)
        {
            case Type.Magic:
                {
                    return "魔 法 卡";
                }
            case Type.Physics:
                {
                    return "物 理 卡";
                }
            case Type.None:
            default:
                {
                    return "Nah";
                }
        }
    }

    public static string TagToString(Tag _tag)
    {
        switch (_tag)
        {
            case Tag.Aoe:
                {
                    return "群 伤";
                }
            case Tag.Soe:
                {
                    return "点 伤";
                }
            case Tag.Heal:
                {
                    return "回 复";
                }
            case Tag.Control:
                {
                    return "控 制";
                }
            case Tag.Burst:
                {
                    return "爆 发";
                }
            case Tag.Buff:
                {
                    return "增 益";
                }
            case Tag.Shield:
                {
                    return "护 盾";
                }
            case Tag.Rd:
                {
                    return "减 伤";
                }
            case Tag.Posion:
                {
                    return "毒 伤";
                }
            case Tag.Common:
                {
                    return "剧 情 卡";
                }
            case Tag.None:
            default:
                {
                    return "玄 学 卡";
                }
        }
    }
}
