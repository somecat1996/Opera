using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Real-Time Data")]
    public bool gameStart = false;
    public bool gamePause = false;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // �״����� ����
        if (PlayerManager.instance.data.firstTimeRunning)
            GUIManager.instance.LockButton_Continue(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // ��ʼ����������-�¿���Ϸʱ����
    public void InitializeAllData()
    {
        PlayerManager.instance.data.firstTimeRunning = false;

        PlayerManager.instance.InitializeData(); // ��ʼ����������Լ��ؿ���Ϣ
        CardManager.instance.InitializeAllCards(); // ��ʼ����������
    }

    /// <summary>
    /// ����Ƿ���ڴ浵
    /// </summary>
    public void CheckExistArchive()
    {
        // �޴浵
        if (PlayerManager.instance.data.firstTimeRunning)
        {
            InitializeAllData();
            GUIManager.instance.SetActivePanelTitle(false);
        }
        // �д浵
        else
        {
            GUIManager.instance.DisplayConfirmDiaglog_NewGame();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // ������ҺͿ������� *��ʱ����* ��������ŵ������ű���awake��
    public void LoadData()
    {
        //CardManager.instance.LoadCardInstance(); // �������п���ʵ��
        //CardManager.instance.LoadCardLibrary(); // �����п�����Ϣ���뵽��Ϸ��
        //BuffManager.instance.LoadAllBuffInstances(); // ����BUFFʵ��
    }

    /// <summary>
    /// ����Ƿ�����Ч��Ϸս������
    /// </summary>
    /// <returns></returns>
    public bool CheckIfGameRunning()
    {
        return gameStart && !gamePause;
    }

    public void SetPauseGame(bool _v)
    {
        gamePause = _v;
    }

    public void SetStartGame(bool _v)
    {
        gameStart = _v;
    }
}
