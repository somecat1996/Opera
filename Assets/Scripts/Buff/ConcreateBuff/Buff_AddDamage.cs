using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_AddDamage : BuffPrototype
{
    public int level = 0;
    public float damage = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        if (!activated)
        {
            activated = true;
            GlobalValue.damageIncrement_General += damage;
        }

    }

    private void OnDisable()
    {
        if (!activated)
        {
            activated = true;
            GlobalValue.damageIncrement_General -= damage;
        }


    }

    // 按照Buff自行需求 刷新数据
    public override void ReflashData()
    {
        float probability = Random.Range(0, 1.0f);

        if (probability <= 0.5f) // Level 1
        {
            damage = 0.06f;
            level = 1;

            replaceText = (damage).ToString();
        }
        else if(probability > 0.5f && probability <= 0.8f)
        {
            damage = 0.08f;
            level = 2;

            replaceText = (damage).ToString();
        }
        else
        {
            damage = 0.1f;
            level = 3;

            replaceText = (damage).ToString();
        }
    }
}
