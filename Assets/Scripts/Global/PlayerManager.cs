using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance; 

    [Header("Real-Time Data")]
    public CharacterType.CharacterTag cur_Character = CharacterType.CharacterTag.Sheng;
    public PlayerData data;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GUIManager.instance.ChangeMoneyText(data.money);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ��ʼ������
    public void InitializeData()
    {
        data.Initialize();
        GUIManager.instance.ChangeMoneyText(data.money);
    }

    /// <summary>
    /// ͨ��������ʽ�޸Ļ���
    /// </summary>
    /// <param name="_increment">����</param>
    /// <returns></returns>
    public bool ChangeMoney(int _increment)
    {
        if(data.money + _increment < 0)
        {
            return false;
        }

        data.money += _increment;
        GUIManager.instance.ChangeMoneyText(data.money);
        return true;
    }

    //  �޸ĵ�ǰ��ɫ
    public void SwitchCharacter(CharacterType.CharacterTag _v)
    {
        if (_v == cur_Character)
            return;

        cur_Character = _v;

        // �����ѡ���Ƶ���Ϣ
        GUIManager.instance.ClearUnselectedCardList();
        CardManager.instance.ClearSelectedCard();

        // ��������ͨ�ÿ��� ����ͨ�ÿ�����δ��� �������ɫ����
        CardManager.instance.LoadAllCardIntoUnselectedList();

    }

    // �����ؿ�
    public void UnlockLevel(int _id)
    {
        data.levelStatus[_id] = true;
    }
}
