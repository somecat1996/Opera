using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public static GUIManager instance;

    private void Awake()
    {
        instance = this;
    }

    [Header("Objects")]
    public Slider boss_HealthPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBossHealthPoint(float _v)
    {
        boss_HealthPoint.value = _v;
    }
}
