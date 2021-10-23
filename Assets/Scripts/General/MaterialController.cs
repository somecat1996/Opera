using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MaterialController : MonoBehaviour
{
    public Transform parent_MainSprite;
    public Transform parent_OutlineSprite;

    List<Renderer> mainRenderer = new List<Renderer>();
    List<Renderer> outlineRenderer = new List<Renderer>();

    Color color_Poison = new Color(0.2491f, 0.9905f, 0.4156f, 1);

    void Start()
    {
        foreach(var i in parent_MainSprite.GetComponentsInChildren<Renderer>())
        {
            mainRenderer.Add(i);
        }
        foreach (var i in parent_OutlineSprite.GetComponentsInChildren<Renderer>())
        {
            outlineRenderer.Add(i);
        }
    }

    /// <summary>
    /// ����Ѱ����Ⱦ��
    /// </summary>
    /// <param name="mparent_MainSprite">��ͼ�㸸����</param>
    /// <param name="mparent_OutlineSprite">���ͼ�㸸����</param>
    public void ResearchRenderer(Transform mparent_MainSprite,Transform mparent_OutlineSprite)
    {
        parent_MainSprite = mparent_MainSprite;
        parent_OutlineSprite = mparent_OutlineSprite;

        foreach (var i in this.parent_MainSprite.GetComponentsInChildren<Renderer>())
        {
            mainRenderer.Add(i);
        }
        foreach (var i in this.parent_OutlineSprite.GetComponentsInChildren<Renderer>())
        {
            outlineRenderer.Add(i);
        }
    }

    /// <summary>
    /// �������Ч��
    /// </summary>
    public void ClearEffect()
    {
        SetEnableDissolution(false);
        SetEnableStealth(false);
        SetEnableChangeColor_Poison(false);
    }

    /// <summary>
    /// �����Ƿ����ܽ�Ч�� ����ʱ���ûص�����
    /// </summary>
    /// <param name="_v"></param>
    /// <param name="_duration">�ܽ�ʱ��</param>
    ///  <param name="_callbackFun">�ܽ����ʱִ�е��޲κ���</param>
    public void SetEnableDissolution(bool _v,float _duration = 1.5f, TweenCallback _callbackFun = null)
    {
        if (_v)
        {
            foreach (var i in mainRenderer)
            {
                i.material.DOFloat(1, "DissolutionValue", _duration).OnComplete(()=> { if (_callbackFun != null) _callbackFun(); });
            }
            foreach(var i in outlineRenderer)
            {
                i.material.DOFloat(1, "DissolutionValue", _duration);
            }
        }
        else
        {
            foreach (var i in mainRenderer)
            {
                i.material.DOFloat(0, "DissolutionValue", _duration);
            }
            foreach (var i in outlineRenderer)
            {
                i.material.DOFloat(0, "DissolutionValue", _duration);
            }
        }

    }

    /// <summary>
    /// ���ÿ�������Ч�� ���������ʱ�Զ��ط�
    /// </summary>
    /// <param name="_v"></param>
    /// <param name="_duration">����ʱ��</param>
    /// <param name="_callbackFun"></param>
    public void SetEnableStealth(bool _v,float _duration = 1f)
    {
        if (_v)
        {
            SetDisableOutlineSprites(true);

            foreach (var i in mainRenderer)
            {
                i.material.DOFloat(1, "StealthValue", _duration);
            }
        }
        else
        {
            foreach (var i in mainRenderer)
            {
                i.material.DOFloat(0, "StealthValue", _duration);
            }

            SetDisableOutlineSprites(false);

        }
    }

    /// <summary>
    /// ���ÿ����ж���ɫ
    /// </summary>
    /// <param name="_v"></param>
    /// <param name="_duration">����ʱ��</param>
    /// <param name="_callbackFun"></param>
    public void SetEnableChangeColor_Poison(bool _v, float _duration = 0.5f)
    {
        if (_v)
        {
            foreach (var i in mainRenderer)
            {
                i.material.DOVector(color_Poison, "LitColor", _duration);
            }
        }
        else
        {
            foreach (var i in mainRenderer)
            {
                i.material.DOVector(Color.white, "LitColor", _duration).OnComplete(() => { SetDisableOutlineSprites(false); });
            }

          
        }
    }

    /// <summary>
    /// ���ÿ��ر�Ե��Ⱦ��
    /// </summary>
    /// <param name="_v"></param>
    void SetDisableOutlineSprites(bool _v)
    {
        foreach(var i in outlineRenderer)
        {
            i.material.SetFloat("AlphaClip", _v==true?1.5f:1);
        }
    }
}
