using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class BuffInfoDisplayer : MonoBehaviour
{
    public BuffPrototype buff;

    [Space]
    public TextMeshProUGUI text;
    public GameObject panel_Desc;

    public void SetInfo(BuffPrototype _buff)
    {
        buff = _buff;

        GetComponent<Image>().sprite = buff.buffInfo.icon;
    }

    public void MouseEnter()
    {
        SetActiveDescPanel(true);
    }

    public void MouseExit()
    {
        SetActiveDescPanel(false);
    }

    public void SetActiveDescPanel(bool _v)
    {
        if (_v)
        {
            text.text = buff.GetDesc();
            panel_Desc.transform.position = transform.position;
        }

        panel_Desc.SetActive(_v);
        
    }
}
