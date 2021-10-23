using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonCloud : MonoBehaviour
{
    public float poisonTime = 1f;

    private List<GameObjectInterface> enemies;
    private float poisonTimer;
    public float life;

    private void Awake()
    {
        life = Mathf.Infinity;
        poisonTimer = poisonTime;
        enemies = new List<GameObjectInterface>();
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
            foreach (GameObjectInterface tmp in enemies)
                tmp.Poisoning();
        }
    }

    public void Instantiate(Vector3 position, float time, float range)
    {
        life = time;
        transform.localScale *= range;
        transform.position = position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            enemies.Add(other.GetComponent<GameObjectInterface>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            enemies.Remove(other.GetComponent<GameObjectInterface>());
        }
    }
}
