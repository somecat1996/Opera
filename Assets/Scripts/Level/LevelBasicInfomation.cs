using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/LevelBasicInfomation")]
public class LevelBasicInfomation : ScriptableObject
{
    [Header("基本信息")]
    public int id;
    public string levelName;
    public Sprite bossIcon;
    public AudioClip sound;
    [Header("关卡奖励系数")]
    public float[] rewardFactor = new float[5];
    [Header("战利品卡牌ID")]
    public int[] lootCard = new int[3];

    [Header("需存档信息")]
    public bool unlocked = false;
    public int victoryTime = 0;

    /// <summary>
    /// 初始化数据 配合新游戏使用
    /// </summary>
    public void InitializeData()
    {
        if(id == 0)
        {
            unlocked = true;
        }
        else
        {
            unlocked = false;
        }

        victoryTime = 0;
    }

}
