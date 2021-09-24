using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���忨������
// ʹ�ÿ��Ʋ����ӿ��Լ�Ч��ʵ�ֽӿ�������������Ϣ��
public class Card_Attack : MonoBehaviour, ICardOperation,ICardEffectTrigger
{
    public CardBasicInfomation cardInfo;

    // ÿ�ſ��������Ч����ֵ���ھ�������

    private void Awake()
    {
        
    }

    private void Start()
    {

    }

    public void mouseDrag()
    {
        transform.position = Input.mousePosition;
    }

    public void mouseEnter()
    {
        Vector3 scale = new Vector3(1.2f, 1.2f, 1.2f);
        transform.localScale = scale;
    }

    // �ɿ�����ʱ���ò���
    // ���ݿ��Ʋ�ͬ��ⳡ�����巽ʽ��ͬ
    public void mouseUp()
    {
        // ��������ֵ
        /*
            if(xx - cardInfo.cost)
                ...
         */

        // �˴���ʱ��TriggerEffect�������ݵ�������
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.transform.name);

            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.cur_healthPoint -= 5;

                // ʹ����� �����ӻصȴ����л���ֱ������
                CardManager.instance.SendToTempGroup(gameObject);
            }
        }

        // ��δ��⵽Ŀ���������ԭ��ʧЧʱ ����λ��
        CardManager.instance.ReflashLayoutGroup();
        Vector3 scale = Vector3.one;
        transform.localScale = scale;
    }

    public void mouseExit()
    {
        // ��δ��⵽Ŀ���������ԭ��ʧЧʱ ����λ��
        CardManager.instance.ReflashLayoutGroup();
        Vector3 scale = Vector3.one;
        transform.localScale = scale;
    }

    public void RevokeEffect()
    {
        throw new System.NotImplementedException();
    }

    public void TriggerEffect()
    {
        throw new System.NotImplementedException();
    }


}
