using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [Header("Configuration")]
    public float max_PowerPoint;
    public float default_RecoverySpeed_PowerPoint;
    public float lowBoundary_RecoverySpeed = 0.5f;
    [Space]
    public float max_HealthPoint;

    [Header("Real-Time Data")]
    public float cur_PowerPoint = 0;
    public float cur_HealthPoint = 0;
    public float cur_RecoverySpeed_PowerPoint = 0;

    [Space]
    [Header("Player")]
    public GameObjectBase player;


    [Space]
    public CharacterType.CharacterTag cur_Character = CharacterType.CharacterTag.Dan;
    public CharacterBasicInfomation cur_CharacterInfo;

    [Space]
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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            player.Hurt(2);
        }
    }

    /// <summary>
    /// ������Ҷ��󲢴����������ֵ����
    /// </summary>
    /// <param name="_player"></param>
    public void SetPlayer(GameObjectBase _player)
    {
        player = _player;
        player.SetMaxHealth(max_HealthPoint);
        cur_HealthPoint = max_HealthPoint;
    }
    /// <summary>
    /// ͬ���������ֵ
    /// </summary>
    /// <param name="_hp"></param>
    public void SetCurrentHealthPoint(float _hp)
    {
        cur_HealthPoint = _hp;
        cur_HealthPoint = Mathf.Clamp(cur_HealthPoint, 0, Mathf.Infinity);
    }

    // ��ʼ������
    public void InitializeData()
    {
        data.Initialize();
        GUIManager.instance.UpdateMoneyText(data.money);
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
        GUIManager.instance.UpdateMoneyText(data.money);
        return true;
    }

    //  �޸ĵ�ǰ��ɫ
    public void SwitchCharacter(CharacterType.CharacterTag _v,CharacterBasicInfomation _info)
    {
        if (_v == cur_Character)
            return;

        cur_CharacterInfo = _info;
        cur_Character = _v;

        // �����ѡ���Ƶ���Ϣ
        GUIManager.instance.ClearUnselectedCardList();
        CardManager.instance.ClearSelectedCard();

        // ��������ͨ�ÿ��Ƶ�ѡ�������� ����ͨ�ÿ�����δ��� �������ɫ����
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
       // cur_HealthPoint += _v;
        player.InstantHealing(_v);
        //cur_HealthPoint = Mathf.Clamp(cur_HealthPoint, 0, max_HealthPoint);
    }

    /// <summary>
    /// ����Ѫ���ٷֱ�
    /// </summary>
    /// <returns></returns>
    public float GetPercentage_HealthPoint()
    {
        return cur_HealthPoint / max_HealthPoint;
    }
    
    /// <summary>
    /// ͨ�������޸�����ֵ
    /// </summary>
    /// <param name="_v"></param>
    public void ChangeRecoverySpeed_PowerPoint(float _v)
    {
        cur_RecoverySpeed_PowerPoint += _v;
        cur_RecoverySpeed_PowerPoint = Mathf.Clamp(cur_RecoverySpeed_PowerPoint, lowBoundary_RecoverySpeed, Mathf.Infinity);
    }

    /// <summary>
    /// ����ս�����ݡ��������¹ؿ�ʱ����
    /// </summary>
    public void ResetBattleData()
    {
        cur_HealthPoint = max_HealthPoint;
        cur_PowerPoint = 0;
        cur_RecoverySpeed_PowerPoint = default_RecoverySpeed_PowerPoint;
    }

    // �����ؿ�
    public void UnlockLevel(int _id)
    {
        data.levelStatus[_id] = true;
    }
}
