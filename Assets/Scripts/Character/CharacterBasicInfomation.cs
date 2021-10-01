using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Character/Basic Infomation")]
public class CharacterBasicInfomation : ScriptableObject
{
    [Header("������Ϣ")]
    public int id;
    public string charName;
    [TextArea]
    public string description;
    [TextArea]
    public string story;

    public Sprite icon = null;

    public CharacterType.CharacterTag charTag = CharacterType.CharacterTag.Common;

}
