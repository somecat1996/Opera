using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffPrototype : MonoBehaviour
{
    public BuffBasicInfomation buffInfo;

    [HideInInspector]
    public string replaceText = "";
    public bool activated = false;

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
            text.Replace("#MainValue", replaceText);
        }else if (text.Contains("%MainValue"))
        {

            text.Replace("%MainValue", (int.Parse(replaceText) * 100).ToString()+"%");
        }

        return text;
    }
}
