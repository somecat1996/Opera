using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Buff/BuffBasicInfomation")]
public class BuffBasicInfomation : ScriptableObject
{
    [Header("基本信息")]
    public int id;
    public string buffName;
    [TextArea]
    public string description;
    public Sprite icon;
}
