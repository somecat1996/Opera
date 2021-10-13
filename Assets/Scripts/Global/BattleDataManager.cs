using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDataManager : MonoBehaviour
{
    public static BattleDataManager instance;
    [Header("Real-Time Data")]
    public float totalDamage = 0; // �ɵ���Hurt�����ϴ��˺���Ϣ
    public int totalUsedCard = 0; // ��CardManger.SendToTempLayoutGroup�ϴ�
    public GameObjectBase lastTargetEnemy; // �ɵ�����������ϴ�
    [Space]
    public CardPrototype selectingCard; // ��ҵ�ǰѡ�еĿ��� �� ����Ҫʹ�õĿ���
    public CardPrototype lastUsedCard; // ��CardManger.SendToTempLayoutGroup�ϴ�
    [Space]
    public List<GameObjectBase> enemyList = new List<GameObjectBase>(); // �ɵ��������ϴ���Ϣ
    [Space]
    public bool playerMoving = false;

    [Header("Obejcts And Related Configuration")]
    // ��Χ����ָʾ��
    public GameObject rangeDisplayer;
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
    /// ������Χ��ʾ��
    /// </summary>
    /// <param name="_v"></param>
    /// <param name="_radius">�뾶</param>
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
    /// ��������Ŀ��ָʾ��
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
        totalDamage = 0;
        totalUsedCard = 0;
        lastTargetEnemy = null;
        lastUsedCard = null;

        enemyList.Clear();

        playerMoving = false;

        // ָʾ��
        directionPointer.SetActive(false);
        rangeDisplayer.SetActive(false);
        targetMarker.SetActive(false);
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
}
