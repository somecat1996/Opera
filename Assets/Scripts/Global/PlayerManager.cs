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

    public List<Sprite> bossIcon = new List<Sprite>();
    [Header("Real-Time Data")]
    public float cur_PowerPoint = 0;
    public float cur_HealthPoint = 0;
    public float cur_RecoverySpeed_PowerPoint = 0;

    public bool slowDownRecoverSpeed_PowerPoint = false;
    public float cur_RecoverySpeed_PP_Decrement = 0;

    public int cur_LevelIndex = -1;
    public int cur_BossIndex = -1;
    public int cur_Difficity = 0; // Ĭ���Ѷ�Ϊ0
    // һ����Ϸ����������ؿ�����
    public List<int> bossIndexQueue = new List<int>();
    [Space]
    [Header("Player")]
    public GameObjectBase player;

    [Space]
    public CharacterType.CharacterTag cur_Character = CharacterType.CharacterTag.Dan;
    public int cur_CharBuffID = 0;
    public CharacterBasicInfomation cur_CharacterInfo;
    private Dictionary<int, CharacterBasicInfomation> charInfo = new Dictionary<int, CharacterBasicInfomation>();

    [Header("Data need to be save")]
    public PlayerData data;
    [Space]
    public Dictionary<int, LevelBasicInfomation> levelInfo = new Dictionary<int, LevelBasicInfomation>();
    private void Awake()
    {
        instance = this;

        // ����ؿ���Ϣ
        foreach(var i in Resources.LoadAll<LevelBasicInfomation>("LevelBasicInfomation"))
        {
            levelInfo.Add(i.id, i);
        }
        // �����ɫ��Ϣ
        foreach (var i in Resources.LoadAll<CharacterBasicInfomation>("CharacterInfomation"))
        {
            charInfo.Add(i.id, i);
        }
    }

    void Start()
    {
        GUIManager.instance.UpdateMoneyText(data.money);

        cur_CharacterInfo = charInfo[((int)cur_Character)];
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.CheckIfGameRunning())
        {
            return;
        }

        // �ж��Ƿ���������ֵ����״̬
        if (slowDownRecoverSpeed_PowerPoint)
        {
            float temp = cur_RecoverySpeed_PowerPoint - cur_RecoverySpeed_PP_Decrement;
            temp = Mathf.Clamp(temp, lowBoundary_RecoverySpeed, max_PowerPoint);

            cur_PowerPoint += (temp) * Time.deltaTime;
            cur_PowerPoint = Mathf.Clamp(cur_PowerPoint, 0, max_PowerPoint);
            GUIManager.instance.UpdatePowerPoint(cur_PowerPoint / max_PowerPoint, cur_PowerPoint);
        }
        else
        {
            cur_PowerPoint += cur_RecoverySpeed_PowerPoint * Time.deltaTime;
            cur_PowerPoint = Mathf.Clamp(cur_PowerPoint, 0, max_PowerPoint);
            GUIManager.instance.UpdatePowerPoint(cur_PowerPoint / max_PowerPoint, cur_PowerPoint);
        }
    }

    /// <summary>
    /// ͨ��������������ֵ�ظ������ٶ� ����Ч��ʱ���븺��
    /// </summary>
    /// <param name="_v">����</param>
    public void ChangeDecrement_RecoverySpeed_PowerPoint(float _v)
    {
        slowDownRecoverSpeed_PowerPoint = true;
        cur_RecoverySpeed_PP_Decrement += _v;
    }

    /// <summary>
    /// ��������Ĺؿ�������в�����ؿ������Ѷ�ѡ�����Ľ��밴ťʹ��
    /// </summary>
    public void SpawnLevelIndexList()
    {
        // ����ؿ�����
        ResetLevelIndex();

        // �������� boss����
        List<int> tempList = new List<int>();
        foreach(var i in levelInfo.Values)
        {
            if (LevelSelector.instance.currentIndex <= i.id)
                tempList.Add(i.id);
        }

        bossIndexQueue.Clear();
        while(tempList.Count != 0)
        {
            int index = Random.Range(0, tempList.Count);
            bossIndexQueue.Add(tempList[index]);
            tempList.RemoveAt(index);
        }

        EnterLevel();
    }

    /// <summary>
    /// ����ؿ�
    /// </summary>
    public void EnterLevel(bool _restart = false)
    {
        // �����ؿ��ؿ� ��ǰ�ؿ��±겻��
        if (!_restart)
        {
            cur_BossIndex = bossIndexQueue[0];
            bossIndexQueue.RemoveAt(0);
        }

        // �׶�4 ���ʾ boss�Ѿ����� ��PM����ر�Ļ�� �˴��ж���Ҫ������BDM����֮ǰ
        if (BattleDataManager.instance.cur_Stage == 4)
        {
            GUIManager.instance.SetDisplayCurtain(false);
        }

        // ֪ͨAudioManager���ſ�ͷ��Ч
        AudioManager.instance.PlaySound(levelInfo[cur_BossIndex].sound);
        AudioManager.instance.PlayBGM(cur_BossIndex);

        // ֪ͨGUI�ر��޹�UI����ʾ�ؿ���Ϣ,����BossѪ�� �޸�Bossͷ��
        GUIManager.instance.DisableAllGUI();
        GUIManager.instance.SpawnLevelName(levelInfo[cur_BossIndex].levelName);
        //GUIManager.instance.UpdateBossHealthPoint(1);
        GUIManager.instance.SetBossIcon(bossIcon[cur_BossIndex]);

        /*
        // Buff��� Buff�ڽ���ؿ�ʱ�������
        BuffManager.instance.DiableAllBuff(); // ���BUFF
        EnableCharBuff(); // ���ý�ɫ����
        BuffManager.instance.EnableAllSelectedBuff(); // ��������ѡ���BUFF
        */

        // �������
        CardManager.instance.ClearAllActivatedCard(); // ����������еĿ���ʵ��
        CardManager.instance.RealignAndLoadCards(); // ���ӳ��Ͽ���

        // ��ҽ�ɫ���
        ResetBattleData();

        // ֪ͨ��Ϸ������
        GameManager.instance.SetStartGame(true);
        GameManager.instance.SetPauseGame(false);

        // ���ս��������ʵ�岢����BDM
        EnemyManager.instance.Clear();
        BattleDataManager.instance.ResetAllData();

        // Buff��� �����Ѿ�ѡ���buff����
        BuffManager.instance.ResetActivatedBuffData();

        // �����ؿ�
        EnemyManager.instance.EnterLevel(cur_BossIndex,cur_Difficity);
    }

    /// <summary>
    /// �����ؿ�
    /// </summary>
    public void RestartLevel()
    {
        Invoke("restartLevel", Time.deltaTime);
    }
    void restartLevel()
    {
        //EnterLevel(true);

        BuffManager.instance.DiableAllBuff();
        EnableCharBuff();
        // ����ؿ�����
        ResetLevelIndex();

        // �������� boss����
        List<int> tempList = new List<int>();
        foreach (var i in levelInfo.Values)
        {
            tempList.Add(i.id);
        }

        bossIndexQueue.Clear();
        while (tempList.Count != 0)
        {
            int index = Random.Range(0, tempList.Count);
            bossIndexQueue.Add(tempList[index]);
            tempList.RemoveAt(index);
        }

        EnterLevel();
    }
    /// <summary>
    /// ������һ�ؿ�
    /// </summary>
    public void EnterNextLevel()
    {
        // �ؿ��±���1
        cur_LevelIndex++;
        Invoke("enterNextLevel", Time.deltaTime);
    }
    void enterNextLevel()
    {
        EnterLevel(false);
    }

    public void ResetLevelIndex()
    {
        cur_LevelIndex = 0;
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

    // ��ʼ������ ������Һ͹ؿ���Ϣ����
    public void InitializeData()
    {
        // ��ʼ���������
        data.Initialize();
        GUIManager.instance.UpdateMoneyText(data.money);

        // ��ʼ���ؿ�����
        foreach (var i in levelInfo.Values)
            i.InitializeData();
    }

    /// <summary>
    /// ��õ�ǰ�ؿ���Ϣ
    /// </summary>
    /// <returns></returns>
    public LevelBasicInfomation GetCurrentLevelInfo()
    {
        return levelInfo[cur_LevelIndex];
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

    /// <summary>
    /// �޸ĵ�ǰѡ���ɫ��Ϣ
    /// </summary>
    /// <param name="_v"></param>
    /// <param name="_info"></param>
    /// <param name="_buffIndex">�����±� 0-1 Ĭ��ѡ��0</param>
    public void SwitchCharacter(CharacterType.CharacterTag _v,CharacterBasicInfomation _info, int _buffIndex = 0)
    {
        cur_CharBuffID = _info.buffID[_buffIndex];

        if (_v == cur_Character)
            return;

        // �л���ɫ��Ϣ �� �л�������ɫʵ��
        cur_CharacterInfo = _info;
        cur_Character = _v;
        Player.instance.ChangeCharacter(cur_CharacterInfo.id);

        // �����ѡ���Ƶ���Ϣ
        GUIManager.instance.ClearUnselectedCardList();
        CardManager.instance.ClearSelectedCard();

        // ��������ͨ�ÿ��Ƶ�ѡ��������
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
            GUIManager.instance.SpawnSystemText("����ֵ����!");
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
        cur_RecoverySpeed_PP_Decrement = 0;
        slowDownRecoverSpeed_PowerPoint = false;


        player.SetMaxHealth(max_HealthPoint);
    }

    /// <summary>
    /// ���ý�ɫBUFF
    /// </summary>
    public void EnableCharBuff()
    {
        BuffManager.instance.EnableBuff(cur_CharBuffID);
    }

    // �����ؿ�
    public void UnlockLevel(int _id)
    {
        _id = Mathf.Clamp(_id, 0, levelInfo.Count - 1);
        levelInfo[_id].unlocked = true;
    }
    // ��¼�ؿ�ͨ�ش���
    public void UpdateVictoryTime()
    {
        levelInfo[cur_LevelIndex].victoryTime++;
    }

    public void EnterLevel_Test(int _index)
    {
        cur_LevelIndex = _index;

        // ֪ͨGUI�ر��޹�UI����ʾ�ؿ���Ϣ,����BossѪ��
        GUIManager.instance.DisableAllGUI();
        GUIManager.instance.SpawnLevelName(levelInfo[cur_LevelIndex].levelName);
        //GUIManager.instance.UpdateBossHealthPoint(1);

        // Buff���
        BuffManager.instance.DiableAllBuff(); // ���BUFF
        EnableCharBuff(); // ���ý�ɫ����
        BuffManager.instance.EnableAllSelectedBuff(); // ��������ѡ���BUFF

        // �������
        CardManager.instance.ClearAllActivatedCard(); // ����������еĿ���ʵ��
        CardManager.instance.RealignAndLoadCards(); // ���ӳ��Ͽ���

        // ��ҽ�ɫ���
        ResetBattleData();

        // ֪ͨ��Ϸ������
        GameManager.instance.SetStartGame(true);
        GameManager.instance.SetPauseGame(false);

        // ���ս��������ʵ�岢����BDM
        EnemyManager.instance.Clear();
        BattleDataManager.instance.ResetAllData();

        // �����ؿ�
        EnemyManager.instance.EnterLevel(cur_LevelIndex);
    }

    /// <summary>
    /// ��⵱ǰ�ؿ��Ƿ�Ϊ���չؿ�
    /// </summary>
    public bool CheckIfFinalLevel()
    {
        return bossIndexQueue.Count == 0;
    }

    /// <summary>
    /// ��õ�ǰ�ؿ��±�
    /// </summary>
    /// <returns></returns>
    public int GetLevelIndex()
    {
        return cur_LevelIndex;
    }
}
