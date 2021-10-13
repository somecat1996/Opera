using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Line : MonoBehaviour
{
    public float life = 5f;

    private Vector3 tarPosition;
    // Update is called once per frame
    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Init(string line, Vector3 t)
    {
        Text text = gameObject.GetComponent<Text>();
        text.text = line;
        tarPosition = t;
        //转化为屏幕坐标
        gameObject.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(tarPosition);
    }
}
