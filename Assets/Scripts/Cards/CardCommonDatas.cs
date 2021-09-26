using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���ƹ�ͬ����
[CreateAssetMenu(menuName = "Card/Create CardCommonDatas")]
public class CardCommonDatas : ScriptableObject
{
    [Header("��������")]
    public float upgradeValue;

    [Header("ͨ������")]
    public int max_Level = 5;
    
    [Space]
    public List<Color> color_Quality = new List<Color>();
    public List<int> upgrade_Demanded = new List<int>();
    public List<int> upgrade_MoneyCost = new List<int>();
    public List<float> probability = new List<float>();
}
