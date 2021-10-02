using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardPrototype : MonoBehaviour
{
    public CardBasicInfomation cardInfo;

    private Vector3 scale_Selected= new Vector3(1.3f,1.3f,1.3f);
    private Vector3 offset_Selected = new Vector3(0, 0, 0);
    private float animateSpeed = 0.1f;

    public int GetID()
    {
        return cardInfo.id;
    }

    public void LoadCardInfo(string _path)
    {
        cardInfo = Resources.Load<CardBasicInfomation>(_path);

        GetComponent<Image>().sprite = cardInfo.illustration;
    }

    public void SetOnSelected(bool _v)
    {
        if (_v)
        {
            //transform.localScale = scale_Selected;
            transform.DOScale(scale_Selected, animateSpeed);
        }
        else
        {
            transform.DOScale(Vector3.one, animateSpeed);
        }
    }
    
}
