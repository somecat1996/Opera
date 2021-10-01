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

    // 初始化数据
    public void InitializeData()
    {
        data.Initialize();
        GUIManager.instance.ChangeMoneyText(data.money);
    }

    /// <summary>
    /// 通过增量形式修改货币
    /// </summary>
    /// <param name="_increment">增量</param>
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

    //  修改当前角色
    public void SwitchCharacter(CharacterType.CharacterTag _v)
    {
        if (_v == cur_Character)
            return;

        cur_Character = _v;

        // 清除已选择卡牌的信息
        GUIManager.instance.ClearUnselectedCardList();
        CardManager.instance.ClearSelectedCard();

        // 重新载入通用卡牌 由于通用卡牌暂未完成 故载入角色卡牌
        CardManager.instance.LoadAllCardIntoUnselectedList();

    }

    // 解锁关卡
    public void UnlockLevel(int _id)
    {
        data.levelStatus[_id] = true;
    }
}
