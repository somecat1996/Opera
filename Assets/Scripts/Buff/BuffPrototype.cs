using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffPrototype : MonoBehaviour
{
    public BuffBasicInfomation buffInfo;

    [HideInInspector]
    public float replaceValue = 0;
    public bool activated = false;

    public GameObject buffGUIicon;
    // ��ʼ������
    virtual public void ReflashData() { }

    public int GetID()
    {
        return buffInfo.id;
    }

    // ����ı��а���#MainValue �����滻
    public string GetDesc()
    {
        string text = buffInfo.description;
        if (text.Contains("#MainValue"))
        {
            text = text.Replace("#MainValue", replaceValue.ToString());
        }else if (text.Contains("%MainValue"))
        {
            text = text.Replace("%MainValue", (replaceValue * 100).ToString()+"%");
        }

        return text;
    }
}
