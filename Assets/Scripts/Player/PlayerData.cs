using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    public bool firstTimeRunning = true; // �����ж��Ƿ����´浵 ����ʼ����Ϸʱ����Ϊfalse ���ǰ��Ҫ�����ֶ�����ΪTrue

    public int money;

    // �ؿ�����״̬ С��ȹؿ�����ID 0��ʹ��
    public bool[] levelStatus = new bool[10];

    public void Initialize()
    {
        money = 0;

        for(int i = 0; i < levelStatus.Length; i++)
        {
            levelStatus[i] = false;
        }

        levelStatus[1] = true; // Ĭ�Ͻ�����һ��
    }
}
