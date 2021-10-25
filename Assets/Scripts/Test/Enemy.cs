using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int max_HealthPoint = 100;
    public int cur_healthPoint = 100;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //GUIManager.instance.UpdateBossHealthPoint(cur_healthPoint / (float)max_HealthPoint);
    }
}
