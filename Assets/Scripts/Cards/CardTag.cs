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

    public static string TagToString(Tag _tag)
    {
        switch (_tag)
        {
            case Tag.Aoe:
                {
                    return "Ⱥ��";
                }
            case Tag.Soe:
                {
                    return "����";
                }
            case Tag.Heal:
                {
                    return "�ظ�";
                }
            case Tag.Control:
                {
                    return "����";
                }
            case Tag.Burst:
                {
                    return "����";
                }
            case Tag.Buff:
                {
                    return "����";
                }
            case Tag.Shield:
                {
                    return "����";
                }
            case Tag.Rd:
                {
                    return "����";
                }
            case Tag.None:
            default:
                {
                    return "None";
                }
        }
    }
}
