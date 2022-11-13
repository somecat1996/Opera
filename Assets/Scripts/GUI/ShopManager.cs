using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Configuration")]
    public float reflashTimer_Origin = 300;

    public int[] pricePerRarity = new int[5];

    [Header("Real-Time Data")]
    public int cur_SelectedItemIndex;

    public float reflashTimer;
    [Space]
    public List<CardBasicInfomation> itemCardInfo = new List<CardBasicInfomation>();

    [Header("Obejcts")]
    public List<ListCardSetter> setter_Item = new List<ListCardSetter>();
    public List<GameObject> image_Plat = new List<GameObject>();

    public GameObject panel_ItemDetail;
    public ListCardSetter setter_ItemInDetail;
    public TextMeshProUGUI text_CardName;
    public TextMeshProUGUI text_Desc;
    public TextMeshProUGUI text_Price;

    public TextMeshProUGUI text_Timer;

    // Start is called before the first frame update
    void Start()
    {
        ReflashItem();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            int min = (int)(reflashTimer / 60);
            int sec = (int)(reflashTimer % 60);

            string text = min.ToString() + "分" + (sec < 10 ? "0" + sec.ToString() : sec.ToString()) + "秒";
            text_Timer.text = text;

            reflashTimer -= Time.deltaTime;
            reflashTimer = Mathf.Clamp(reflashTimer, 0, Mathf.Infinity);
            
            if(reflashTimer == 0)
            {
                ReflashItem();
            }
        }
    }

    /// <summary>
    /// 通过消耗金钱来强制刷新
    /// </summary>
    public void ReflashItenByMoney()
    {
        if (PlayerManager.instance.ChangeMoney(-50))
        {
            ReflashItem();
        }
        else
        {
            GUIManager.instance.SpawnSystemText("金钱不足!");
        }
    }

    /// <summary>
    /// 刷新商品
    /// </summary>
    public void ReflashItem()
    {
        reflashTimer = reflashTimer_Origin;

        panel_ItemDetail.SetActive(false);

        itemCardInfo = CardManager.instance.GetCardsRandomly(8);

        for(int i = 0; i < setter_Item.Count; i++)  
        {
            setter_Item[i].SetCardInfo(itemCardInfo[i]);
        }

        foreach (var i in image_Plat)
            i.SetActive(false);
    }

    /// <summary>
    /// 显示物品详细信息
    /// </summary>
    /// <param name="_index">商品下标</param>
    public void DisplayItemDetail(int _index)
    {
        cur_SelectedItemIndex = _index;

        text_CardName.text = itemCardInfo[_index].cardName;
        text_Desc.text = itemCardInfo[_index].GetDesc();
        text_Price.text = pricePerRarity[itemCardInfo[_index].rarity].ToString();

        setter_ItemInDetail.SetCardInfo(itemCardInfo[_index]);

        panel_ItemDetail.SetActive(true);
    }

    /// <summary>
    /// 购买物品
    /// </summary>
    public void Purchase()
    {
        int price = pricePerRarity[itemCardInfo[cur_SelectedItemIndex].rarity];

        // 购买成功 关闭画面 对应物品数量增加
        if (PlayerManager.instance.ChangeMoney(-price))
        {
            itemCardInfo[cur_SelectedItemIndex].quantity++;
            GUIManager.instance.SpawnSystemText("购买成功!");

            panel_ItemDetail.SetActive(false);
            image_Plat[cur_SelectedItemIndex].SetActive(true);
        }
        else
        {
            GUIManager.instance.SpawnSystemText("金币不足!");
        }
    }
}
