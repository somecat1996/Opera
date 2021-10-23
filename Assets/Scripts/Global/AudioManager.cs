using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Configuration")]
    public int max_AudioSources_SE = 10;
    public float fadeTime = 2f;

    public List<AudioClip> sound_BossBGM = new List<AudioClip>();

    [Header("Audiosources")]
    public AudioSource audio_BGM;
    public List<AudioSource> audio_SE = new List<AudioSource>();

    [Header("Real-TIme Setting")]
    public float volume_BGM = 1f;
    public float volume_SE = 1f;

    [Header("Global Sound Clip")]
    public AudioClip sound_ReleaseCard;

    [Header("Music Score")]
    public int scroeIndex = 0;
    public List<int> musicScroe = new List<int>();
    public List<AudioClip> sound_btn = new List<AudioClip>();

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
    /// 播放音效
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

    /// <summary>
    /// 播放boss BGM
    /// </summary>
    /// <param name="_bossIndex"></param>
    public void PlayBossBGM(int _bossIndex)
    {
        audio_BGM.clip = sound_BossBGM[_bossIndex];
        audio_BGM.Play();
    }

    /// <summary>
    /// 暂停boss BGM
    /// </summary>
    public void PauseBossBGM()
    {

    }

    // 重置所有设置
    public void ResetData()
    {
        audio_BGM.volume = 1;
        audio_BGM.playOnAwake = false;

        // 补充音效音频
        audio_SE.Clear();
        for(int i = 0;i< max_AudioSources_SE;i++)
        {
            audio_SE.Add(gameObject.AddComponent<AudioSource>());
            audio_SE[i].volume = 1;
            audio_SE[i].playOnAwake = false;
        }
    }

    // 修改声音大小
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

    /// <summary>
    /// 播放按钮音效 按照谱子行进
    /// </summary>
    public void PlaySound_Button()
    {
        PlaySound(sound_btn[musicScroe[scroeIndex++]]);

        if (scroeIndex == musicScroe.Count)
            scroeIndex = 0;
    }

    /// <summary>
    /// 释放卡牌音效
    /// </summary>
    public void PlaySound_ReleaseCard()
    {
        PlaySound(sound_ReleaseCard);
    }
}
