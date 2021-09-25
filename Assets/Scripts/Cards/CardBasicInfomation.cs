using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Card/Create CardInfomation")]
public class CardBasicInfomation : ScriptableObject
{
    // �����������
    public int id;
    public string cardName;
    public string description;
    public string story;
    public Sprite illustration;

    // ����Ч�����
    public int cost;
    public float duration;
    public bool useForEnemy;

    // �����������
    public int quantity; // ��ǰ��������
    public int level = 0; // 0 ��������δ����

}
