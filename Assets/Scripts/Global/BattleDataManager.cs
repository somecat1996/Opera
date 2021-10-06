using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDataManager : MonoBehaviour
{
    public static BattleDataManager instance;

    [Header("Real-Time Data")]
    public float totalDamage = 0;
    public int totalUsedCard = 0;
    public GameObjectBase lastTargetEnemy;
    public CardPrototype lastUsedCard;
    [Space]
    public List<GameObjectBase> enemyList = new List<GameObjectBase>();
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

    // �������ݡ��������¹ؿ�ʱ����
    public void ResetAllData()
    {
        totalDamage = 0;
        totalUsedCard = 0;
        lastTargetEnemy = null;
        lastUsedCard = null;

        enemyList.Clear();

        playerMoving = false;
    }

    // �����˺�����
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

    // ��������ƶ�״̬ true->�ƶ��� false->δ�ƶ�
    public void UpdatePlayerMovingStatus(bool _v)
    {
        playerMoving = _v;
    }


    // ����(�Ƴ�)ս���ϵ�����Ϣ �����ɺ�*���˶�������ǰ*����
    public void AddEnemyData(GameObjectBase _gob)
    {
        enemyList.Add(_gob);
    }
    public void RemoveEnemyData(GameObjectBase _gob)
    {
        enemyList.Remove(_gob);
    }
}
