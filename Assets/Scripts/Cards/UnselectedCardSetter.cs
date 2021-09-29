using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnselectedCardSetter : ListCardSetter,ICardOperation
{
    public bool selected = false;
    public Vector3 slotPosition;

    public void mouseDrag()
    {
        transform.position = Input.mousePosition;
    }

    public void mouseEnter()
    {
        if (!selected)
        {
            GUIManager.instance.EnableCardListLaygout(false);
            transform.parent = GUIManager.instance.selectedCardTempParent;
        }
    }

    public void mouseExit()
    {
        if (!selected)
        {
            transform.parent = GUIManager.instance.unselectedCardList.transform;
            GUIManager.instance.EnableCardListLaygout(true);
            GUIManager.instance.ReflashUnselectedCardList();
        }
        else
        {
            transform.position = slotPosition;
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

                if(onSlot && !repeated)
                {

                    transform.position = slot.transform.position;

                    slotPosition = slot.gameObject.transform.position;

                    CardManager.instance.SelectCard(cardInfo);

                    GUIManager.instance.EnableCardListLaygout(true);
                    selected = true;
                    return;
                }
            }

            // 未检测到插槽 返回
            transform.parent = GUIManager.instance.unselectedCardList.transform;
            GUIManager.instance.EnableCardListLaygout(true);
            GUIManager.instance.ReflashUnselectedCardList();
        }
        else
        {
            // 判断是否返回未选择队列
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            if (results.Count != 0)
            {
                foreach (var i in results)
                {
                    if (i.gameObject.layer == LayerMask.NameToLayer("UI_UnselectedList"))
                    {
                        transform.parent = GUIManager.instance.unselectedCardList.transform;
                        CardManager.instance.RemoveSelectedCard(cardInfo);
                        GUIManager.instance.ReflashUnselectedCardList();
                        selected = false;
                        return;
                    }
                }
            }

            transform.position = slotPosition;
        }


    }

    // Start is called before the first frame update

}
