using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonCloud : MonoBehaviour
{
    public float poisonTime = 1f;

    private List<GameObjectInterface> enemies;
    private float poisonTimer;
    private float life;
    // Start is called before the first frame update
    void Start()
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

    public void Instantiate(float time, float range)
    {
        life = time;
        transform.localScale *= range;
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
