using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [Header("Configuration")]
    public float max_PowerPoint;
    public float default_RecoverySpeed_PowerPoint;
    [Space]
    public float max_HealthPoint;

    [Header("Real-Time Data")]
    public float cur_PowerPoint = 0;
    public float cur_HealthPoint = 0;
    public float cur_RecoverySpeed_PowerPoint = 0;

    [Space]
    public CharacterType.CharacterTag cur_Character = CharacterType.CharacterTag.Dan;
    public PlayerData data;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GUIManager.instance.UpdateMoneyText(data.money);

        ResetBattleData();
    }

    // Update is called once per frame
    void Update()
    {
        cur_PowerPoint += cur_RecoverySpeed_PowerPoint * Time.deltaTime;
        cur_PowerPoint = Mathf.Clamp(cur_PowerPoint, 0, max_PowerPoint);
        GUIManager.instance.UpdatePowerPoint(cur_PowerPoint);
        
    }

    // 初始化数据
    public void InitializeData()
    {
        data.Initialize();
        GUIManager.instance.UpdateMoneyText(data.money);
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
        GUIManager.instance.UpdateMoneyText(data.money);
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

    /// <summary>
    /// 通过增量修改心流值
    /// </summary>
    /// <param name="_v">增量</param>
    public bool ChangePowerPoint(float _v)
    {
        if(cur_PowerPoint + _v < 0)
        {
            return false;
        }
        else
        {
            cur_PowerPoint += _v;
            cur_PowerPoint = Mathf.Clamp(cur_PowerPoint, 0, max_PowerPoint);
            return true;
        }
    }

    /// <summary>
    /// 通过增量修改氛围值
    /// </summary>
    /// <param name="_v">增量</param>
    public void ChangeHealthPoint(float _v)
    {
        cur_HealthPoint += _v;
        cur_HealthPoint = Mathf.Clamp(cur_HealthPoint, 0, max_HealthPoint);
    }
    
    public void ChangeRecoverySpeed_PowerPoint(float _v)
    {
        cur_RecoverySpeed_PowerPoint += _v;
        cur_RecoverySpeed_PowerPoint = Mathf.Clamp(cur_RecoverySpeed_PowerPoint, 0, Mathf.Infinity);
    }

    /// <summary>
    /// 重设战斗数据——进入新关卡时调用
    /// </summary>
    public void ResetBattleData()
    {
        cur_HealthPoint = max_HealthPoint;
        cur_PowerPoint = 0;
        cur_RecoverySpeed_PowerPoint = default_RecoverySpeed_PowerPoint;
    }

    // 解锁关卡
    public void UnlockLevel(int _id)
    {
        data.levelStatus[_id] = true;
    }
}
