using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTest : MonoBehaviour
{
    public GameObject quad1;
    public GameObject quad2;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            quad1.SetActive(true);
            quad2.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            quad1.SetActive(false);
            quad2.SetActive(true);
        }
    }
}
