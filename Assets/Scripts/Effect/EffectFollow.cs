using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFollow : EffectBase
{
    public Transform target;
    public Vector3 offset;

    protected override void Update()
    {
        base.Update();
        if (target)
        {
            transform.position = target.position + offset;
            transform.localScale = target.localScale;
        }
    }

    public void InstantiateFollow(float _life, Transform _target, Vector3 _offset)
    {
        Instantiate(_life, Vector3.zero, Vector3.zero);
        target = _target;
        offset = _offset;
    }
}
