using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    private Slider hpSlider;
    private RectTransform rectTrans;

    private Transform target;
    // ͷ��ƫ����
    public Vector3 offsetPos; 

    private void Start()
    {
        hpSlider = GetComponent<Slider>();
        rectTrans = GetComponent<RectTransform>();
    }

    // ��ʼ�������ø���Ŀ���ƫ����
    public void Init(Transform t, Vector3 o)
    {
        target = t;
        offsetPos = o;
    }

    private void Update()
    {
        if (target == null) return;

        //ͨ��Collider����ȡͷ������
        var col = target.GetComponent<Collider>();
        var topAhcor = new Vector3(col.bounds.max.x, col.bounds.center.y, col.bounds.center.z);
        //����ͷ��ƫ����
        Vector3 tarPos = topAhcor;

        var viewPos = Camera.main.WorldToViewportPoint(tarPos); //�õ��Ӵ�����

        Vector2 screenPos;

        if (viewPos.z > 0f && viewPos.x > 0f && viewPos.x < 1f && viewPos.y > 0f && viewPos.y < 1f)
        {
            //��ȡ��Ļ����
            screenPos = Camera.main.WorldToScreenPoint(tarPos + offsetPos); //����ͷ��ƫ����
        }
        else
        {
            //���ڿ��Ӵ��ڵ�ģ�ͣ��������ƶ���������
            screenPos = Vector3.up * 3000f;
        }

        //ת��Ϊ��Ļ����
        rectTrans.position = screenPos;
    }

    // ����Ѫ����ʾ
    public void UpdateHealth(float v)
    {
        hpSlider.value = v;
    }
}
