using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public bool test = false;
    public static EffectsManager instance;
    public List<GameObject> effects;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (test)
            Test();
    }

    public void CreateEffect(int id, float _life, Vector3 position, Vector3 offset)
    {
        GameObject tmp = Instantiate(effects[id]);
        tmp.GetComponent<EffectBase>().Instantiate(_life, position, offset);
    }

    public void CreateFollowEffect(int id, float _life, Transform _target, Vector3 _offset)
    {
        GameObject tmp = Instantiate(effects[id]);
        tmp.GetComponent<EffectFollow>().InstantiateFollow(_life, _target, _offset);
    }

    private void Test()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            CreateFollowEffect(0, 3f, GameObject.FindGameObjectWithTag("Player").transform, Vector3.zero);
        }
    }
}
