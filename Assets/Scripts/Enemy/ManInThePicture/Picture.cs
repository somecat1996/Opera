using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picture : MonoBehaviour
{
    private int currentStage;

    public float stage1CoolingTime = 20f;
    public float stage1Damage = 100;
    private float stage1CoolingTimer;


    // Start is called before the first frame update
    void Start()
    {
        currentStage = 1;

        stage1CoolingTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentStage == 1)
            Stage1();
        else if (currentStage == 2)
            Stage2();
    }

    public void Change()
    {
        currentStage = 2;
    }

    private void Stage1()
    {

    }

    private void Stage2()
    {

    }
}
