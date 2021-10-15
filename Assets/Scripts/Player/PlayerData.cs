using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    public bool firstTimeRunning = true; // 用于判断是否有新存档 当开始新游戏时设置为false 打包前需要将其手动设置为True

    public int money;

    public void Initialize()
    {
        money = 0;
        firstTimeRunning = false;
    }
}
