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
        // 首次运行 锁定
        if (PlayerManager.instance.data.firstTimeRunning)
            GUIManager.instance.LockButton_Continue(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // 初始化所有数据-新开游戏时调用
    public void InitializeAllData()
    {
        PlayerManager.instance.data.firstTimeRunning = false;

        PlayerManager.instance.InitializeData(); // 初始化玩家数据以及关卡信息
        CardManager.instance.InitializeAllCards(); // 初始化卡牌数据
    }

    /// <summary>
    /// 检测是否存在存档
    /// </summary>
    public void CheckExistArchive()
    {
        // 无存档
        if (PlayerManager.instance.data.firstTimeRunning)
        {
            InitializeAllData();
            GUIManager.instance.SetActivePanelTitle(false);
        }
        // 有存档
        else
        {
            GUIManager.instance.DisplayConfirmDiaglog_NewGame();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // 载入玩家和卡牌数据 *暂时不用* 数据载入放到各个脚本的awake中
    public void LoadData()
    {
        //CardManager.instance.LoadCardInstance(); // 载入所有卡牌实体
        //CardManager.instance.LoadCardLibrary(); // 将所有卡牌信息载入到游戏中
        //BuffManager.instance.LoadAllBuffInstances(); // 载入BUFF实体
    }

    /// <summary>
    /// 检测是否处于有效游戏战斗场景
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
