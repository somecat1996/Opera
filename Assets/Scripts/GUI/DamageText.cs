using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public float life;
    public float speed;
    public GameObject background;
    public GameObject text;

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

    public void Init(float damage, Vector3 t, bool critical = false)
    {
        if (critical)
        {
            background.SetActive(true);
            Text tmp = text.GetComponent<Text>();
            tmp.color = Color.white;
            tmp.text = System.Math.Round(damage, 0).ToString();
            tarPosition = t;
            //转化为屏幕坐标
            gameObject.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(tarPosition);
        }
        else
        {
            background.SetActive(false);
            Text tmp = text.GetComponent<Text>();
            tmp.color = Color.black;
            tmp.text = System.Math.Round(damage, 0).ToString();
            tarPosition = t;
            //转化为屏幕坐标
            gameObject.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(tarPosition);
        }
    }
}
