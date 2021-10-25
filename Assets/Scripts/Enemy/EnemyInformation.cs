using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "EnemyInfo/EnemyInfo")]
public class EnemyInformation : ScriptableObject
{
    public string enemyName;
    [TextArea]
    public string description;
    public Sprite illus;
}
