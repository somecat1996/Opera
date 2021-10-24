using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : SummonedObjectPrototype
{
    public float speed = 20f;
    public float life = 5f;

    private float damage;
    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        damage = 0;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && other.GetComponent<EnemyStatus>())
        {
            EffectsManager.instance.CreateEffect(11, 0.2f, other.transform.position, Vector3.zero);
            other.GetComponent<EnemyStatus>().Hurt(damage, false, 1, HurtType.Physic);
        }
    }

    public void Instantiate(float d, Vector3 startPosition, Vector3 direction)
    {
        damage = d;
        transform.position = startPosition;
        rigidbody.velocity = speed * direction.normalized;
        transform.rotation = Quaternion.Euler(direction);
    }
}
