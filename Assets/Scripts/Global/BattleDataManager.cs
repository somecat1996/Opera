using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDataManager : MonoBehaviour
{
    public static BattleDataManager instance;
    [Header("Real-Time Data")]
    public float gameTimer = 0;
    public int loot = 0;

    public float totalDamage = 0; // 由敌人Hurt函数上传伤害信息
    public int totalUsedCard = 0; // 由CardManger.SendToTempLayoutGroup上传
    public GameObjectBase lastTargetEnemy; // 上一个被单体攻击的敌人 由单体输出卡牌上传
    public float cur_bossHP_Pencentage = 0;
    [Space]
    public CardPrototype selectingCard; // 玩家当前选中的卡牌 或 即将要使用的卡牌
    public CardPrototype lastUsedCard; // 由CardManger.SendToTempLayoutGroup上传
    [Space]
    public List<GameObjectBase> enemyList = new List<GameObjectBase>(); // 由敌人自身上传信息
    [Space]
    public bool playerMoving = false;

    [Header("Obejcts And Related Configuration")]
    // 范围卡牌指示器
    public GameObject rangeDisplayer;
    [Space]
    // 单体卡牌指示器
    public Vector3 markerOffset;
    public bool activateTargetMarker;
    public GameObject targetMarker;
    [Space]
    // 方向性卡牌指示器
    public LineRenderer lineRender_Dp;
    public GameObject directionPointer;

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
        if (Input.GetKeyDown(KeyCode.O))
        {
            EvaluateGameResult(true);
        }

        if (!GameManager.instance.CheckIfGameRunning())
        {
            return;
        }

        gameTimer += Time.deltaTime;

        if (rangeDisplayer.activeSelf)
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
                    targetMarker.SetActive(true);
                    targetMarker.transform.position = (hit.transform.position) + markerOffset;
                }
                else
                {
                    targetMarker.SetActive(false);
                }
            }
            else
            {
                targetMarker.SetActive(false);
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
    /// 开启范围显示器
    /// </summary>
    /// <param name="_v"></param>
    /// <param name="_radius">半径</param>
    public void SetActiveRangeDisplayer(bool _v,float _radius = 0)
    {
        if (_v)
        {
            rangeDisplayer.transform.localScale = new Vector3(_radius, _radius, _radius) * 2;
            rangeDisplayer.SetActive(_v);
        }
        else
        {
            rangeDisplayer.SetActive(_v);
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
            targetMarker.SetActive(false);
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
        directionPointer.SetActive(false);
        rangeDisplayer.SetActive(false);
        targetMarker.SetActive(false);
        activateTargetMarker = false;
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
    public void EvaluateGameResult(bool _playerVictory)
    {
        GameManager.instance.SetStartGame(false);
        EnemyManager.instance.Pause();

        // 计算金币 仅使用难度1系数 未知关卡难度选择操作
        if (_playerVictory)
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
            loot = (int)((Random.Range(100, 200) + timeReward) * PlayerManager.instance.GetCurrentLevelInfo().rewardFactor[0]);
        }
        else
        {
            // 失败时 金币结算
            loot = (int)(Random.Range(100, 200) * PlayerManager.instance.GetCurrentLevelInfo().rewardFactor[0] * (1 - cur_bossHP_Pencentage));
        }


        // 随机抽取3张卡
        List<CardBasicInfomation> lootCard = CardManager.instance.GetCardsRandomly(3);
        if (_playerVictory)
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
        foreach(var i in lootCard)
        {
            if (i.belongner != CharacterType.CharacterTag.Common)
                CardManager.instance.cardLibrary[i.id].quantity++;
            else
                CardManager.instance.cardLibrary_Common[i.id].level = 1;
        }

        if (_playerVictory)
        {
            PlayerManager.instance.UpdateVictoryTime();
        }

        // GUI 显示
        GUIManager.instance.EnableGameResult(_playerVictory, gameTimer, loot,lootCard);
    }

    /// <summary>
    /// 更新BOSS百分比血量
    /// </summary>
    /// <param name=""></param>
    public void UpdateBossHP(float _percentage)
    {
        cur_bossHP_Pencentage = _percentage;
    }
}
