using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 卡牌共同数据
[CreateAssetMenu(menuName = "Card/Create CardCommonDatas")]
public class CardCommonDatas : ScriptableObject
{
    [Header("升级倍率")]
    public float upgradeValue;

    [Header("通用数据")]
    public int max_Level = 5;
    
    [Space]
    public List<Color> color_Quality = new List<Color>();
    public List<int> upgrade_Demanded = new List<int>();
    public List<int> upgrade_MoneyCost = new List<int>();
    public List<float> probability = new List<float>();
}
