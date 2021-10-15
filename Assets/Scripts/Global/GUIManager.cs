using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{
    public static GUIManager instance;

    [Header("Configuration")]
    public float offsetX_CardDesc = 300;

    [Header("CardList Objects")]
    public GameObject cardList; // �����б�����
    public GameObject listCardSample; // �б�������
    [Space]
    [Header("UnselectedCardList Objects")]
    public Transform slotLayout; // ��ѡ���Ʋ������
    public List<RectTransform> slot_UnselectedCard; // ��ѡ��������������λ��
    public GameObject unselectedCardList; // ��ѡ��������
    public GameObject unselectedCardTemplatee; // ��ѡ��������ģ��
    public Transform selectedCardTempParent; // ѡ�п�����ʱ(����)��ĸ�ڵ�

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
    public GameObject canvas;

    [Header("GameScene Objects")]
    public Image player_PowerPoint;
    public TextMeshProUGUI text_PowerPoint;
    public Image boss_HealthPoint;
    public GameObject panel_CardDesc;
    public TextMeshProUGUI text_CardDesc;
    [Space]
    public GameObject panel_BuffIcon;
    public GameObject prefab_BuffIcon;

    [Header("System Objects")]
    public Transform panel_Common;
    [Space]
    public Transform spawnPos_SystemText;
    public GameObject prefab_SystemText;

    [Header("Evaluation UI Objects")]
    public GameObject panel_Evaluation;
    public TextMeshProUGUI text_GameResult;
    public TextMeshProUGUI text_Loot_Money;
    public TextMeshProUGUI text_GameTime;
    public Image image_Character;
    public ListCardSetter[] card_Loot = new ListCardSetter[3];


    private void Awake()
    {
        instance = this;

        // ��ȡ��ѡ���Ʋ������
        for(int i = 0; i < slotLayout.childCount; i++)
        {
            slot_UnselectedCard.Add(slotLayout.GetChild(i).GetComponent<RectTransform>());
        }
    }

    void Start()
    {
        
    }

    void Update()
    {

    }


    
    /// <summary>
    /// �޸�BOSSѪ�� ����ٷ�ֵ
    /// </summary>
    /// <param name="_v"></param>
    public void UpdateBossHealthPoint(float _v)
    {
        boss_HealthPoint.fillAmount = _v;
    }
    /// <summary>
    /// �޸�����ֵ ����ٷ�ֵ �� ��ʵֵ
    /// </summary>
    /// <param name="_v"></param>
    public void UpdatePowerPoint(float _v,float _trueV)
    {
        player_PowerPoint.fillAmount = _v;
        text_PowerPoint.text = ((int)_trueV).ToString();
    }

    // ��տ����б�
    public void ClearCardList()
    {
        for(int i = 0; i < cardList.transform.childCount; i++)
        {
            Destroy(cardList.transform.GetChild(i).gameObject);
        }
    }

    // ��ȡ���Ʋ����뿨���б�
    public void LoadCardIntoList(CardBasicInfomation _cardInfo)
    {
        GameObject go = GameObject.Instantiate(listCardSample);
        go.transform.parent = cardList.transform;
        go.transform.localScale = Vector3.one;
        go.GetComponent<ListCardSetter>().SetCardInfo(_cardInfo);

        // *****δ֪�������BUG ��ʱ�������*****
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
    
    // չʾ����ϸ�� ���б������ݴ洢��ϸ�ڻ��������
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
        // ʵ����ֵ�ı估�жϲ���GUI��������
        selectedCard.cardInfo.UpgradeMainValue(); 

        // ˢ�½�����Ϣ
        selectedCard.SetCardInfo(selectedCard.cardInfo); // �Դ�ˢ�¿����б�������Ϣ
        SetCardDetail(selectedCard); // ˢ��ϸ���б����Ϣ
    }

    // ���δѡ�����б� �� *��ѡ�����б�* ��ʵ�����
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


    // ����һ�ſ��Ʋ�����δѡ���б�   
    public void AddUnselectedCard(CardBasicInfomation _card)
    {
        GameObject go = Instantiate(unselectedCardTemplatee);
        go.GetComponent<UnselectedCardSetter>().SetCardInfo(_card);
        go.transform.parent = unselectedCardList.transform;
        go.transform.localScale = Vector3.one;
    }

    public void UpdateMoneyText(int _v)
    {
        foreach(var i in text_Money)
        {
            i.text = _v.ToString();
        }

    }

    // ս�����濨�ƽ���
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

    // ����������ť
    public void LockButton_Continue(bool _v)
    {
        button_Contiune.interactable = !_v;
    }

    // �ر�����UI ������ս��GUI
    public void DisableAllGUI()
    {
        for(int i = 0;i< canvas.transform.childCount; i++)
        {
            if (canvas.transform.GetChild(i).tag == "GUI_Stay")
                continue;

            canvas.transform.GetChild(i).gameObject.SetActive(false);
        }
        
    }

    // ����Buffͼ��
    public GameObject SpawnBuffIcon(BuffPrototype _buff)
    {
        GameObject go = Instantiate(prefab_BuffIcon);
        go.transform.parent = panel_BuffIcon.transform;
        go.transform.localScale = Vector3.one;
        go.GetComponent<BuffInfoDisplayer>().SetInfo(_buff);

        return go;

    }

    // �������buffͼ�ꡪ���˳�ս��ʱʹ��
    public void ClearAllBuffIcon()
    {
        for(int i = 0;i< panel_BuffIcon.transform.childCount; i++)
        {
            Destroy(panel_BuffIcon.transform.GetChild(i));
        }
    }

    /// <summary>
    /// ����ϵͳ�ı�
    /// </summary>
    /// <param name="_text"></param>
    public void SpawnSystemText(string _text)
    {
        GameObject go = Instantiate(prefab_SystemText);
        go.transform.parent = panel_Common;
        go.transform.localScale = Vector3.one;
        go.transform.position = spawnPos_SystemText.position;
        go.GetComponent<SystemText>().SetText(_text);
    }

    /// <summary>
    /// ��ʾ��Ϸ���㻭��
    /// </summary>
    public void EnableGameResult(bool _playerVictory,float gameTime,int _money,List<CardBasicInfomation> _card)
    {
        text_GameResult.text = _playerVictory == true ? "ʤ ��!" : "�� ��!";
        text_Loot_Money.text = _money.ToString();

        int min = (int)(gameTime / 60);
        int sec = (int)(gameTime % 60);
        text_GameTime.text = min + "��" + sec + "��";

        image_Character.sprite = PlayerManager.instance.cur_CharacterInfo.illustration;

        // ������Ϣ
        for(int i = 0; i < _card.Count; i++)
        {
            card_Loot[i].SetCardInfo(_card[i]);
        }

        panel_Evaluation.SetActive(true);
    }
    /// <summary>
    /// �رս��㻭��
    /// </summary>
    public void DisableGameResult()
    {
        panel_Evaluation.SetActive(false);
    }

}
