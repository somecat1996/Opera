using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BuffSelector : MonoBehaviour
{
    public static BuffSelector instance;

    public int[] buffID = new int[3];

    public Image[] btnImage = new Image[3];
    public TextMeshProUGUI[] buffDesc = new TextMeshProUGUI[3];

    public bool activated = false;

    public float moveDuration = 1f;
    public int y_Enable = 53;
    public int y_Disable = 355;

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

    /// <summary>
    /// 重刷新buff
    /// </summary>
    public void ReflashBuffBtn()
    {
        int index = 0;
        foreach(var i in BuffManager.instance.GetBuffInfoRandomly(3))
        {
            buffID[index] = i.id;
            btnImage[index].sprite = i.icon;
            buffDesc[index].text = "[" + i.buffName + "]\n" + BuffManager.instance.buffInstanceLibrary[i.id].GetComponent<BuffPrototype>().GetDesc();

            index++;
        }

        activated = true;
    }

    /// <summary>
    /// 选择Buff
    /// </summary>
    public void SelectBuff(int _btnIndex)
    {
        if (!activated) return;

        activated = false;
        BuffManager.instance.EnableBuff(buffID[_btnIndex]);

        // 退出Buff选择框且根据阶段是否拉开幕布
        DisablePanel();
        BattleDataManager.instance.DisappealAppealPoint();

    }
    
    // 关闭BUFF选择面板且根据阶段拉开幕布
    public void DisablePanel()
    {
        transform.DOLocalMoveY(y_Disable, moveDuration);
        GameManager.instance.SetPauseGame(false);

        
        if(BattleDataManager.instance.cur_Stage != 4)
        {
            GUIManager.instance.SetDisplayCurtain(false);
            AudioManager.instance.SetTurnDownBGM(false);
        }
        // 若为阶段4 则选择buff界面不在拥有拉开幕布的功能 且在选择完BUFF后进行结算
        else if (BattleDataManager.instance.cur_Stage == 4)
        {
            BattleDataManager.instance.EvaluateGameResult(true, false);
        }
            
    }
    public void EnablePanel()
    {
        transform.DOLocalMoveY(y_Enable, moveDuration);
        ReflashBuffBtn();
    }
}
