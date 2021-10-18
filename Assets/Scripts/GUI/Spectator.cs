using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Spectator : MonoBehaviour
{
    Image image_OnGameScene;
    public Image image_UnderCurtain;

    public float animationSpeed = 0.5f;

    public static Color color_Activated = new Color(0.8f,0.8f,0.8f,1);
    public static Color color_Highlight = Color.white;

    void Start()
    {
        image_OnGameScene = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 激活观众
    public void Activate()
    {
        image_OnGameScene.DOColor(color_Activated, animationSpeed);
        image_UnderCurtain.DOColor(color_Activated, animationSpeed);
    }

    // 高光观众
    public void Highlight()
    {
        image_OnGameScene.DOColor(color_Highlight, animationSpeed);
        image_UnderCurtain.DOColor(color_Highlight, animationSpeed);
    }

    // 反激活
    public void Deactivate()
    {
        image_OnGameScene.DOColor(Color.black, 0.01f);
        image_UnderCurtain.DOColor(Color.black, 0.01f);
    }
}
