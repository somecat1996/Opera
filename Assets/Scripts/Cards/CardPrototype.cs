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

    public float fadeOut_Duration = 0.25f;
    public bool fadeOut = false;

    public bool canChangePos = true;
    public Vector3 originPos;
    public float return_Duration = 0.5f;

    private Vector3 scale_Selected= new Vector3(1.3f,1.3f,1.3f);
    private Vector3 offset_Selected = new Vector3(0, 0, 0);
    private float animateSpeed = 0.1f;

    private void Start()
    {
        UpdateGUIInfo();
    }

    public int GetID()
    {
        return cardInfo.id;
    }
    public CardTag.Type GetCardType()
    {
        return cardInfo.cardType;
    }

    /// <summary>
    /// 检测该卡是否为伤害卡
    /// </summary>
    /// <returns></returns>
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
    public void UpdateGUIInfo()
    {
        // 信息载入
        Transform son = transform.GetChild(0);
        son.Find("Image_Rarity").GetComponent<Image>().sprite = CardManager.instance.cardCommonData.sprite_Quality[cardInfo.rarity];
        son.Find("Text_Name").GetComponent<TextMeshProUGUI>().text = cardInfo.cardName;

        Transform grandson = son.transform.Find("Cost");
        grandson.Find("Text_Cost").GetComponent<TextMeshProUGUI>().text = cardInfo.cost.ToString();
        transform.GetComponent<Image>().sprite = cardInfo.illustration;
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
            if (canChangePos && !DOTween.IsTweening(transform))
            {
                canChangePos = false;
                originPos = transform.position;
            }

            //transform.localScale = scale_Selected;
            transform.DOScale(scale_Selected, animateSpeed);

            // 开启技能效果显示栏
            GUIManager.instance.EnableCardDesc(cardInfo,GetComponent<RectTransform>().position);
            // 向 BDM 发送当前所选择的卡牌信息
            BattleDataManager.instance.UpdateSelectingCard(this);


        }
        else
        {
            transform.DOScale(Vector3.one, animateSpeed);
            GUIManager.instance.DisableCardDesc();

            // 用于判断卡牌是否已经送入等待队列 如果否 则DOTween回到原点
            if(transform.parent == CardManager.instance.layoutGroup && !canChangePos)
                transform.DOMove(originPos, return_Duration);

        }

        if (fadeOut)
        {
            SetFadeOutAndShowRange(false);
        }
    }

    /// <summary>
    /// 使的卡牌可以重新更新在layout中的位置
    /// </summary>
    public void ReflashOriginPos()
    {
        canChangePos = true;
    }

    /// <summary>
    /// 设置卡牌完全渐出且启用范围显示器——AOE卡牌使用
    /// </summary>
    /// <param name="_v">设定是否渐出</param>
    public void SetFadeOutAndShowRange(bool _v)
    {
        if (_v && CheckOnValidArea())
        {
            if (!fadeOut)
            {
                foreach(var i in GetComponentsInChildren<Image>(true))
                {
                    i.DOFade(0, fadeOut_Duration);
                }
                foreach(var i in GetComponentsInChildren<TextMeshProUGUI>(true))
                {
                    i.DOFade(0, fadeOut_Duration);
                }

                fadeOut = true;
                BattleDataManager.instance.SetActiveRangeDisplayer(true,cardInfo.radius);
            }
        }
        else
        {
            if (fadeOut)
            {
                foreach (var i in GetComponentsInChildren<Image>(true))
                {
                    i.DOFade(1, fadeOut_Duration);
                }
                foreach (var i in GetComponentsInChildren<TextMeshProUGUI>(true))
                {
                    i.DOFade(1, fadeOut_Duration);
                }

                fadeOut = false;
                BattleDataManager.instance.SetActiveRangeDisplayer(false);
            }
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
