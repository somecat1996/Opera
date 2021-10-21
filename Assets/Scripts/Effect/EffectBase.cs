using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBase : MonoBehaviour
{
    protected float life;

    protected virtual void Awake()
    {
        life = Mathf.Infinity;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0)
            Destroy(gameObject);
    }

    public virtual void Instantiate(float _life, Vector3 position, Vector3 offset)
    {
        life = _life;
        transform.position = position + offset;
    }
}
