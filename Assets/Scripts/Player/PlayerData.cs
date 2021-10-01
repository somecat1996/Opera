using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    public bool firstTimeRunning = true; // 用于判断是否有新存档 当开始新游戏时设置为false 打包前需要将其手动设置为True

    public int money;

    // 关卡解锁状态 小标既关卡数字ID 0不使用
    public bool[] levelStatus = new bool[10];

    public void Initialize()
    {
        money = 0;

        for(int i = 0; i < levelStatus.Length; i++)
        {
            levelStatus[i] = false;
        }

        levelStatus[1] = true; // 默认解锁第一关
    }
}
