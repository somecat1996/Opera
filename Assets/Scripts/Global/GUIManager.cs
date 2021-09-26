using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{
    public static GUIManager instance;

    [Header("Objects")]
    public GameObject cardList; // 卡牌列表容器
    public GameObject listCardSample; // 列表卡牌样本

    [Header("CardDetails Object")]
    public ListCardSetter selectedCard;

    public TextMeshProUGUI cardDetail_Text_Name;
    public TextMeshProUGUI cardDetail_Text_Level;
    public Image cardDetail_ColorBar;

    public Image cardDetail_Illustration;
    public Image cardDetail_Illustration_Completed;

    public TextMeshProUGUI cardDetail_Text_Quantity;
    public TextMeshProUGUI cardDetail_Text_Demanded;
    public Slider cardDetail_UpgradeSlider;
    public GameObject cardDetail_Sprit;

    public TextMeshProUGUI cardDetail_Text_Description;
    public TextMeshProUGUI cardDetail_Text_Story;

    public GameObject cardDetail_Label_Max;
        

    [Header("Temp")]
    public Slider boss_HealthPoint;



    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ClearCardList();
        }
    }

    public void SetBossHealthPoint(float _v)
    {
        boss_HealthPoint.value = _v;
    }

    // 清空卡牌列表
    public void ClearCardList()
    {
        for(int i = 0; i < cardList.transform.childCount; i++)
        {
            Destroy(cardList.transform.GetChild(i).gameObject);
        }
    }

    // 读取卡牌并放入卡牌列表
    public void LoadCardIntoList(CardBasicInfomation _cardInfo)
    {
        GameObject go = GameObject.Instantiate(listCardSample);
        go.transform.parent = cardList.transform;
        go.GetComponent<ListCardSetter>().SetCardInfo(_cardInfo);

        // *****未知组件启用BUG 暂时解决方法*****
        foreach(var i in cardList.GetComponentsInChildren<Image>())
        {
            i.enabled = true;
        }
        foreach (var i in cardList.GetComponentsInChildren<Button>())
        {
            i.enabled = true;
        }
        foreach (var i in cardList.GetComponentsInChildren<TextMeshProUGUI>())
        {
            i.enabled = true;
        }
    }
    
    // 展示卡牌细节 将列表卡牌数据存储至细节画面对象中
    public void SetCardDetail(ListCardSetter _lcs)
    {
        selectedCard = _lcs;

        cardDetail_Illustration_Completed.sprite = _lcs.illustration.sprite;
        cardDetail_Illustration.sprite = _lcs.illustration.sprite;

        cardDetail_Text_Name.text = _lcs.cardInfo.cardName;
        cardDetail_Text_Level.text = _lcs.text_Level.text;
        cardDetail_ColorBar.color = _lcs.colorBar.color;

        if(_lcs.cardInfo.level == CardManager.instance.cardCommonData.max_Level || _lcs.cardInfo.level == 0)
        {
            cardDetail_Label_Max.gameObject.SetActive(true);

            cardDetail_UpgradeSlider.gameObject.SetActive(false);
            cardDetail_Text_Quantity.gameObject.SetActive(false);
            cardDetail_Text_Demanded.gameObject.SetActive(false);
            cardDetail_Sprit.gameObject.SetActive(false);
        }
        else
        {
            cardDetail_Label_Max.gameObject.SetActive(false);

            cardDetail_UpgradeSlider.gameObject.SetActive(true);
            cardDetail_Text_Quantity.gameObject.SetActive(true);
            cardDetail_Text_Demanded.gameObject.SetActive(true);
            cardDetail_Sprit.gameObject.SetActive(true);

            cardDetail_Text_Demanded.text = _lcs.text_Demanded.text;
            cardDetail_Text_Quantity.text = _lcs.text_Quantity.text;
            cardDetail_UpgradeSlider.maxValue = _lcs.upgradeSlider.maxValue;
            cardDetail_UpgradeSlider.value = _lcs.upgradeSlider.value;
        }

        cardDetail_Text_Description.text = _lcs.cardInfo.description;
        cardDetail_Text_Story.text = _lcs.cardInfo.story;
    }

    public void UpgradeCard()
    {
        // 实际数值改变及判断不在GUI管理器中
        selectedCard.cardInfo.UpgradeMainValue(); 

        // 刷新界面信息
        selectedCard.SetCardInfo(selectedCard.cardInfo);
        SetCardDetail(selectedCard);
    }
}
