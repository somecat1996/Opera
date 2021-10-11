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
        if (activated)
        {
            activated = false;
            GlobalValue.damageIncrement_General -= damage;
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }

    // ����Buff�������� ˢ������
    public override void ReflashData()
    {
        float probability = Random.Range(0, 1.0f);

        if (probability <= 0.5f) // Level 1
        {
            damage = 0.06f;
            level = 1;

            replaceValue = damage;
        }
        else if(probability > 0.5f && probability <= 0.8f)
        {
            damage = 0.08f;
            level = 2;

            replaceValue = damage;
        }
        else
        {
            damage = 0.1f;
            level = 3;

            replaceValue = damage;
        }
    }
}
