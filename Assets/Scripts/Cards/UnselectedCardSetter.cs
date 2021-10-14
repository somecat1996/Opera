using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UnselectedCardSetter : ListCardSetter,ICardOperation
{
    public bool selected = false;
    public Vector3 slotPosition_Selected;
    public Vector3 slotPosition_Unselected;

    public float moveTime = 0.45f;
    public static List<Transform> unselectedCardList = new List<Transform>();
    public static List<Transform> selectedCardList = new List<Transform>();

    int originIndex;

    [Header("Objects")]
    public GameObject panel_Detail;
    public Image image_Illus;
    public TextMeshProUGUI text_CardName;
    public TextMeshProUGUI text_CardDesc;


    private void Start()
    {
        unselectedCardList.Add(transform);

        // 生成时获得初始位置 并移动至指定方向
        slotPosition_Unselected = GUIManager.instance.slot_UnselectedCard[transform.GetSiblingIndex()].position;
        transform.DOMove(slotPosition_Unselected, moveTime);
        originIndex = transform.GetSiblingIndex();
    }
    private void OnDestroy()
    {
        if (unselectedCardList.Contains(transform))
            unselectedCardList.Remove(transform);
        else
            selectedCardList.Remove(transform);
    }

    /// <summary>
    /// 刷新所有未选择卡牌的位置
    /// </summary>
    private void ReflashPosition()
    {
        for(int i = 0; i < GUIManager.instance.unselectedCardList.transform.childCount; i++)
        {
            if(GUIManager.instance.unselectedCardList.transform.GetChild(i).position != GUIManager.instance.slot_UnselectedCard[i].position)
            {
                GUIManager.instance.unselectedCardList.transform.GetChild(i).GetComponent<UnselectedCardSetter>().slotPosition_Unselected = GUIManager.instance.slot_UnselectedCard[i].position;
                GUIManager.instance.unselectedCardList.transform.GetChild(i).DOMove(GUIManager.instance.slot_UnselectedCard[i].position, moveTime);
                originIndex = transform.GetSiblingIndex();
            }
        }
    }

    public void mouseDrag()
    {
        transform.position = Input.mousePosition;
    }

    public void mouseEnter()
    {
        DisplayCardDesc();
    }

    public void mouseExit()
    {
        transform.SetSiblingIndex(originIndex);
        ReturnToSlotPosition();
    }

    public void mouseDown()
    {
        originIndex = transform.GetSiblingIndex();
        transform.SetSiblingIndex(transform.parent.childCount - 1);
    }

    public void DisplayCardDesc()
    {
        panel_Detail.SetActive(true);

        image_Illus.sprite = cardInfo.illustration;
        text_CardName.text = cardInfo.cardName;
        text_CardDesc.text = cardInfo.GetDesc();
    }

    /// <summary>
    /// 根据选择状态返回相应插槽位置
    /// </summary>
    public void ReturnToSlotPosition()
    {
        if (selected)
        {
            transform.DOMove(slotPosition_Selected, moveTime);
        }
        else
        {
            transform.DOMove(slotPosition_Unselected, moveTime);
        }
    }

    public void mouseUp()
    {
        // 此处判断是否在插槽中
        if (!selected)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            if (results.Count != 0)
            {
                bool onSlot = false;
                bool repeated = false;
                GameObject slot = null;

                foreach(var i in results)
                {
                    if(i.gameObject.tag == "Card" && i.gameObject != gameObject)
                    {
                        repeated = true;
                    }
                    if (i.gameObject.layer == LayerMask.NameToLayer("UI_CardSlot"))
                    {
                        onSlot = true;
                        slot = i.gameObject;
                    }
                }

                // 位于插槽上且不重复 选择成功
                if(onSlot && !repeated)
                {

                    transform.position = slot.transform.position;
                    slotPosition_Selected = slot.gameObject.transform.position;
                    CardManager.instance.SelectCard(cardInfo);
                    selected = true;

                    transform.parent = GUIManager.instance.selectedCardTempParent.transform; // 转移父母

                    ReturnToSlotPosition(); // 卡牌返回新插槽位置
                    unselectedCardList.Remove(transform);
                    selectedCardList.Add(transform);
                    ReflashPosition(); // 刷新所有未选择卡牌位置

                    return;
                }
            }

            // 未检测到插槽 返回
            mouseExit();

        }
        // 已选择的卡牌
        else
        {
            // 判断是否返回未选择队列
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            if (results.Count != 0)
            {
                bool repeated = false;

                foreach (var i in results)
                {
                    // 检测到返回未选择卡牌插槽
                    if (i.gameObject.layer == LayerMask.NameToLayer("UI_UnselectedList"))
                    {
                        transform.parent = GUIManager.instance.unselectedCardList.transform;

                        unselectedCardList.Add(transform);
                        selectedCardList.Remove(transform);
                        ReflashPosition();
                        CardManager.instance.RemoveSelectedCard(cardInfo);
                        
                        
                        selected = false;
                        return;
                    }

                    if (i.gameObject.tag == "Card" && i.gameObject != gameObject)
                    {
                        repeated = true;
                    }

                    // 更换新插槽
                    if (!repeated && i.gameObject.layer == LayerMask.NameToLayer("UI_CardSlot"))
                    {
                        slotPosition_Selected = i.gameObject.transform.position;
                        ReturnToSlotPosition();

                        return;
                    }
                }
            }

            mouseExit();
        }


    }

}
