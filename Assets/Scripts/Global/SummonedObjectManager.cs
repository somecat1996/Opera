using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedObjectManager : MonoBehaviour
{
    public static SummonedObjectManager instance;
    [Header("�ٻ���")]
    public GameObject knife;
    public GameObject poisonCloud;

    private void Start()
    {
        instance = this;
    }

    public void SummonKnife(float damage, Vector3 startPosition, Vector3 direction)
    {
        Knife tmp = Instantiate(knife).GetComponent<Knife>();
        tmp.Instantiate(damage, startPosition, direction);
    }

    public void SummonPoisonCloud(float time, float range)
    {
        PoisonCloud tmp = Instantiate(poisonCloud).GetComponent<PoisonCloud>();
        tmp.Instantiate(time, range);
    }
}
