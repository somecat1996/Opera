using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : HealthManager
{
    public float maxHealth;
    public Vector3 offsetPos;
    public GameObject healthBarPrefab;
    public int position;

    private float curHealth;
    private HealthBarManager healthBarManager;
    // Start is called before the first frame update
    void Start()
    {
        // ��������ʼ��Ѫ��
        GameObject healthBar = Instantiate(healthBarPrefab, GameObject.FindGameObjectWithTag("HealthBarCanvas").transform);
        healthBarManager = healthBar.GetComponent<HealthBarManager>();

        curHealth = maxHealth;
        healthBarManager.Init(transform, offsetPos);
    }

    // ��¼������
    public void SetPosition(int p)
    {
        position = p;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Hurt(float damage)
    {
        curHealth -= damage;
        healthBarManager.UpdateHealth(curHealth / maxHealth);
    }
}
