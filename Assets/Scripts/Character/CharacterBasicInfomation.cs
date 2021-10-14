using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Character/Basic Infomation")]
public class CharacterBasicInfomation : ScriptableObject
{
    [Header("基本信息")]
    public int id;
    public string charName;
    [TextArea]
    public string description;
    [TextArea]
    public string story;

    public Sprite icon = null;
    public Sprite illustration = null;

    public CharacterType.CharacterTag charTag = CharacterType.CharacterTag.Common;

    [Header("可用被动Buff")]
    public List<int> buffID = new List<int>();

}
