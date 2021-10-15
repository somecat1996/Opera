using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonCloud : MonoBehaviour
{
    public float poisonTime = 1f;

    private float poisonTimer;
    private float life;
    // Start is called before the first frame update
    void Start()
    {
        life = Mathf.Infinity;
        poisonTimer = poisonTime;
    }

    // Update is called once per frame
    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0) Destroy(gameObject);

        poisonTimer -= Time.deltaTime;
        if (poisonTimer <= 0)
        {
            poisonTimer = poisonTime;

        }
    }

    public void Instantiate(float time, float range)
    {
        life = time;
        transform.localScale *= range;
    }
}
