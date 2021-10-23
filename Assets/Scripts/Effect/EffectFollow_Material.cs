using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EffectFollow_Material : EffectFollow
{
    Renderer renderer;

    void Start()
    {
        renderer = transform.GetComponentInChildren<Renderer>();

        renderer.material.SetFloat("Appear", 0);
        renderer.material.DOFloat(1, "Appear", 1.5f);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void DestoryObject()
    {
        renderer.material.DOFloat(0, "Appear", 1.5f).OnComplete(() =>{ Destroy(gameObject); });
    }
}
