using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Card/Create CardInfomation")]
public class CardBasicInfomation : ScriptableObject
{
    // 卡牌资料相关
    public int id;
    public string cardName;
    public string description;
    public string story;
    public Sprite illustration;

    // 技能效果相关
    public int cost;
    public float duration;
    public bool useForEnemy;

    // 升级数据相关
    public int quantity; // 当前持有数量
    public int level = 0; // 0 级代表卡牌未解锁

}
