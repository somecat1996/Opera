using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_KickYourAss : BuffPrototype   
{
    public float duration = 10f;
    public float probability = 0.08f;

    public GameObjectBase lastEnemy = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override float GetTrueMainValue()
    {
        return GlobalValue.GetTrueProbaility(probability);

    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
