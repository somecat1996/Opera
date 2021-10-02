using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [Header("Configuration")]
    public float max_PowerPoint;
    public float recoverySpeed_PowerPoint;
    [Space]
    public float max_HealthPoint;

    [Header("Real-Time Data")]
    public float cur_PowerPoint = 0;
    public float cur_HealthPoint = 0;

    [Space]
    public CharacterType.CharacterTag cur_Character = CharacterType.CharacterTag.Dan;
    public PlayerData data;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GUIManager.instance.ChangeMoneyText(data.money);

        ResetBattleData();
    }

    // Update is called once per frame
    void Update()
    {
        cur_PowerPoint += recoverySpeed_PowerPoint * Time.deltaTime;
        cur_PowerPoint = Mathf.Clamp(cur_PowerPoint, 0, max_PowerPoint);
        GUIManager.instance.SetPowerPoint(cur_PowerPoint);
        
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

    /// <summary>
    /// ͨ�������޸�����ֵ
    /// </summary>
    /// <param name="_v">����</param>
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
    /// ͨ�������޸ķ�Χֵ
    /// </summary>
    /// <param name="_v">����</param>
    public void ChangeHealthPoint(float _v)
    {
        cur_HealthPoint += _v;
        cur_HealthPoint = Mathf.Clamp(cur_HealthPoint, 0, max_HealthPoint);
    }

    /// <summary>
    /// ����ս�����ݡ��������¹ؿ�ʱ����
    /// </summary>
    public void ResetBattleData()
    {
        cur_HealthPoint = max_HealthPoint;
        cur_PowerPoint = 0;
    }

    // �����ؿ�
    public void UnlockLevel(int _id)
    {
        data.levelStatus[_id] = true;
    }
}
