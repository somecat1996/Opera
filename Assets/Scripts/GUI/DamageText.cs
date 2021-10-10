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

    public void Init(float damage, Transform t)
    {
        Text text = gameObject.GetComponent<Text>();
        text.text = damage.ToString();

        var col = t.GetComponent<Collider>();
        var topAhcor = new Vector3(col.bounds.center.x, col.bounds.max.y, col.bounds.center.z);
        tarPosition = topAhcor;
        //ת��Ϊ��Ļ����
        gameObject.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(tarPosition);
    }
}
