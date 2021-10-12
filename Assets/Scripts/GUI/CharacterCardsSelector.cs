using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCardsSelector : MonoBehaviour
{
    // 默认 从0到3 为 生 旦 净 丑
    public Image[] buttonImage = new Image[4];
    public int currentIndex = -1;

    public Sprite sprite_Normal;
    public Sprite sprite_Selected;

    void Start()
    {
        currentIndex = (int)PlayerManager.instance.cur_Character;
        DisplayCard(currentIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // 现实所选角色卡牌
    public void DisplayCard(int _charID)
    {
        currentIndex = _charID;
        ReflashAllButton();
        CardManager.instance.LoadSecificCharCardIntoCardList(currentIndex);
    }

    // 刷新卡牌选择情况
    public void ReflashAllButton()
    {
        for(int i = 0;i < 4;i++)
        {
            if (i == currentIndex)
            {
                buttonImage[i].sprite = sprite_Selected;
            }
            else
            {
                buttonImage[i].sprite = sprite_Normal;
            }
        }
    }
}
