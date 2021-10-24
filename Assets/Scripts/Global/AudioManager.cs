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

    [Header("Audiosources")]
    public AudioSource audio_BGM;
    public List<AudioSource> audio_SE = new List<AudioSource>();

    [Header("Real-TIme Setting")]
    public int cur_BGM_Index = -1;
    public float volume_BGM = 0.4f;
    public float volume_SE = 0.7f;

    [Header("Global Sound Clip")]
    public List<AudioClip> sound_BossBGM = new List<AudioClip>();
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
        PlayBGM(3);
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

    /// <summary>
    /// ����boss BGM
    /// </summary>
    /// <param name="_index">012�ֱ��Ӧ������ ���� ��ĸ�� 3��������BGM</param>
    public void PlayBGM(int _index)
    {
        cur_BGM_Index = _index;

        if (!audio_BGM.isPlaying)
        {
            playBGM();
        }
        else
        {
            FadeOutBGM(playBGM);
        }
    }
    void playBGM()
    {
        // ����
        float temp = slider_BGM.value;
        audio_BGM.volume = 0;

        audio_BGM.clip = sound_BossBGM[cur_BGM_Index];
        audio_BGM.Play();

        DOTween.To(() => audio_BGM.volume, x => audio_BGM.volume = x, temp, 1f);
    }

    /// <summary>
    /// ��ͣBGM ʹ�佥��
    /// </summary>
    public void FadeOutBGM(TweenCallback _callback)
    {
        DOTween.To(() => audio_BGM.volume, x => audio_BGM.volume = x, 0, 1f).OnComplete(()=> { if (_callback != null) _callback(); });
    }

    /// <summary>
    /// �����Ƿ񽵵�BGM
    /// </summary>
    /// <param name="_v"></param>
    public void SetTurnDownBGM(bool _v)
    {
        if (_v)
        {
            DOTween.To(() => audio_BGM.volume, x => audio_BGM.volume = x, 0.05f, 1f);
        }
        else
        {
            DOTween.To(() => audio_BGM.volume, x => audio_BGM.volume = x, volume_BGM, 1f);
        }
    }

    // ������������
    public void ResetData()
    {
        slider_BGM.value = volume_BGM;
        audio_BGM.volume = volume_BGM;
        audio_BGM.playOnAwake = true;

        // ������Ч��Ƶ
        audio_SE.Clear();
        for(int i = 0;i< max_AudioSources_SE;i++)
        {
            audio_SE.Add(gameObject.AddComponent<AudioSource>());
            slider_SE.value = volume_SE;
            audio_SE[i].volume = volume_SE;
            audio_SE[i].playOnAwake = false;
        }
    }

    // �޸�������С
    public void ChangeVolume_BGM(float _v)
    {
        audio_BGM.volume = _v;
        volume_BGM = _v;
    }

    public void ChangeVolume_SE(float _v)
    {
        volume_SE = _v;
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
    /// ���Ű�ť��Ч ���������н�
    /// </summary>
    public void PlaySound_Button()
    {
        PlaySound(sound_btn[musicScroe[scroeIndex++]]);

        if (scroeIndex == musicScroe.Count)
            scroeIndex = 0;
    }

    /// <summary>
    /// �ͷſ�����Ч
    /// </summary>
    public void PlaySound_ReleaseCard()
    {
        PlaySound(sound_ReleaseCard);
    }
}
