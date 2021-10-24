using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public float life;
    public float speed;
    public GameObject background;
    public GameObject text;
    public Vector3 direction;

    public TMP_ColorGradient normalColor;
    public TMP_ColorGradient criticalColor;

    private Vector3 tarPosition;
    // Update is called once per frame
    void Update()
    {
        life -= Time.deltaTime;
        if (life >= 0)
        {
            tarPosition += direction.normalized * speed * Time.deltaTime;
            gameObject.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(tarPosition);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init(float damage, Vector3 t, bool critical = true)
    {
        if (critical)
        {
            background.SetActive(true);
            TMP_Text tmp = text.GetComponent<TMP_Text>();
            tmp.colorGradientPreset = criticalColor;
            tmp.text = System.Math.Round(damage, 0).ToString();
            tarPosition = t;
            //转化为屏幕坐标
            gameObject.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(tarPosition);
        }
        else
        {
            background.SetActive(false);
            TMP_Text tmp = text.GetComponent<TMP_Text>();
            tmp.colorGradientPreset = normalColor;
            tmp.text = System.Math.Round(damage, 0).ToString();
            tarPosition = t;
            //转化为屏幕坐标
            gameObject.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(tarPosition);
        }
    }
}
