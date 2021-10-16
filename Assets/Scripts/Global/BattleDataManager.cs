using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleDataManager : MonoBehaviour
{
    public static BattleDataManager instance;
    [Header("Real-Time Data")]
    public float gameTimer = 0;
    public int loot = 0;

    public float totalDamage = 0; // �ɵ���Hurt�����ϴ��˺���Ϣ
    public int totalUsedCard = 0; // ��CardManger.SendToTempLayoutGroup�ϴ�
    public GameObjectBase lastTargetEnemy; // ��һ�������幥���ĵ��� �ɵ�����������ϴ�
    public float cur_bossHP_Pencentage = 0;
    [Space]
    public CardPrototype selectingCard; // ��ҵ�ǰѡ�еĿ��� �� ����Ҫʹ�õĿ���
    public CardPrototype lastUsedCard; // ��CardManger.SendToTempLayoutGroup�ϴ�
    [Space]
    public List<GameObjectBase> enemyList = new List<GameObjectBase>(); // �ɵ��������ϴ���Ϣ
    [Space]
    public bool playerMoving = false;

    [Header("Obejcts And Related Configuration")]
    // ͨ������
    [Space]
    public float fadeTime = 0.5f;
    // ��Χ����ָʾ��
    public GameObject rangeDisplayer;
    public bool activateRangeDIsplayer = false;
    [Space]
    // ���忨��ָʾ��
    public Vector3 markerOffset;
    public bool activateTargetMarker;
    public GameObject targetMarker;
    [Space]
    // �����Կ���ָʾ��
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
    /// ������Χ��ʾ��
    /// </summary>
    /// <param name="_v"></param>
    /// <param name="_radius">�뾶</param>
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
    /// ��������Ŀ��ָʾ��
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
    /// ��������ָʾ��
    /// </summary>
    /// <param name="_v"></param>
    public void SetActiveDirectionPointer(bool _v)
    {
        directionPointer.SetActive(_v);
    }



    /// <summary>
    ///  �������ݡ��������¹ؿ�ʱ����
    /// </summary>
    public void ResetAllData()
    {
        // ϵͳ��Ϣ
        gameTimer = 0;

        // �˺���ֵ
        totalDamage = 0;
        totalUsedCard = 0;
        lastTargetEnemy = null;
        lastUsedCard = null;

        // ���ϵ����б�
        enemyList.Clear();

        // �����Ϣ
        playerMoving = false;

        // ָʾ��
        //directionPointer.SetActive(false);
        //rangeDisplayer.SetActive(false);
        //targetMarker.SetActive(false);
        activateTargetMarker = false;
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
    public void UpdateSelectingCard(CardPrototype _cp)
    {
        selectingCard = _cp;
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

    /// <summary>
    /// ��Ϸ���� ������Ϸ���������Ϸ����ʱ����
    /// </summary>
    /// <param name="_playerVictory">����Ƿ�ʤ��</param>
    public void EvaluateGameResult(bool _playerVictory)
    {
        GameManager.instance.SetStartGame(false);
        GameManager.instance.SetPauseGame(false);

        // ������ ��ʹ���Ѷ�1ϵ�� δ֪�ؿ��Ѷ�ѡ�����
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
            // ʧ��ʱ ��ҽ���
            loot = (int)(Random.Range(100, 200) * PlayerManager.instance.GetCurrentLevelInfo().rewardFactor[0] * (1 - cur_bossHP_Pencentage));
        }


        // �����ȡ3�ſ�
        List<CardBasicInfomation> lootCard = CardManager.instance.GetCardsRandomly(3);
        if (_playerVictory)
        {
            // ���鿨��ʱ�б�
            List<CardBasicInfomation> lootCard_Common = new List<CardBasicInfomation>();
            foreach (var i in PlayerManager.instance.GetCurrentLevelInfo().lootCard)
            {
                // ����Ӧ���鿨��δ�������ݴ�
                if (CardManager.instance.cardLibrary_Common[i].level == 0)
                    lootCard_Common.Add(CardManager.instance.cardLibrary_Common[i]);
            }
            if (lootCard_Common.Count != 0)
            {
                // ������Ƴ�һ�� ֮��������һ��
                lootCard.RemoveAt(Random.Range(0, lootCard.Count));
                lootCard.Add(lootCard_Common[Random.Range(0, lootCard_Common.Count)]);
            }
        }
        else
        {
            lootCard.Clear();
        }


        // ʵ����ֵ���䵽CardManager��PlayerManager;
        PlayerManager.instance.ChangeMoney(loot);
        foreach(var i in lootCard)
        {
            if (i.belongner != CharacterType.CharacterTag.Common)
            {
                CardManager.instance.cardLibrary[i.id].quantity++;
            }
            else
            {
                CardManager.instance.cardLibrary_Common[i.id].level = 1;

                // ��þ��鿨�� ����ˢ�������ѡ�Ŀ��� *****ע��***** �˴���ʱʹ�� ���������ѡ�Ŀ��� �պ���Ҫ�Ż��������
                // �����ѡ���Ƶ���Ϣ
                GUIManager.instance.ClearUnselectedCardList();
                CardManager.instance.ClearSelectedCard();

                // ��������ͨ�ÿ��Ƶ�ѡ��������
                CardManager.instance.LoadAllCardIntoUnselectedList();
            }
               
        }

        if (_playerVictory)
        {
            PlayerManager.instance.UpdateVictoryTime();
        }

        // GUI ��ʾ
        GUIManager.instance.EnableGameResult(_playerVictory, gameTimer, loot,lootCard);
    }

    /// <summary>
    /// ����BOSS�ٷֱ�Ѫ��
    /// </summary>
    /// <param name=""></param>
    public void UpdateBossHP(float _percentage)
    {
        cur_bossHP_Pencentage = _percentage;
    }
}
