using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtyWater : MonoBehaviour
{
    public float life = 3f;

    private Donkey donkey;

    private void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && donkey)
        {
            donkey.SummonXiaomin();
        }
    }

    public void Instantiate(Vector3 startPosition, Donkey d)
    {
        donkey = d;
        transform.position = startPosition;
    }
}
