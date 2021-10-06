using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;

public class CardPrototype : MonoBehaviour
{
    public CardBasicInfomation cardInfo;

　　

    private Vector3 scale_Selected= new Vector3(1.3f,1.3f,1.3f);
    private Vector3 offset_Selected = new Vector3(0, 0, 0);
    private float animateSpeed = 0.1f;

    private void Start()
    {
        UpdateInfo();
    }

    public int GetID()
    {
        return cardInfo.id;
    }
    public CardTag.Type GetCardType()
    {
        return cardInfo.cardType;
    }

    // 检测该卡是否为伤害卡
    public bool CheckIfDamageCard()
    {
        if(GetCardType() == CardTag.Type.Magic || GetCardType() == CardTag.Type.Physics)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 更新UI信息
    /// </summary>
    public void UpdateInfo()
    {
        // 信息载入
        Transform son = transform.GetChild(0);
        son.Find("Image_Rarity").GetComponent<Image>().sprite = CardManager.instance.cardCommonData.sprite_Quality[cardInfo.rarity];
        son.Find("Text_Name").GetComponent<TextMeshProUGUI>().text = cardInfo.cardName;

        Transform grandson = son.transform.Find("Cost");
        grandson.Find("Text_Cost").GetComponent<TextMeshProUGUI>().text = cardInfo.cost.ToString();
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

            GUIManager.instance.EnableCardDesc(cardInfo,GetComponent<RectTransform>().position);
        }
        else
        {
            transform.DOScale(Vector3.one, animateSpeed);

            GUIManager.instance.DisableCardDesc();
        }
    }
    
    public bool CheckOnValidArea()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        if (results.Count != 0)
        {
            foreach (var i in results)
            {
                if (i.gameObject.layer == LayerMask.NameToLayer("UI_ValidArea"))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
