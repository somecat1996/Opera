using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public float life;
    public float speed;

    private Vector3 tarPosition;
    // Update is called once per frame
    void Update()
    {
        life -= Time.deltaTime;
        if (life >= 0)
        {
            tarPosition.y += speed * Time.deltaTime;
            gameObject.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(tarPosition);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init(float damage, Vector3 t)
    {
        Text text = gameObject.GetComponent<Text>();
        text.text = System.Math.Round(damage, 1).ToString();
        tarPosition = t;
        //转化为屏幕坐标
        gameObject.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(tarPosition);
    }
}
