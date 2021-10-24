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
    /// ��ˢ��buff
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
    /// ѡ��Buff
    /// </summary>
    public void SelectBuff(int _btnIndex)
    {
        if (!activated) return;

        activated = false;
        BuffManager.instance.EnableBuff(buffID[_btnIndex]);

        // �˳�Buffѡ����Ҹ��ݽ׶��Ƿ�����Ļ��
        DisablePanel();
        BattleDataManager.instance.DisappealAppealPoint();

    }
    
    // �ر�BUFFѡ������Ҹ��ݽ׶�����Ļ��
    public void DisablePanel()
    {
        transform.DOLocalMoveY(y_Disable, moveDuration);
        GameManager.instance.SetPauseGame(false);

        
        if(BattleDataManager.instance.cur_Stage != 4)
        {
            GUIManager.instance.SetDisplayCurtain(false);
            AudioManager.instance.SetTurnDownBGM(false);
        }
        // ��Ϊ�׶�4 ��ѡ��buff���治��ӵ������Ļ���Ĺ��� ����ѡ����BUFF����н���
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
