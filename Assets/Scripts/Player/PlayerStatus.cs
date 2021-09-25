using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : HealthManager
{
    public float maxHealth;
    public Vector3 offsetPos;
    public GameObject healthBarPrefab;

    private float curHealth;
    private HealthBarManager healthBarManager;
    // Start is called before the first frame update
    void Start()
    {
        // 创建并初始化血条
        GameObject healthBar = Instantiate(healthBarPrefab, GameObject.FindGameObjectWithTag("HealthBarCanvas").transform);
        healthBarManager = healthBar.GetComponent<HealthBarManager>();

        curHealth = maxHealth;
        healthBarManager.Init(transform, offsetPos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Hurt(float damage)
    {
        curHealth -= damage;
        Debug.Log(curHealth);
        healthBarManager.UpdateHealth(curHealth / maxHealth);
    }
}
