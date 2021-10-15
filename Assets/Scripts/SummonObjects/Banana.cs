using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour
{
    public Vector3 offset;

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
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerStatus>().Hurt(damage);
            Destroy(gameObject);
        }
    }

    public void Instantiate(Vector3 startPosition, float d)
    {
        damage = d;
        transform.position = startPosition + offset;
    }
}
