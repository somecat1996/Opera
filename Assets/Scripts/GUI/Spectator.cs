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

    public static Color color_Activated = new Color(0.3f,0.3f,0.3f,1);
    public static Color color_Highlight = Color.white;

    public float blinkInterval = 0.5f;
    public Coroutine timer;
    public GameObject moodImage;

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

        timer = StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (moodImage.transform.parent.parent.parent.gameObject.activeSelf)
        {
            moodImage.SetActive(!moodImage.activeSelf);

            yield return new WaitForSeconds(blinkInterval);
        }
        moodImage.SetActive(false);
        StopCoroutine(timer);
    }

    // 反激活
    public void Deactivate()
    {
        image_OnGameScene.DOColor(Color.black, 0.01f);
        image_UnderCurtain.DOColor(Color.black, 0.01f);
    }
}
