using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doutianzhang : MonoBehaviour
{
    public float life = 10f;
    public float speed = 5f;
    public float walkTime = 2f;
    public float damage = 100000;

    public Animator sprite;
    public Animator shadow;

    private bool walk;
    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        walk = true;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = speed * Vector3.right;

    }

    // Update is called once per frame
    void Update()
    {
        life -= Time.deltaTime;
        if (walk)
        {
            if (life <= walkTime)
            {
                walk = false;
                sprite.SetTrigger("Throw");
                shadow.SetTrigger("Throw");
                rigidbody.velocity = Vector3.zero;
            }
        }
        else if (life <= 0)
        {
            EnemyManager.instance.HurtAll(damage);
            Destroy(gameObject);
        }
    }
}
