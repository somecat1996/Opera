using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffPrototype : MonoBehaviour
{
    public BuffBasicInfomation buffInfo;

    [HideInInspector]
    public string replaceText = "";
    public bool activated = false;

    // 初始化数据
    virtual public void ReflashData() { }

    public int GetID()
    {
        return buffInfo.id;
    }

    // 如果文本中包含#MainValue 则将其替换
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
