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
    public float returnTime = 0.2f;

    private Vector3 scale_Selected= new Vector3(1.3f,1.3f,1.3f);
    private Vector3 offset_Selected = new Vector3(0, 0, 0);
    private float scaleTime = 0.1f;

    public int originIndex;

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
            // 当选择时 将卡牌置顶
            originIndex = transform.GetSiblingIndex();
            transform.SetSiblingIndex(CardManager.instance.layoutGroup.transform.childCount - 1);

            //transform.localScale = scale_Selected;
            transform.DOScale(scale_Selected, scaleTime);

            // 开启技能效果显示栏
            GUIManager.instance.EnableCardDesc(cardInfo,GetComponent<RectTransform>().position);
            // 向 BDM 发送当前所选择的卡牌信息
            BattleDataManager.instance.UpdateSelectingCard(this);


        }
        else
        {
            // 放弃选择时 当前下标设置成选择之前的状态
            transform.SetSiblingIndex(originIndex);

            transform.DOScale(Vector3.one, scaleTime);
            GUIManager.instance.DisableCardDesc();

            // 用于判断卡牌是否已经送入等待队列 如果否 则DOTween回到原点
            if(transform.parent == CardManager.instance.layoutGroup)
            {
                // 再次进行一次位置判断 强制修正
                if (originPos != CardManager.instance.slotPos[originIndex].position)
                {
                    // 获得该卡牌在父亲下的坐标
                    int index = transform.GetSiblingIndex();
                    originPos = CardManager.instance.slotPos[index].position;
                }

                transform.DOMove(originPos, returnTime);
            }
                

        }

        if (fadeOut)
        {
            SetFadeOutAndShowRange(false);
            SetFadeOutAndShowTargetMarker(false);
        }
    }

    /// <summary>
    /// 使的卡牌可以重新更新在layout中的位置
    /// </summary>
    public void ReflashOriginPos()
    {
        // 获得该卡牌在父亲下的坐标
        int index = transform.GetSiblingIndex();
        originPos = CardManager.instance.slotPos[index].position;
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

    /// <summary>
    /// 设置卡牌完全渐出且启用目标指示器——单体目标卡牌使用
    /// </summary>
    /// <param name="_v"></param>
    public void SetFadeOutAndShowTargetMarker(bool _v)
    {
        if (_v && CheckOnValidArea())
        {
            if (!fadeOut)
            {
                foreach (var i in GetComponentsInChildren<Image>(true))
                {
                    i.DOFade(0, fadeOut_Duration);
                }
                foreach (var i in GetComponentsInChildren<TextMeshProUGUI>(true))
                {
                    i.DOFade(0, fadeOut_Duration);
                }

                fadeOut = true;
                BattleDataManager.instance.SetActiveTargetMarker(true);
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
            }

            if (BattleDataManager.instance.activateTargetMarker)
            {
                BattleDataManager.instance.SetActiveTargetMarker(false);
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
