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
                    return "魔法卡";
                }
            case Type.Physics:
                {
                    return "物理卡";
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
                    return "群伤";
                }
            case Tag.Soe:
                {
                    return "点伤";
                }
            case Tag.Heal:
                {
                    return "回复";
                }
            case Tag.Control:
                {
                    return "控制";
                }
            case Tag.Burst:
                {
                    return "爆发";
                }
            case Tag.Buff:
                {
                    return "增益";
                }
            case Tag.Shield:
                {
                    return "护盾";
                }
            case Tag.Rd:
                {
                    return "护盾";
                }
            case Tag.None:
            default:
                {
                    return "None";
                }
        }
    }
}
