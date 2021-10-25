using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;


/**
 *                  *************注意*************
 *  LevelSelector转变为难度选择器 关卡选择界面不在有选择指定关卡的功能
 *  仅用于选择相对应的难度系数
 * 
 */
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
    /// 通知PlayManager生成关卡队列并进入关卡
    /// </summary>
    public void Confirm()
    {
        GUIManager.instance.DisplayCurtain(confirm);

    }
    void confirm()
    {
        PlayerManager.instance.SpawnLevelIndexList();
    }

    // 左旋
    public void TurnLeft()
    {
        if (currentIndex == 0 || !enable)
            return;
        currentIndex--;
        PlayerManager.instance.cur_Difficity = currentIndex;

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
        PlayerManager.instance.cur_Difficity = currentIndex;

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
