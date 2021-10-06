using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CreateAssetMenu(menuName = "Buff/BuffBasicInfomation")]
public class BuffBasicInfomation : ScriptableObject
{
    [Header("基本信息")]
    public int id;
    public string buffName;
    [TextArea]
    public string description;
    public Sprite icon;
    [Space]
    [Header("是否显示在GUI上")]
    public bool hideInGUI = false;
}