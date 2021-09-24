using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Create CardInfomation")]
public class CardBasicInfomation : ScriptableObject
{
    public int id;
    public string cardName;
    public int cost;
    public string description;
    public float duration;
    public bool useForEnemy;
}
