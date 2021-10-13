using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SystemText : MonoBehaviour
{
    [Header("Objects")]
    public TextMeshProUGUI text;
    public Image image;

    [Header("Configuration")]
    public float fadeInTime = 0.25f;

    [Space]
    public float moveSpeed = 1.0f;
    public float fadeOutTime = 3.0f;


    public void SetText(string _text)
    {
        text.text = _text;
    }

    private void Awake()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
    }

    void Start()
    {
        text.DOFade(1, fadeInTime);
        image.DOFade(1, fadeInTime).OnComplete(() => { FadeOut(); });

    }

    public void FadeOut()
    {
        text.DOFade(0, fadeOutTime).SetEase(Ease.InQuint);
        image.DOFade(0, fadeOutTime).SetEase(Ease.InQuint).OnComplete(() => { Destroy(gameObject); });
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 nv = transform.position + Vector3.up * moveSpeed;
        transform.position = nv;
    }
}
