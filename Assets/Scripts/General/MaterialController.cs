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
    /// 重新寻找渲染器
    /// </summary>
    /// <param name="mparent_MainSprite">主图层父对象</param>
    /// <param name="mparent_OutlineSprite">描边图层父对象</param>
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
    /// 清除所有效果
    /// </summary>
    public void ClearEffect()
    {
        SetEnableDissolution(false);
        SetEnableStealth(false);
        SetEnableChangeColor_Poison(false);
    }

    /// <summary>
    /// 设置是否开启溶解效果 结束时调用回调函数
    /// </summary>
    /// <param name="_v"></param>
    /// <param name="_duration">溶解时间</param>
    ///  <param name="_callbackFun">溶解完成时执行的无参函数</param>
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
    /// 设置开启隐身效果 当播放完成时自动回放
    /// </summary>
    /// <param name="_v"></param>
    /// <param name="_duration">持续时间</param>
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
    /// 设置开启中毒变色
    /// </summary>
    /// <param name="_v"></param>
    /// <param name="_duration">持续时间</param>
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
    /// 设置开关边缘渲染器
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
