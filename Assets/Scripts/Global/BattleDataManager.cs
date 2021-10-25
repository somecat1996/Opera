using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class BattleDataManager : MonoBehaviour
{
    public static BattleDataManager instance;
    [Header("Real-Time Data")]
    public float gameTimer = 0; // 游戏市场
    public int loot = 0; // 战利品金币
    public bool playerVictory = false;
    [Space]
    public float totalDamage = 0; // 由敌人Hurt函数上传伤害信息
    public int totalUsedCard = 0; // 由CardManger.SendToTempLayoutGroup上传
    public GameObjectBase lastTargetEnemy; // 上一个被单体攻击的敌人 由单体输出卡牌上传
    public float cur_bossHP_Percentage = 0; // 当前boss剩余血量百分比
    public int cur_Stage = 0; // 当前阶段
    public float timer_LastStage = 0; // 上一阶段进行的时间
    [Space]
    public CardPrototype selectingCard; // 玩家当前选中的卡牌 或 即将要使用的卡牌
    public CardPrototype lastUsedCard; // 由CardManger.SendToTempLayoutGroup上传
    [Space]
    public List<GameObjectBase> enemyList = new List<GameObjectBase>(); // 由敌人自身上传信息
    [Space]
    public bool playerMoving = false; // 玩家是否移动
    [Space]
    public float appealPoint = 0;// 喝彩值

    [Header("Obejcts And Related Configuration")]
    // 通用配置
    [Space]
    public float fadeTime = 0.5f;
    // 范围卡牌指示器
    public GameObject rangeDisplayer;
    public bool activateRangeDIsplayer = false;
    [Space]
    // 单体卡牌指示器
    public Vector3 markerOffset;
    public bool activateTargetMarker;
    public GameObject targetMarker;
    [Space]
    // 方向性卡牌指示器
    public LineRenderer lineRender_Dp;
    public GameObject directionPointer;
    [Header("Spectator Objects And RealTime Data")]
    public int activatedSpectator = 0;
    public int highlightSpectator = 0;

    [Space]
    // 观众实体 分别为 未激活-点亮-高光
    public List<Image> deactivatedSpectatorList = new List<Image>();
    public List<Image> activatedSpectatorList = new List<Image>();
    public List<Image> highlightSpectatorList = new List<Image>();

    public Image spectator_Spectial;
    [Space]
    public TextMeshProUGUI text_AppeapPoint;
    public TextMeshProUGUI text_AppealPoint_Label;


    [Header("Sound Configurtaion")]
    public AudioClip sound_Victory;
    public AudioClip sound_Defeated;
    public AudioClip[] sound_Applause = new AudioClip[3];

    private void Awake()
    {
        instance = this;

        if (!rangeDisplayer)
            rangeDisplayer = GameObject.FindWithTag("RangeDisplayer");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.CheckIfGameRunning())
        {
            return;
        }

        gameTimer += Time.deltaTime;
        timer_LastStage = gameTimer;

        if (activateRangeDIsplayer)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 1000, CardManager.instance.groundLayer))
            {
                rangeDisplayer.transform.position = hit.point;
            }
        }

        if (activateTargetMarker)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.tag == "Enemy")
                {
                    //targetMarker.SetActive(true);
                    targetMarker.transform.DOScale(Vector3.one, fadeTime);
                    targetMarker.transform.position = (hit.transform.position) + markerOffset;
                }
                else
                {
                    targetMarker.transform.DOScale(Vector3.zero, fadeTime);
                    //targetMarker.SetActive(false);
                }
            }
            else
            {
                targetMarker.transform.DOScale(Vector3.zero, fadeTime);
                //targetMarker.SetActive(false);
            }
        }

        if (directionPointer)
        {
            lineRender_Dp.SetPosition(0, PlayerManager.instance.player.transform.position);
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 1000, CardManager.instance.groundLayer))
            {
                lineRender_Dp.SetPosition(1, hit.point);
            }
            else
            {

            }
        }
    }

    private void OnEnable()
    {
        ResetAllData();
    }

    /// <summary>
    ///  重设数据——开启新关卡时调用
    /// </summary>
    public void ResetAllData()
    {
        // 系统信息
        gameTimer = 0;

        // 伤害数值
        totalDamage = 0;
        totalUsedCard = 0;
        lastTargetEnemy = null;
        lastUsedCard = null;

        // 场上敌人列表
        enemyList.Clear();

        // 玩家信息
        playerMoving = false;

        // 指示器
        //directionPointer.SetActive(false);
        //rangeDisplayer.SetActive(false);
        //targetMarker.SetActive(false);
        activateTargetMarker = false;

        // 阶段信息
        cur_Stage = 0;
        timer_LastStage = 0;

        // 喝彩值相关
        appealPoint = 0;

        // 观众相关
        // 将所有已经激活的观众设置成剪影
        while (activatedSpectatorList.Count != 0)
        {
            deactivatedSpectatorList.Add(activatedSpectatorList[0]);
            activatedSpectatorList.RemoveAt(0);
        }
        while(highlightSpectatorList.Count != 0)
        {
            deactivatedSpectatorList.Add(highlightSpectatorList[0]);
            highlightSpectatorList.RemoveAt(0);
        }
        foreach(var i in deactivatedSpectatorList)
        {
            i.GetComponent<Spectator>().Deactivate();
        }
        spectator_Spectial.GetComponent<Spectator>().Deactivate();

        activatedSpectator = 0;
        highlightSpectator = 0;

        // BOSS信息
        cur_bossHP_Percentage = 1;
    }


    /// <summary>
    /// 开启范围显示器
    /// </summary>
    /// <param name="_v"></param>
    /// <param name="_radius">半径</param>
    public void SetActiveRangeDisplayer(bool _v,float _radius = 0)
    {
        activateRangeDIsplayer = _v;

        if (_v)
        {
            //rangeDisplayer.transform.localScale = new Vector3(_radius, _radius, _radius) * 2;
            //rangeDisplayer.SetActive(_v);
            rangeDisplayer.transform.DOScale(new Vector3(_radius, _radius, _radius) * 2, fadeTime);
        }
        else
        {
            //rangeDisplayer.SetActive(_v);
            rangeDisplayer.transform.DOScale(Vector3.zero, fadeTime);
        }
    }

    /// <summary>
    /// 开启单体目标指示器
    /// </summary>
    /// <param name="_v"></param>
    public void SetActiveTargetMarker(bool _v)
    {
        activateTargetMarker = _v;

        if(_v == false)
        {
            targetMarker.transform.DOScale(Vector3.zero, fadeTime);
            //targetMarker.SetActive(false);
        }
    }

    /// <summary>
    /// 开启方向指示器
    /// </summary>
    /// <param name="_v"></param>
    public void SetActiveDirectionPointer(bool _v)
    {
        directionPointer.SetActive(_v);
    }

    // 更新伤害数据
    public void UpdateDamage(float _damage)
    {
        totalDamage += _damage;
    }

    public void UpdateUsedCard(CardPrototype _cp)
    {
        totalUsedCard++;
        lastUsedCard = _cp;
    }
    public void UpdateSelectingCard(CardPrototype _cp)
    {
        selectingCard = _cp;
    }

    public void UpdateTargetEnemy(GameObjectBase _gob)
    {
        lastTargetEnemy = _gob;
    }

    // 更新玩家移动状态 true->移动中 false->未移动
    public void UpdatePlayerMovingStatus(bool _v)
    {
        playerMoving = _v;
    }


    // 增加(移除)战场上敌人信息 在生成和*敌人对象销毁前*调用
    public void AddEnemyData(GameObjectBase _gob)
    {
        enemyList.Add(_gob);
    }
    public void RemoveEnemyData(GameObjectBase _gob)
    {
        enemyList.Remove(_gob);
    }

    /// <summary>
    /// 游戏结束 结算游戏结果——游戏结束时调用
    /// </summary>
    /// <param name="_playerVictory">玩家是否胜利</param>
    ///     /// <param name="_animate">是否需要幕布动画</param>
    public void EvaluateGameResult(bool _playerVictory,bool _animate = true)
    {
        playerVictory = _playerVictory;

        if (PlayerManager.instance.CheckIfFinalLevel())
            GUIManager.instance.SetInteractable_btn_NextLevel(false);
        else
            GUIManager.instance.SetInteractable_btn_NextLevel(true);


        GameManager.instance.SetStartGame(false);
        GameManager.instance.SetPauseGame(false);

        if (_animate)
        {
            GUIManager.instance.DisplayCurtain(evaluateGameResult);
        }
        else
        {
            evaluateGameResult();
        }
    }
    /// <summary>
    /// 强制失败
    /// </summary>
    public void ForceDefeated()
    {
        EvaluateGameResult(false, true);
    }
    void evaluateGameResult()
    {
        // 计算金币 仅使用难度1系数 未知关卡难度选择操作
        if (playerVictory)
        {
            int timeReward = 0;
            if (gameTimer <= 120)
            {
                timeReward = 100;
            }
            else if (gameTimer > 120 && gameTimer < 180)
            {
                timeReward = 50;
            }
            else
            {
                timeReward = 0;
            }
            loot = GlobalValue.GetTrueLoot(((Random.Range(100, 200) + timeReward) * PlayerManager.instance.GetCurrentLevelInfo().rewardFactor[0]));
        }
        else
        {
            // 失败时 金币结算
            loot = GlobalValue.GetTrueLoot((Random.Range(100, 200) * PlayerManager.instance.GetCurrentLevelInfo().rewardFactor[0] * (1 - cur_bossHP_Percentage)));
        }


        // 随机抽取3张卡
        List<CardBasicInfomation> lootCard = CardManager.instance.GetCardsRandomly(3);
        if (playerVictory)
        {
            // 剧情卡临时列表
            List<CardBasicInfomation> lootCard_Common = new List<CardBasicInfomation>();
            foreach (var i in PlayerManager.instance.GetCurrentLevelInfo().lootCard)
            {
                // 若对应剧情卡牌未解锁则暂存
                if (CardManager.instance.cardLibrary_Common[i].level == 0)
                    lootCard_Common.Add(CardManager.instance.cardLibrary_Common[i]);
            }
            if (lootCard_Common.Count != 0)
            {
                // 先随机移除一个 之后在新增一个
                lootCard.RemoveAt(Random.Range(0, lootCard.Count));
                lootCard.Add(lootCard_Common[Random.Range(0, lootCard_Common.Count)]);
            }
        }
        else
        {
            lootCard.Clear();
        }


        // 实际数值传输到CardManager和PlayerManager;
        PlayerManager.instance.ChangeMoney(loot);
        foreach (var i in lootCard)
        {
            if (i.belongner != CharacterType.CharacterTag.Common)
            {
                CardManager.instance.cardLibrary[i.id].quantity++;
            }
            else
            {
                CardManager.instance.cardLibrary_Common[i.id].level = 1;

                // 获得剧情卡牌 重新刷新玩家所选的卡牌 *****注意***** 此处临时使用 会冲掉玩家所选的卡牌 日后需要优化这个功能
                // 清除已选择卡牌的信息
                GUIManager.instance.ClearUnselectedCardList();
                CardManager.instance.ClearSelectedCard();

                // 重新载入通用卡牌到选择容器中
                CardManager.instance.LoadAllCardIntoUnselectedList();
            }

        }

        // 下面代码已无用
        if (playerVictory)
        {
            PlayerManager.instance.UpdateVictoryTime();
            // 解锁下一关
            PlayerManager.instance.UnlockLevel(PlayerManager.instance.cur_LevelIndex + 1);

            AudioManager.instance.PlaySound(sound_Victory);
        }
        else
        {
            AudioManager.instance.PlaySound(sound_Defeated);
        }

        // GUI 显示
        GUIManager.instance.EnableGameResult(playerVictory, gameTimer, loot, lootCard);
    }


    /// <summary>
    /// 更新BOSS血量
    /// </summary>
    /// <param name="_percentage">百分比</param>
    /// <param name="_v">血量</param>
    public void UpdateBossHP(float _percentage,float _v)
    {
        cur_bossHP_Percentage = _percentage;
        cur_bossHP_Percentage = Mathf.Clamp(cur_bossHP_Percentage, 0, Mathf.Infinity);
        GUIManager.instance.UpdateBossHealthPoint(cur_bossHP_Percentage,_v);
    }

    /// <summary>
    /// 更新当前阶段数
    /// </summary>
    /// <param name="_v"></param>
    public void UpdateStage(int _v)
    {
        // 开头不进行结算
        if (cur_Stage == 0)
        {
            cur_Stage = _v; ;
            return;
        }

        AudioManager.instance.SetTurnDownBGM(true);
        GameManager.instance.SetPauseGame(true);
        GUIManager.instance.SetDisplayCurtain(true, updateStage);
        
    }

    /// <summary>
    /// 该方法由幕布回调函数执行
    /// </summary>
    void updateStage()
    {
        // 计算上一阶段喝彩值和赏钱
        int tempAP = 0;
        if (timer_LastStage <= 30)
        {
            tempAP = 120 - (int)timer_LastStage;
        }
        else if (timer_LastStage > 30 && timer_LastStage <= 60)
        {
            tempAP = 150 - 2 * (int)timer_LastStage;
        }
        else
        {
            tempAP = 30;
        }

        // 获得真实喝彩值
        tempAP = GlobalValue.GetTrueReward(tempAP);

        // 根据单阶段喝彩值回复血量
        if (tempAP >= 30 && tempAP <= 49)
        {
            PlayerManager.instance.player.InstantHealing(60 + GlobalValue.hpIncrement_Reward);
            GUIManager.instance.SpawnSystemText("回复 " + (60 + GlobalValue.hpIncrement_Reward) + " 氛围值");
        }
        else if (tempAP >= 50 && tempAP <= 79)
        {
            PlayerManager.instance.player.InstantHealing(80 + GlobalValue.hpIncrement_Reward);
            GUIManager.instance.SpawnSystemText("回复 " + (80 + GlobalValue.hpIncrement_Reward) + " 氛围值");
        }
        else if (tempAP >= 80)
        {
            PlayerManager.instance.player.InstantHealing(100 + GlobalValue.hpIncrement_Reward);
            GUIManager.instance.SpawnSystemText("回复 " + (100 + GlobalValue.hpIncrement_Reward) + " 氛围值");
        }

        if (tempAP <= 60)
        {
            PlayerManager.instance.player.InstantHealing(100 + GlobalValue.hpIncrement_Reward);
            GUIManager.instance.SpawnSystemText("回复 " + (100 + GlobalValue.hpIncrement_Reward) + " 氛围值");
        }

        appealPoint += tempAP;

        // 根据喝彩值显示观众人数
        if (CheckInRange(appealPoint, 30, 39))
        {
            int rSpect_Act = 1 - activatedSpectator + highlightSpectator; // 剩余需要激活的观众
            int rSpect_Hl = 0 - highlightSpectator; // 剩余需要高光的观众

            rSpect_Act = Mathf.Clamp(rSpect_Act, 0, 8);
            rSpect_Hl = Mathf.Clamp(rSpect_Hl, 0, 8);

            StartCoroutine(HandleSpcetator(rSpect_Act, rSpect_Hl));

            AudioManager.instance.PlaySound(sound_Applause[0]);
            
        }
        else if (CheckInRange(appealPoint, 40, 79))
        {
            int rSpect_Act = 3 - activatedSpectator + highlightSpectator; // 剩余需要激活的观众
            int rSpect_Hl = 1 - highlightSpectator; // 剩余需要高光的观众

            rSpect_Act = Mathf.Clamp(rSpect_Act, 0, 8);
            rSpect_Hl = Mathf.Clamp(rSpect_Hl, 0, 8);

            StartCoroutine(HandleSpcetator(rSpect_Act, rSpect_Hl));

            AudioManager.instance.PlaySound(sound_Applause[0]);
        }
        else if (CheckInRange(appealPoint,80, 119))
        {
            int rSpect_Act = 5 - activatedSpectator + highlightSpectator; // 剩余需要激活的观众
            int rSpect_Hl = 1 - highlightSpectator; // 剩余需要高光的观众

            rSpect_Act = Mathf.Clamp(rSpect_Act, 0, 8);
            rSpect_Hl = Mathf.Clamp(rSpect_Hl, 0, 8);

            StartCoroutine(HandleSpcetator(rSpect_Act, rSpect_Hl));

            AudioManager.instance.PlaySound(sound_Applause[1]);
        }
        else if (CheckInRange(appealPoint, 120, 149))
        {
            int rSpect_Act = 5 - activatedSpectator + highlightSpectator; // 剩余需要激活的观众
            int rSpect_Hl = 2 - highlightSpectator; // 剩余需要高光的观众

            rSpect_Act = Mathf.Clamp(rSpect_Act, 0, 8);
            rSpect_Hl = Mathf.Clamp(rSpect_Hl, 0, 8);

            StartCoroutine(HandleSpcetator(rSpect_Act, rSpect_Hl));

            AudioManager.instance.PlaySound(sound_Applause[1]);
        }
        else if (CheckInRange(appealPoint, 150, 179))
        {
            int rSpect_Act = 6 - activatedSpectator + highlightSpectator; // 剩余需要激活的观众
            int rSpect_Hl = 2 - highlightSpectator; // 剩余需要高光的观众

            rSpect_Act = Mathf.Clamp(rSpect_Act, 0, 8);
            rSpect_Hl = Mathf.Clamp(rSpect_Hl, 0, 8);

            StartCoroutine(HandleSpcetator(rSpect_Act, rSpect_Hl));

            AudioManager.instance.PlaySound(sound_Applause[2]);
        }
        else if (CheckInRange(appealPoint, 180, 219))
        {
            int rSpect_Act = 6 - activatedSpectator + highlightSpectator; // 剩余需要激活的观众
            int rSpect_Hl = 3 - highlightSpectator; // 剩余需要高光的观众

            rSpect_Act = Mathf.Clamp(rSpect_Act, 0, 8);
            rSpect_Hl = Mathf.Clamp(rSpect_Hl, 0, 8);

            StartCoroutine(HandleSpcetator(rSpect_Act, rSpect_Hl));

            AudioManager.instance.PlaySound(sound_Applause[2]);
        }
        else if (appealPoint >= 220)
        {
            int rSpect_Act = 6 - activatedSpectator + highlightSpectator; // 剩余需要激活的观众
            int rSpect_Hl = 6 - highlightSpectator; // 剩余需要高光的观众

            rSpect_Act = Mathf.Clamp(rSpect_Act, 0, 8);
            rSpect_Hl = Mathf.Clamp(rSpect_Hl, 0, 8);

            StartCoroutine(HandleSpcetator(rSpect_Act, rSpect_Hl));

            AudioManager.instance.PlaySound(sound_Applause[2]);
        }

        // 更新阶段数
        cur_Stage++;

        // 重置阶段计时器
        timer_LastStage = 0;

        // 显示喝彩值文本
        DisplayAppealPoint();
    }

    IEnumerator HandleSpcetator(int _remainActivated,int _remainHighlight)
    {
        // 首先激活观众
        while(_remainActivated-- > 0)
        {
            if (deactivatedSpectatorList.Count == 0)
                break;

            int index = Random.Range(0, deactivatedSpectatorList.Count);

            activatedSpectatorList.Add(deactivatedSpectatorList[index]);
            deactivatedSpectatorList[index].GetComponent<Spectator>().Activate();
            deactivatedSpectatorList.RemoveAt(index);
            activatedSpectator++;

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1f);

        // 之后高光已经激活的观众
        while (_remainHighlight-- > 0)
        {
            int index = Random.Range(0, activatedSpectatorList.Count);

            highlightSpectatorList.Add(activatedSpectatorList[index]);
            activatedSpectatorList[index].GetComponent<Spectator>().Highlight();
            activatedSpectatorList.RemoveAt(index);
            highlightSpectator++;
            activatedSpectator--;

            yield return new WaitForSeconds(0.5f);
        }

        // 符合富婆激活条件
        if(appealPoint >= 150 && spectator_Spectial.color == Color.black)
        {
            spectator_Spectial.GetComponent<Spectator>().Activate();
        }
        if(appealPoint >= 220)
        {
            spectator_Spectial.GetComponent<Spectator>().Highlight();
        }

        // 根据当前已选BUFF数决定是否再让玩家选择Buff
        if(BuffManager.instance.activiatedBuffList.Count >= 10)
        {
            // Buff已满
            Curtain.instance.SetActivatable(true);
        }
        else
        {
            // 显示buff选择栏
            BuffSelector.instance.EnablePanel();
        }
    }

    // 显示喝彩值文本
    public void DisplayAppealPoint()
    {
        text_AppeapPoint.text = appealPoint.ToString();
        text_AppeapPoint.DOFade(1, 0.35f);
        text_AppealPoint_Label.DOFade(1, 0.35f);
    }
    public void DisappealAppealPoint()
    {
        text_AppeapPoint.DOFade(0, 0.35f);
        text_AppealPoint_Label.DOFade(0, 0.35f);
    }

    // 激活特殊观众
    void ActivatedFinalSpectator()
    {
        spectator_Spectial.GetComponent<Spectator>().Activate();
    }
    // 高光特殊观众
    void HighlightFinalSpectator()
    {
        spectator_Spectial.GetComponent<Spectator>().Highlight();
    }

    /// <summary>
    /// 检测最后的特殊观众是否被激活
    /// </summary>
    /// <returns></returns>
    public bool CheckActivated_FInalSpectator()
    {
        return !(spectator_Spectial.color == Color.black);
    }

    bool CheckInRange(int _v,int _min,int _max)
    {
        return _v >= _min && _v <= _max;
    }
    bool CheckInRange(float _v, int _min, int _max)
    {
        return _v >= _min && _v <= _max;
    }
}
