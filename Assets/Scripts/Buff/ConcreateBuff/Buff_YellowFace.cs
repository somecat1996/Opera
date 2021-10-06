using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_YellowFace : BuffPrototype
{
    public GameObjectBase cur_Target = null;
    public float probability = 0.3f;

    private void Update()
    {
        if(cur_Target != BattleDataManager.instance.lastTargetEnemy)
        {
            cur_Target = BattleDataManager.instance.lastTargetEnemy;

            if (Random.Range(0, 1.0f) <= probability)
            {
                Debug.Log("Á÷Ñª£¡");
            }
        }
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
        }
    }
}
