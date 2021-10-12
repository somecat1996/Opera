using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCardsSelector : MonoBehaviour
{
    // Ĭ�� ��0��3 Ϊ �� �� �� ��
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


    // ��ʵ��ѡ��ɫ����
    public void DisplayCard(int _charID)
    {
        currentIndex = _charID;
        ReflashAllButton();
        CardManager.instance.LoadSecificCharCardIntoCardList(currentIndex);
    }

    // ˢ�¿���ѡ�����
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
