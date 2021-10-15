using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : SummonedObjectPrototype
{
    public float speed = 20f;

    private float damage;
    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        damage = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
            other.GetComponent<EnemyStatus>().Hurt(damage, false, 1, HurtType.Physic);
    }

    public void Instantiate(float d, Vector3 startPosition, Vector3 direction)
    {
        damage = d;
        transform.position = startPosition;
        rigidbody.velocity = speed * direction.normalized;
    }
}
