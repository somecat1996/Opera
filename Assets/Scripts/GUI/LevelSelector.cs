using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class LevelSelector : MonoBehaviour
{
    List<GameObject> buttonList = new List<GameObject>();

    [Header("Configuration")]
    public float speed = 1.0f;
    public int offset = 250;
    public Vector3 maxScale = new Vector3(1.35f, 1.35f, 1.35f);
    public Color color_Disable = new Color(0.5f, 0.5f, 0.5f);

    [Header("Real-Time Data")]
    public int currentIndex = 0;
    private bool enable = true;


    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            buttonList.Add(transform.GetChild(i).gameObject);
        }

        InitializeAllButton();
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        InitializeAllButton();
    }

    public void InitializeAllButton()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(currentIndex == i)
            {
                buttonList[i].GetComponent<RectTransform>().localScale = maxScale;
            }
            else
            {
                buttonList[i].GetComponent<Image>().color = color_Disable;

                foreach (var j in buttonList[i].transform.GetComponentsInChildren<Image>())
                {
                    j.color = color_Disable;
                }

            }
        }
    }

    /// <summary>
    /// 确认进入关卡 并初始化一些战斗数据
    /// </summary>
    public void ConfirmLevel()
    {
        // 开启关卡
        GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>().EnterLevel(currentIndex);

        // Buff相关
        BuffManager.instance.DiableAllBuff(); // 清空BUFF
        PlayerManager.instance.EnableCharBuff(); // 启用角色被动
        BuffManager.instance.EnableAllSelectedBuff(); // 启用所有角色的ID

        // 卡牌相关
        CardManager.instance.ClearAllActivatedCard(); // 清除场上所有的卡牌实体
        CardManager.instance.RealignAndLoadCards(); // 增加场上卡牌
    }

    // 左旋
    public void TurnLeft()
    {
        if (currentIndex == 0 || !enable)
            return;
        currentIndex--;


        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform rect = buttonList[i].GetComponent<RectTransform>();
            Image image = buttonList[i].GetComponent<Image>();
            Vector3 pos = rect.localPosition;
            pos.x += offset;

            rect.transform.DOLocalMove(pos, speed);

            if (i == currentIndex)
            {
                rect.transform.DOScale(maxScale, speed);
                image.DOColor(Color.white,speed);

                foreach (var j in buttonList[i].transform.GetComponentsInChildren<Image>())
                {
                    j.DOColor(Color.white, speed);
                }
            }
            else
            {
                rect.transform.DOScale(Vector3.one, speed);
                image.DOColor(color_Disable, speed);

                foreach (var j in buttonList[i].transform.GetComponentsInChildren<Image>())
                {
                    j.DOColor(color_Disable, speed);
                }
            }
        }

        enable = false;
        Invoke("Enable", speed+0.1f);

    }
    
    // 右旋
    public void TurnRight()
    {
        if (currentIndex == transform.childCount-1 || !enable)
            return;
        currentIndex++;


        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform rect = buttonList[i].GetComponent<RectTransform>();
            Image image = buttonList[i].GetComponent<Image>();
            Vector3 pos = rect.localPosition;
            pos.x -= offset;

            rect.transform.DOLocalMove(pos, speed);

            if (i == currentIndex)
            {
                rect.transform.DOScale(maxScale, speed);
                image.DOColor(Color.white, speed);

                foreach (var j in buttonList[i].transform.GetComponentsInChildren<Image>())
                {
                    j.DOColor(Color.white, speed);
                }
            }
            else
            {
                rect.transform.DOScale(Vector3.one, speed);
                image.DOColor(color_Disable, speed);

                foreach (var j in buttonList[i].transform.GetComponentsInChildren<Image>())
                {
                    j.DOColor(color_Disable, speed);
                }
            }
        }

        enable = false;
        Invoke("Enable", speed + 0.1f);
    }

    private void Enable()
    {
        enable = true;
    }
}
