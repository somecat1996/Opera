using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour
{
    public Vector3 offset;
    public AudioClip bananaSound;

    private float damage;
    private Rigidbody rigidbody;

    private void Awake()
    {
        damage = 0;
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (transform.position.y < 0)
        {
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!other.GetComponent<PlayerStatus>().IsStunImmunity())
            {
                other.GetComponent<PlayerStatus>().Hurt(damage * EnemyManager.instance.EnemyAttackCoefficient());
                AudioManager.instance.PlaySound(bananaSound);
            }
            Destroy(gameObject);
        }
    }

    public void Instantiate(Vector3 startPosition, float d)
    {
        damage = d;
        transform.position = startPosition + offset;
    }
}
