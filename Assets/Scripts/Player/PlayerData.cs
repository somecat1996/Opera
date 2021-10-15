using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    public bool firstTimeRunning = true; // �����ж��Ƿ����´浵 ����ʼ����Ϸʱ����Ϊfalse ���ǰ��Ҫ�����ֶ�����ΪTrue

    public int money;

    public void Initialize()
    {
        money = 0;
        firstTimeRunning = false;
    }
}
