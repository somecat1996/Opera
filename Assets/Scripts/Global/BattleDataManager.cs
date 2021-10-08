using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDataManager : MonoBehaviour
{
    public static BattleDataManager instance;

    [Header("Real-Time Data")]
    public float totalDamage = 0; // 由敌人Hurt函数上传伤害信息
    public int totalUsedCard = 0; // 由CardManger.SendToTempLayoutGroup上传
    public GameObjectBase lastTargetEnemy; // 由单体输出卡牌上传
    public CardPrototype lastUsedCard; // 由CardManger.SendToTempLayoutGroup上传
    [Space]
    public List<GameObjectBase> enemyList = new List<GameObjectBase>(); // 由敌人自身上传信息
    [Space]
    public bool playerMoving = false;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        ResetAllData();
    }

    // 重设数据——开启新关卡时调用
    public void ResetAllData()
    {
        totalDamage = 0;
        totalUsedCard = 0;
        lastTargetEnemy = null;
        lastUsedCard = null;

        enemyList.Clear();

        playerMoving = false;
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
}
