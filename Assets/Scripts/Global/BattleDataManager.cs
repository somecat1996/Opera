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
    public float gameTimer = 0; // ��Ϸ�г�
    public int loot = 0; // ս��Ʒ���
    public bool playerVictory = false;
    [Space]
    public float totalDamage = 0; // �ɵ���Hurt�����ϴ��˺���Ϣ
    public int totalUsedCard = 0; // ��CardManger.SendToTempLayoutGroup�ϴ�
    public GameObjectBase lastTargetEnemy; // ��һ�������幥���ĵ��� �ɵ�����������ϴ�
    public float cur_bossHP_Percentage = 0; // ��ǰbossʣ��Ѫ���ٷֱ�
    public int cur_Stage = 0; // ��ǰ�׶�
    public float timer_LastStage = 0; // ��һ�׶ν��е�ʱ��
    [Space]
    public CardPrototype selectingCard; // ��ҵ�ǰѡ�еĿ��� �� ����Ҫʹ�õĿ���
    public CardPrototype lastUsedCard; // ��CardManger.SendToTempLayoutGroup�ϴ�
    [Space]
    public List<GameObjectBase> enemyList = new List<GameObjectBase>(); // �ɵ��������ϴ���Ϣ
    [Space]
    public bool playerMoving = false; // ����Ƿ��ƶ�
    [Space]
    public float appealPoint = 0;// �Ȳ�ֵ

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
    [Header("Spectator Objects And RealTime Data")]
    public int activatedSpectator = 0;
    public int highlightSpectator = 0;

    [Space]
    // ����ʵ�� �ֱ�Ϊ δ����-����-�߹�
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

        // �׶���Ϣ
        cur_Stage = 0;
        timer_LastStage = 0;

        // �Ȳ�ֵ���
        appealPoint = 0;

        // �������
        // �������Ѿ�����Ĺ������óɼ�Ӱ
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

        // BOSS��Ϣ
        cur_bossHP_Percentage = 1;
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
    ///     /// <param name="_animate">�Ƿ���ҪĻ������</param>
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
    /// ǿ��ʧ��
    /// </summary>
    public void ForceDefeated()
    {
        EvaluateGameResult(false, true);
    }
    void evaluateGameResult()
    {
        // ������ ��ʹ���Ѷ�1ϵ�� δ֪�ؿ��Ѷ�ѡ�����
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
            // ʧ��ʱ ��ҽ���
            loot = GlobalValue.GetTrueLoot((Random.Range(100, 200) * PlayerManager.instance.GetCurrentLevelInfo().rewardFactor[0] * (1 - cur_bossHP_Percentage)));
        }


        // �����ȡ3�ſ�
        List<CardBasicInfomation> lootCard = CardManager.instance.GetCardsRandomly(3);
        if (playerVictory)
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
        foreach (var i in lootCard)
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

        // �������������
        if (playerVictory)
        {
            PlayerManager.instance.UpdateVictoryTime();
            // ������һ��
            PlayerManager.instance.UnlockLevel(PlayerManager.instance.cur_LevelIndex + 1);

            AudioManager.instance.PlaySound(sound_Victory);
        }
        else
        {
            AudioManager.instance.PlaySound(sound_Defeated);
        }

        // GUI ��ʾ
        GUIManager.instance.EnableGameResult(playerVictory, gameTimer, loot, lootCard);
    }


    /// <summary>
    /// ����BOSSѪ��
    /// </summary>
    /// <param name="_percentage">�ٷֱ�</param>
    /// <param name="_v">Ѫ��</param>
    public void UpdateBossHP(float _percentage,float _v)
    {
        cur_bossHP_Percentage = _percentage;
        cur_bossHP_Percentage = Mathf.Clamp(cur_bossHP_Percentage, 0, Mathf.Infinity);
        GUIManager.instance.UpdateBossHealthPoint(cur_bossHP_Percentage,_v);
    }

    /// <summary>
    /// ���µ�ǰ�׶���
    /// </summary>
    /// <param name="_v"></param>
    public void UpdateStage(int _v)
    {
        // ��ͷ�����н���
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
    /// �÷�����Ļ���ص�����ִ��
    /// </summary>
    void updateStage()
    {
        // ������һ�׶κȲ�ֵ����Ǯ
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

        // �����ʵ�Ȳ�ֵ
        tempAP = GlobalValue.GetTrueReward(tempAP);

        // ���ݵ��׶κȲ�ֵ�ظ�Ѫ��
        if (tempAP >= 30 && tempAP <= 49)
        {
            PlayerManager.instance.player.InstantHealing(60 + GlobalValue.hpIncrement_Reward);
            GUIManager.instance.SpawnSystemText("�ظ� " + (60 + GlobalValue.hpIncrement_Reward) + " ��Χֵ");
        }
        else if (tempAP >= 50 && tempAP <= 79)
        {
            PlayerManager.instance.player.InstantHealing(80 + GlobalValue.hpIncrement_Reward);
            GUIManager.instance.SpawnSystemText("�ظ� " + (80 + GlobalValue.hpIncrement_Reward) + " ��Χֵ");
        }
        else if (tempAP >= 80)
        {
            PlayerManager.instance.player.InstantHealing(100 + GlobalValue.hpIncrement_Reward);
            GUIManager.instance.SpawnSystemText("�ظ� " + (100 + GlobalValue.hpIncrement_Reward) + " ��Χֵ");
        }

        if (tempAP <= 60)
        {
            PlayerManager.instance.player.InstantHealing(100 + GlobalValue.hpIncrement_Reward);
            GUIManager.instance.SpawnSystemText("�ظ� " + (100 + GlobalValue.hpIncrement_Reward) + " ��Χֵ");
        }

        appealPoint += tempAP;

        // ���ݺȲ�ֵ��ʾ��������
        if (CheckInRange(appealPoint, 30, 39))
        {
            int rSpect_Act = 1 - activatedSpectator + highlightSpectator; // ʣ����Ҫ����Ĺ���
            int rSpect_Hl = 0 - highlightSpectator; // ʣ����Ҫ�߹�Ĺ���

            rSpect_Act = Mathf.Clamp(rSpect_Act, 0, 8);
            rSpect_Hl = Mathf.Clamp(rSpect_Hl, 0, 8);

            StartCoroutine(HandleSpcetator(rSpect_Act, rSpect_Hl));

            AudioManager.instance.PlaySound(sound_Applause[0]);
            
        }
        else if (CheckInRange(appealPoint, 40, 79))
        {
            int rSpect_Act = 3 - activatedSpectator + highlightSpectator; // ʣ����Ҫ����Ĺ���
            int rSpect_Hl = 1 - highlightSpectator; // ʣ����Ҫ�߹�Ĺ���

            rSpect_Act = Mathf.Clamp(rSpect_Act, 0, 8);
            rSpect_Hl = Mathf.Clamp(rSpect_Hl, 0, 8);

            StartCoroutine(HandleSpcetator(rSpect_Act, rSpect_Hl));

            AudioManager.instance.PlaySound(sound_Applause[0]);
        }
        else if (CheckInRange(appealPoint,80, 119))
        {
            int rSpect_Act = 5 - activatedSpectator + highlightSpectator; // ʣ����Ҫ����Ĺ���
            int rSpect_Hl = 1 - highlightSpectator; // ʣ����Ҫ�߹�Ĺ���

            rSpect_Act = Mathf.Clamp(rSpect_Act, 0, 8);
            rSpect_Hl = Mathf.Clamp(rSpect_Hl, 0, 8);

            StartCoroutine(HandleSpcetator(rSpect_Act, rSpect_Hl));

            AudioManager.instance.PlaySound(sound_Applause[1]);
        }
        else if (CheckInRange(appealPoint, 120, 149))
        {
            int rSpect_Act = 5 - activatedSpectator + highlightSpectator; // ʣ����Ҫ����Ĺ���
            int rSpect_Hl = 2 - highlightSpectator; // ʣ����Ҫ�߹�Ĺ���

            rSpect_Act = Mathf.Clamp(rSpect_Act, 0, 8);
            rSpect_Hl = Mathf.Clamp(rSpect_Hl, 0, 8);

            StartCoroutine(HandleSpcetator(rSpect_Act, rSpect_Hl));

            AudioManager.instance.PlaySound(sound_Applause[1]);
        }
        else if (CheckInRange(appealPoint, 150, 179))
        {
            int rSpect_Act = 6 - activatedSpectator + highlightSpectator; // ʣ����Ҫ����Ĺ���
            int rSpect_Hl = 2 - highlightSpectator; // ʣ����Ҫ�߹�Ĺ���

            rSpect_Act = Mathf.Clamp(rSpect_Act, 0, 8);
            rSpect_Hl = Mathf.Clamp(rSpect_Hl, 0, 8);

            StartCoroutine(HandleSpcetator(rSpect_Act, rSpect_Hl));

            AudioManager.instance.PlaySound(sound_Applause[2]);
        }
        else if (CheckInRange(appealPoint, 180, 219))
        {
            int rSpect_Act = 6 - activatedSpectator + highlightSpectator; // ʣ����Ҫ����Ĺ���
            int rSpect_Hl = 3 - highlightSpectator; // ʣ����Ҫ�߹�Ĺ���

            rSpect_Act = Mathf.Clamp(rSpect_Act, 0, 8);
            rSpect_Hl = Mathf.Clamp(rSpect_Hl, 0, 8);

            StartCoroutine(HandleSpcetator(rSpect_Act, rSpect_Hl));

            AudioManager.instance.PlaySound(sound_Applause[2]);
        }
        else if (appealPoint >= 220)
        {
            int rSpect_Act = 6 - activatedSpectator + highlightSpectator; // ʣ����Ҫ����Ĺ���
            int rSpect_Hl = 6 - highlightSpectator; // ʣ����Ҫ�߹�Ĺ���

            rSpect_Act = Mathf.Clamp(rSpect_Act, 0, 8);
            rSpect_Hl = Mathf.Clamp(rSpect_Hl, 0, 8);

            StartCoroutine(HandleSpcetator(rSpect_Act, rSpect_Hl));

            AudioManager.instance.PlaySound(sound_Applause[2]);
        }

        // ���½׶���
        cur_Stage++;

        // ���ý׶μ�ʱ��
        timer_LastStage = 0;

        // ��ʾ�Ȳ�ֵ�ı�
        DisplayAppealPoint();
    }

    IEnumerator HandleSpcetator(int _remainActivated,int _remainHighlight)
    {
        // ���ȼ������
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

        // ֮��߹��Ѿ�����Ĺ���
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

        // ���ϸ��ż�������
        if(appealPoint >= 150 && spectator_Spectial.color == Color.black)
        {
            spectator_Spectial.GetComponent<Spectator>().Activate();
        }
        if(appealPoint >= 220)
        {
            spectator_Spectial.GetComponent<Spectator>().Highlight();
        }

        // ���ݵ�ǰ��ѡBUFF�������Ƿ��������ѡ��Buff
        if(BuffManager.instance.activiatedBuffList.Count >= 10)
        {
            // Buff����
            Curtain.instance.SetActivatable(true);
        }
        else
        {
            // ��ʾbuffѡ����
            BuffSelector.instance.EnablePanel();
        }
    }

    // ��ʾ�Ȳ�ֵ�ı�
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

    // �����������
    void ActivatedFinalSpectator()
    {
        spectator_Spectial.GetComponent<Spectator>().Activate();
    }
    // �߹��������
    void HighlightFinalSpectator()
    {
        spectator_Spectial.GetComponent<Spectator>().Highlight();
    }

    /// <summary>
    /// ���������������Ƿ񱻼���
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
