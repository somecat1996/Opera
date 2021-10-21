using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    public bool test;
    public PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        if (test)
        {
            Test();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Test()
    {
        playerMovement.StartMoving();
    }
}
