using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

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
            }
        }
    }

    // 确认进入关卡
    public void ConfirmLevel()
    {
        GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>().EnterLevel(currentIndex);
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
                
            }
            else
            {
                rect.transform.DOScale(Vector3.one, speed);
                image.DOColor(color_Disable, speed);
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
            }
            else
            {
                rect.transform.DOScale(Vector3.one, speed);
                image.DOColor(color_Disable, speed);
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
