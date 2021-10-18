using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyDartChild : SummonedObjectPrototype
{
    public float speed = 20f;
    public float life = 5f;

    private float damage;
    private Rigidbody rigidbody;
    private EnemyStatus target;

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
        if (other.tag == "Enemy" && other.GetComponent<EnemyStatus>().position == target.position)
        {
            other.GetComponent<EnemyStatus>().Hurt(damage, false, 1, HurtType.Physic);
        }
    }

    public void Instantiate(float d, GameObject o, Vector3 startPosition)
    {
        damage = d;
        transform.position = startPosition;
        rigidbody.velocity = -speed * new Vector3(transform.position.x - o.transform.position.x, 0, transform.position.z - o.transform.position.z).normalized;

        target = o.GetComponent<EnemyStatus>();
    }
}
