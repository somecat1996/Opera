using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ListCardSetter : MonoBehaviour
{
    [HideInInspector]
    public CardBasicInfomation cardInfo;

    public Image illustration; 

    [Header("Label")]
    public TextMeshProUGUI text_Name;
    public TextMeshProUGUI text_Level;
    public Image colorBar;

    [Header("UngradeBar")]
    public TextMeshProUGUI text_Quantity;
    public TextMeshProUGUI text_Demanded;
    public GameObject sprit;
    public Slider upgradeSlider;
    public GameObject label_Max;
    

    // 载入卡牌信息并初始化数据
    public void SetCardInfo(CardBasicInfomation _cardInfo)
    {
        cardInfo = _cardInfo;

        illustration.sprite = cardInfo.illustration;

        text_Name.text = cardInfo.cardName;
        text_Level.text = cardInfo.level.ToString();
        colorBar.color = CardManager.instance.cardCommonData.color_Quality[cardInfo.rarity];

        if(cardInfo.level == CardManager.instance.cardCommonData.max_Level || cardInfo.level == 0)
        {
            text_Demanded.gameObject.SetActive(false);
            text_Quantity.gameObject.SetActive(false);
            upgradeSlider.gameObject.SetActive(false);
            sprit.gameObject.SetActive(false);

            label_Max.gameObject.SetActive(true);
            label_Max.GetComponent<TextMeshProUGUI>().enabled = true;
        }
        else
        {
            SetDemanded(CardManager.instance.cardCommonData.upgrade_Demanded[cardInfo.level]);
            SetQuantity(cardInfo.quantity);
        }
    }

    private void SetDemanded(int _v)
    {
        text_Demanded.text = _v.ToString();
        upgradeSlider.maxValue = _v;
    }
    private void SetQuantity(int _v)
    {
        text_Quantity.text = _v.ToString();
        upgradeSlider.value = _v;
    }

    public void DisplayDetail()
    {
        GUIManager.instance.SetCardDetail(this);
    }
}
