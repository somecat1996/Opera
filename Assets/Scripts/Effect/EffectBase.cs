using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBase : MonoBehaviour
{
    public float life = Mathf.Infinity;

    private bool alive;

    private void Awake()
    {
        alive = true;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (alive)
        {
            life -= Time.deltaTime;
            if (life <= 0)
                DestoryObject();
        }
    }

    public virtual void Instantiate(float _life, Vector3 position, Vector3 offset)
    {
        life = _life;
        transform.position = position + offset;
    }

    public virtual void DestoryObject()
    {
        alive = false;
        Destroy(gameObject);
    }
}
