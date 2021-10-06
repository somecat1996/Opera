using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CreateAssetMenu(menuName = "Buff/BuffBasicInfomation")]
public class BuffBasicInfomation : ScriptableObject
{
    [Header("������Ϣ")]
    public int id;
    public string buffName;
    [TextArea]
    public string description;
    public Sprite icon;
    [Space]
    [Header("�Ƿ���ʾ��GUI��")]
    public bool hideInGUI = false;
}