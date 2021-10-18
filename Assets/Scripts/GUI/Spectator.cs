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

    public static Color color_Activated;
    public static Color color_Highligh;

    void Start()
    {
        image_OnGameScene = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ¼¤»î¹ÛÖÚ
    public void Activate()
    {
        image_OnGameScene.DOColor(color_Activated, animationSpeed);
        image_UnderCurtain.DOColor(color_Activated, animationSpeed);
    }
}
