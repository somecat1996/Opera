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
    public int cur_Difficity = 0; // 默认难度为0
    // 一句游戏下来的随机关卡序列
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

        // 载入关卡信息
        foreach(var i in Resources.LoadAll<LevelBasicInfomation>("LevelBasicInfomation"))
        {
            levelInfo.Add(i.id, i);
        }
        // 载入角色信息
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

        // 判断是否开启了心流值减速状态
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
    /// 通过增量设置心流值回复减少速度 返还效果时传入负数
    /// </summary>
    /// <param name="_v">增量</param>
    public void ChangeDecrement_RecoverySpeed_PowerPoint(float _v)
    {
        slowDownRecoverSpeed_PowerPoint = true;
        cur_RecoverySpeed_PP_Decrement += _v;
    }

    /// <summary>
    /// 生成随机的关卡编号序列并进入关卡——难度选择界面的进入按钮使用
    /// </summary>
    public void SpawnLevelIndexList()
    {
        // 重设关卡序列
        ResetLevelIndex();

        // 重新生成 boss序列
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
    /// 进入关卡
    /// </summary>
    public void EnterLevel(bool _restart = false)
    {
        // 若非重开关卡 则当前关卡下标不变
        if (!_restart)
        {
            cur_BossIndex = bossIndexQueue[0];
            bossIndexQueue.RemoveAt(0);
        }

        // 阶段4 则表示 boss已经死亡 由PM代替关闭幕布 此处判断需要在重置BDM数据之前
        if (BattleDataManager.instance.cur_Stage == 4)
        {
            GUIManager.instance.SetDisplayCurtain(false);
        }

        // 通知AudioManager播放开头音效
        AudioManager.instance.PlaySound(levelInfo[cur_BossIndex].sound);
        AudioManager.instance.PlayBGM(cur_BossIndex);

        // 通知GUI关闭无关UI且显示关卡信息,重置Boss血条 修改Boss头像
        GUIManager.instance.DisableAllGUI();
        GUIManager.instance.SpawnLevelName(levelInfo[cur_BossIndex].levelName);
        //GUIManager.instance.UpdateBossHealthPoint(1);
        GUIManager.instance.SetBossIcon(bossIcon[cur_BossIndex]);

        /*
        // Buff相关 Buff在进入关卡时不在清除
        BuffManager.instance.DiableAllBuff(); // 清空BUFF
        EnableCharBuff(); // 启用角色被动
        BuffManager.instance.EnableAllSelectedBuff(); // 启用所有选择的BUFF
        */

        // 卡牌相关
        CardManager.instance.ClearAllActivatedCard(); // 清除场上所有的卡牌实体
        CardManager.instance.RealignAndLoadCards(); // 增加场上卡牌

        // 玩家角色相关
        ResetBattleData();

        // 通知游戏管理器
        GameManager.instance.SetStartGame(true);
        GameManager.instance.SetPauseGame(false);

        // 清空战场上所有实体并重置BDM
        EnemyManager.instance.Clear();
        BattleDataManager.instance.ResetAllData();

        // Buff相关 重置已经选择的buff数据
        BuffManager.instance.ResetActivatedBuffData();

        // 开启关卡
        EnemyManager.instance.EnterLevel(cur_BossIndex,cur_Difficity);
    }

    /// <summary>
    /// 重启关卡
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
        // 重设关卡序列
        ResetLevelIndex();

        // 重新生成 boss序列
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
    /// 进入下一关卡
    /// </summary>
    public void EnterNextLevel()
    {
        // 关卡下标增1
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
    /// 设置玩家对象并传送最大生命值参数
    /// </summary>
    /// <param name="_player"></param>
    public void SetPlayer(GameObjectBase _player)
    {
        player = _player;
        player.SetMaxHealth(max_HealthPoint);
        cur_HealthPoint = max_HealthPoint;
    }
    /// <summary>
    /// 同步玩家生命值
    /// </summary>
    /// <param name="_hp"></param>
    public void SetCurrentHealthPoint(float _hp)
    {
        cur_HealthPoint = _hp;
        cur_HealthPoint = Mathf.Clamp(cur_HealthPoint, 0, Mathf.Infinity);
    }

    // 初始化数据 包括玩家和关卡信息数据
    public void InitializeData()
    {
        // 初始化玩家数据
        data.Initialize();
        GUIManager.instance.UpdateMoneyText(data.money);

        // 初始化关卡数据
        foreach (var i in levelInfo.Values)
            i.InitializeData();
    }

    /// <summary>
    /// 获得当前关卡信息
    /// </summary>
    /// <returns></returns>
    public LevelBasicInfomation GetCurrentLevelInfo()
    {
        return levelInfo[cur_LevelIndex];
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

    /// <summary>
    /// 修改当前选择角色信息
    /// </summary>
    /// <param name="_v"></param>
    /// <param name="_info"></param>
    /// <param name="_buffIndex">被动下标 0-1 默认选择0</param>
    public void SwitchCharacter(CharacterType.CharacterTag _v,CharacterBasicInfomation _info, int _buffIndex = 0)
    {
        cur_CharBuffID = _info.buffID[_buffIndex];

        if (_v == cur_Character)
            return;

        // 切换角色信息 且 切换场景角色实体
        cur_CharacterInfo = _info;
        cur_Character = _v;
        Player.instance.ChangeCharacter(cur_CharacterInfo.id);

        // 清除已选择卡牌的信息
        GUIManager.instance.ClearUnselectedCardList();
        CardManager.instance.ClearSelectedCard();

        // 重新载入通用卡牌到选择容器中
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
            GUIManager.instance.SpawnSystemText("心流值不足!");
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
       // cur_HealthPoint += _v;
        player.InstantHealing(_v);
        //cur_HealthPoint = Mathf.Clamp(cur_HealthPoint, 0, max_HealthPoint);
    }

    /// <summary>
    /// 返回血量百分比
    /// </summary>
    /// <returns></returns>
    public float GetPercentage_HealthPoint()
    {
        return cur_HealthPoint / max_HealthPoint;
    }
    
    /// <summary>
    /// 通过增量修改心流值
    /// </summary>
    /// <param name="_v"></param>
    public void ChangeRecoverySpeed_PowerPoint(float _v)
    {
        cur_RecoverySpeed_PowerPoint += _v;
        cur_RecoverySpeed_PowerPoint = Mathf.Clamp(cur_RecoverySpeed_PowerPoint, lowBoundary_RecoverySpeed, Mathf.Infinity);
    }

    /// <summary>
    /// 重设战斗数据——进入新关卡时调用
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
    /// 启用角色BUFF
    /// </summary>
    public void EnableCharBuff()
    {
        BuffManager.instance.EnableBuff(cur_CharBuffID);
    }

    // 解锁关卡
    public void UnlockLevel(int _id)
    {
        _id = Mathf.Clamp(_id, 0, levelInfo.Count - 1);
        levelInfo[_id].unlocked = true;
    }
    // 记录关卡通关次数
    public void UpdateVictoryTime()
    {
        levelInfo[cur_LevelIndex].victoryTime++;
    }

    public void EnterLevel_Test(int _index)
    {
        cur_LevelIndex = _index;

        // 通知GUI关闭无关UI且显示关卡信息,重置Boss血条
        GUIManager.instance.DisableAllGUI();
        GUIManager.instance.SpawnLevelName(levelInfo[cur_LevelIndex].levelName);
        //GUIManager.instance.UpdateBossHealthPoint(1);

        // Buff相关
        BuffManager.instance.DiableAllBuff(); // 清空BUFF
        EnableCharBuff(); // 启用角色被动
        BuffManager.instance.EnableAllSelectedBuff(); // 启用所有选择的BUFF

        // 卡牌相关
        CardManager.instance.ClearAllActivatedCard(); // 清除场上所有的卡牌实体
        CardManager.instance.RealignAndLoadCards(); // 增加场上卡牌

        // 玩家角色相关
        ResetBattleData();

        // 通知游戏管理器
        GameManager.instance.SetStartGame(true);
        GameManager.instance.SetPauseGame(false);

        // 清空战场上所有实体并重置BDM
        EnemyManager.instance.Clear();
        BattleDataManager.instance.ResetAllData();

        // 开启关卡
        EnemyManager.instance.EnterLevel(cur_LevelIndex);
    }

    /// <summary>
    /// 检测当前关卡是否为最终关卡
    /// </summary>
    public bool CheckIfFinalLevel()
    {
        return bossIndexQueue.Count == 0;
    }

    /// <summary>
    /// 获得当前关卡下标
    /// </summary>
    /// <returns></returns>
    public int GetLevelIndex()
    {
        return cur_LevelIndex;
    }
}
