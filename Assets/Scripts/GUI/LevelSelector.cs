using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelSelector : MonoBehaviour
{
    GameObject[] go = new GameObject[3];

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            go[i] = transform.GetChild(i).gameObject;
        }

        foreach(var i in go)
        {
            i.transform.DOMoveX(100, 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
