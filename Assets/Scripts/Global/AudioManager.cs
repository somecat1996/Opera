using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Configuration")]
    public int max_AudioSources_SE = 5;

    [Header("Audiosources")]
    public AudioSource audio_BGM;
    public List<AudioSource> audio_SE = new List<AudioSource>();

    [Header("Real-TIme Setting")]
    public float volume_BGM = 1f;
    public float volume_SE = 1f;

    [Header("Objects")]
    public Slider slider_BGM;
    public Slider slider_SE;



    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetData();
    }

    /// <summary>
    /// ������Ч
    /// </summary>
    /// <param name="_clip"></param>
    public void PlaySound(AudioClip _clip)
    {
        AudioSource tempAS = audio_SE[0];
        tempAS.clip = _clip;
        tempAS.Play();
        audio_SE.RemoveAt(0);
        audio_SE.Add(tempAS);
    }

    public void PlayBGM(AudioClip _clip)
    {
        audio_BGM.clip = _clip;
        audio_BGM.Play();
    }

    // ������������
    public void ResetData()
    {
        audio_BGM.volume = 1;
        audio_BGM.playOnAwake = false;

        // ������Ч��Ƶ
        audio_SE.Clear();
        for(int i = 0;i< max_AudioSources_SE;i++)
        {
            audio_SE.Add(gameObject.AddComponent<AudioSource>());
            audio_SE[i].volume = 1;
            audio_SE[i].playOnAwake = false;
        }
    }

    // �޸�������С
    public void ChangeVolume_BGM(float _v)
    {
        audio_BGM.volume = _v;
    }

    public void ChangeVolume_SE(float _v)
    {
        foreach (var i in audio_SE)
        {
            i.volume = _v;
        }
    }

    public void AddVolume_BGM(float _v)
    {
        slider_BGM.value += _v;
    }
    public void AddVolume_SE(float _v)
    {
        slider_SE.value += _v;
    }
}
