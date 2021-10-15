using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedObjectManager : MonoBehaviour
{
    public static SummonedObjectManager instance;
    [Header("’ŸªΩŒÔ")]
    public GameObject knife;
    public GameObject poisonCloud;
    public GameObject banana;

    private void Start()
    {
        instance = this;
    }

    public void SummonKnife(float damage, Vector3 startPosition, Vector3 direction)
    {
        Knife tmp = Instantiate(knife).GetComponent<Knife>();
        tmp.Instantiate(damage, startPosition, direction);
    }

    public void SummonPoisonCloud(Vector3 position, float time, float range)
    {
        PoisonCloud tmp = Instantiate(poisonCloud).GetComponent<PoisonCloud>();
        tmp.Instantiate(position, time, range);
    }

    public void SummonBanana(Vector3 position, float damage)
    {
        Banana tmp = Instantiate(banana).GetComponent<Banana>();
        tmp.Instantiate(position, damage);
    }
}
