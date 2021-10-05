using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{
    public static GUIManager instance;

    [Header("Configuration")]
    public float offsetX_CardDesc = 115;

    [Header("CardList Objects")]
    public GameObject cardList; // 卡牌列表容器
    public GameObject listCardSample; // 列表卡牌样本
    [Space]
    [Header("UnselectedCardList Objects")]
    public GameObject unselectedCardList; // 待选卡牌容器
    public LayoutGroup unselectedCardList_LayoutGroup; // 待选卡牌容器的排序组件
    public Mask unselectedCardList_Mask; // 待选卡牌容器遮罩组件
    public GameObject unselectedCardTemplatee; // 待选卡牌容器模板
    public Transform selectedCardTempParent; // 选中卡牌临时(容器)父母节点

    [Header("CardDetails Object")]
    public ListCardSetter selectedCard;

    public TextMeshProUGUI cardDetail_Cost;
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

    [Header("Other Objects")]
    public List<TextMeshProUGUI> text_Money = new List<TextMeshProUGUI>();

    public Button button_Contiune;

    [Header("GameScene Objects")]
    public Slider player_PowerPoint;
    public TextMeshProUGUI text_PowerPoint;
    public Slider boss_HealthPoint;
    public GameObject panel_CardDesc;
    public TextMeshProUGUI text_CardDesc;



    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        player_PowerPoint.maxValue = PlayerManager.instance.max_PowerPoint;
    }

    void Update()
    {
        List<int> a = new List<int>();
        a.Add(1);
        a.Add(2);
        int b = 2;
        if (Input.GetMouseButtonDown(0))
        {
            Test(b);
        }
    }

    void Test(object _d)
    {
        if(_d.GetType() != typeof(List<int>))
        {
            Debug.Log("不匹配！");
            return;
        }
            
        foreach(var i in (List<int>)_d)
            Debug.Log(i);
    }

    

    public void UpdateBossHealthPoint(float _v)
    {
        boss_HealthPoint.value = _v;
    }
    public void UpdatePowerPoint(float _v)
    {
        player_PowerPoint.value = _v;
        text_PowerPoint.text = ((int)_v).ToString();
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
        go.transform.localScale = Vector3.one;
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
        cardDetail_ColorBar.sprite = _lcs.colorBar.sprite;

        cardDetail_Cost.text = _lcs.text_Cost.text;

        if (_lcs.cardInfo.level == CardManager.instance.cardCommonData.max_Level || _lcs.cardInfo.level == 0)
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

        cardDetail_Text_Description.text = _lcs.cardInfo.GetDesc();
        cardDetail_Text_Story.text = _lcs.cardInfo.story;
    }

    public void UpgradeCard()
    {
        // 实际数值改变及判断不在GUI管理器中
        selectedCard.cardInfo.UpgradeMainValue(); 

        // 刷新界面信息
        selectedCard.SetCardInfo(selectedCard.cardInfo); // 自带刷新卡牌列表界面的信息
        SetCardDetail(selectedCard); // 刷新细节列表的信息
    }

    // 重新排序未选中卡牌队列
    public void ReflashUnselectedCardList()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(unselectedCardList.GetComponent<RectTransform>());
    }
    // 清空未选择卡牌列表 及 *已选卡牌列表*
    public void ClearUnselectedCardList()
    {
        for(int i = 0; i < unselectedCardList.transform.childCount; i++)
        {
            Destroy(unselectedCardList.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < selectedCardTempParent.transform.childCount; i++)
        {
            Destroy(selectedCardTempParent.transform.GetChild(i).gameObject);
        }
    }


    // 生成一张卡牌并放入未选择列表   
    public void AddUnselectedCard(CardBasicInfomation _card)
    {
        GameObject go = Instantiate(unselectedCardTemplatee);
        go.GetComponent<UnselectedCardSetter>().SetCardInfo(_card);
        go.transform.parent = unselectedCardList.transform;
        go.transform.localScale = Vector3.one;
    }

    // 设置是否启用未选择卡牌序列的排序组件
    public void EnableCardListLaygout(bool _v)
    {
        unselectedCardList_LayoutGroup.enabled = _v;
        unselectedCardList_Mask.enabled = _v;
    }

    public void UpdateMoneyText(int _v)
    {
        foreach(var i in text_Money)
        {
            i.text = _v.ToString();
        }

    }

    // 战斗界面卡牌介绍
    public void EnableCardDesc(CardBasicInfomation _cardInfo,Vector3 _pos)
    {
        panel_CardDesc.GetComponent<RectTransform>().position = _pos;

        string tag = "[ " + _cardInfo.GetTag() + " ]"; 

        text_CardDesc.text = tag + '\n' + _cardInfo.GetDesc();
        panel_CardDesc.SetActive(true);
    }
    public void DisableCardDesc()
    {
        panel_CardDesc.SetActive(false);
    }

    // 锁定继续按钮
    public void LockButton_Continue(bool _v)
    {
        button_Contiune.interactable = !_v;
    }
}
