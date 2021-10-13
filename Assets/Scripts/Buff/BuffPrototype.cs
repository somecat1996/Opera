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
            text = text.Replace("#MainValue", replaceValue.ToString());
        }else if (text.Contains("%MainValue"))
        {
            text = text.Replace("%MainValue", (replaceValue * 100).ToString()+"%");
        }

        return text;
    }

    /// <summary>
    /// 获得主要参数的 *真实值*
    /// </summary>
    /// <returns></returns>
    public virtual float GetTrueMainValue()
    {
        return -1;
    }

    /// <summary>
    /// 获得主要参数
    /// </summary>
    /// <returns></returns>
    public virtual float GetMainValue()
    {
        return -1;
    }
}
