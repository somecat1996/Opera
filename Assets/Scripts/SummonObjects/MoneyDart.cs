using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyDart : SummonedObjectPrototype
{
    public float speed = 20f;
    public float life = 5f;
    public int childNum = 3;

    private float damage;
    private float childDamage;
    private Rigidbody rigidbody;
    private EnemyStatus target;
    private float range;

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
            EffectsManager.instance.CreateEffect(11, 0.2f, other.transform.position, Vector3.zero);

            List<int> targetList = EnemyManager.instance.FindPositionInRange(target.position, range);
            for (int i = 0; i < childNum; i++)
            {
                if (targetList.Count > 0)
                {
                    int index = Random.Range(0, targetList.Count);
                    SummonedObjectManager.instance.SummonMoneyDartChild(childDamage, EnemyManager.instance.GetGameObjectAt(targetList[index]), target.transform.position);
                    targetList.RemoveAt(index);
                }
            }
            Destroy(gameObject);
        }
    }

    public void Instantiate(float d, float cd, float searchRange, GameObject o)
    {
        damage = d;
        childDamage = cd;
        transform.position = Player.instance.PlayerPosition();
        rigidbody.velocity = -speed * new Vector3(transform.position.x - o.transform.position.x, 0, transform.position.z - o.transform.position.z).normalized;

        target = o.GetComponent<EnemyStatus>();
        range = searchRange;
    }
}
