using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfoCarrier : MonoBehaviour
{
    public EnemyInformation enemyInfo;

    public void DisplayInfo()
    {
        if(enemyInfo!=null)
            GUIManager.instance.DisplayEnemyInfo(enemyInfo);
    }
}
