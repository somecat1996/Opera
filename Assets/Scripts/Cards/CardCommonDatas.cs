using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ���ƹ�ͬ����
[CreateAssetMenu(menuName = "Card/Create CardCommonDatas")]
public class CardCommonDatas : ScriptableObject
{
    [Header("��������")]
    public float upgradeValue;

    [Header("ͨ������")]
    public int max_Level = 5;
    
    [Space]
    //public List<Color> color_Quality = new List<Color>();
    public List<Sprite> sprite_Quality = new List<Sprite>();
    public List<int> upgrade_Demanded = new List<int>();
    public List<int> upgrade_MoneyCost = new List<int>();
    public List<float> probability = new List<float>();
}
