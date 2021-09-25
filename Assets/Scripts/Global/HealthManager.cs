using System.Collections;
using System.Collections.Generic;
using UnityEngine;


interface Hurt
{
    public void Hurt(float damage);
}

public class HealthManager : MonoBehaviour
{
    public virtual void Hurt(float damage)
    {
        // 基类调用
    }
}
