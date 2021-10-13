using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDataManager : MonoBehaviour
{
    public static BattleDataManager instance;
    [Header("Real-Time Data")]
    public float totalDamage = 0; // 由敌人Hurt函数上传伤害信息
    public int totalUsedCard = 0; // 由CardManger.SendToTempLayoutGroup上传
    public GameObjectBase lastTargetEnemy; // 由单体输出卡牌上传
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
                    targetMarker.transform.position = Camera.main.WorldToScreenPoint(hit.transform.position) + markerOffset;
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
        totalDamage = 0;
        totalUsedCard = 0;
        lastTargetEnemy = null;
        lastUsedCard = null;

        enemyList.Clear();

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
}
