using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/LevelBasicInfomation")]
public class LevelBasicInfomation : ScriptableObject
{
    [Header("������Ϣ")]
    public int id;
    public string levelName;
    public Sprite bossIcon;
    public AudioClip sound;
    [Header("�ؿ�����ϵ��")]
    public float[] rewardFactor = new float[5];
    [Header("ս��Ʒ����ID")]
    public int[] lootCard = new int[3];

    [Header("��浵��Ϣ")]
    public bool unlocked = false;
    public int victoryTime = 0;

    /// <summary>
    /// ��ʼ������ �������Ϸʹ��
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
