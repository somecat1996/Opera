using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 卡牌共同数据
[CreateAssetMenu(menuName = "Card/Create CardCommonDatas")]
public class CardCommonDatas : ScriptableObject
{
    public float upgradeValue;

    public List<Color> color_Quality = new List<Color>();
    public List<int> upgrade_Demanded = new List<int>();
    public List<int> Upgrade_MoneyCost = new List<int>();
}
