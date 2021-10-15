using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour
{
    private float damage;

    private void Awake()
    {
        damage = 0;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
            other.GetComponent<EnemyStatus>().Hurt(damage, false, 1, HurtType.Physic);
    }

    public void Instantiate(float d, Vector3 startPosition)
    {
        damage = d;
        transform.position = startPosition;
    }
}
