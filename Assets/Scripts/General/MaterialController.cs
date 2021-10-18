using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MaterialController : MonoBehaviour
{
    public Transform parent_MainSprites;
    public Transform parent_ShadowSprites;

    List<Renderer> mainRenderer = new List<Renderer>();
    List<Renderer> outlineRenderer = new List<Renderer>();

    void Start()
    {
        foreach(var i in parent_MainSprites.GetComponentsInChildren<Renderer>())
        {
            mainRenderer.Add(i);
        }
        foreach (var i in parent_ShadowSprites.GetComponentsInChildren<Renderer>())
        {
            outlineRenderer.Add(i);
        }
    }

    bool test = true;
    bool test2 = true;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SetEnableDissolution(test);
            test = !test;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            SetEnableStealth(test2);
            test2 = !test2;
        }
    }

    /// <summary>
    /// 清除所有效果
    /// </summary>
    public void ClearEffect()
    {
        SetEnableDissolution(false);
        SetEnableStealth(false);
    }

    /// <summary>
    /// 设置是否开启溶解效果
    /// </summary>
    /// <param name="_v"></param>
    /// <param name="_duration">溶解时间</param>
    ///  <param name="_callbackFun">溶解完成时 回调函数</param>
    public void SetEnableDissolution(bool _v,float _duration = 1f, TweenCallback _callbackFun = null)
    {
        if (_v)
        {
            foreach (var i in mainRenderer)
            {
                i.material.DOFloat(1, "DissolutionValue", _duration).OnComplete(()=> { if (_callbackFun != null) _callbackFun(); });
            }
            foreach (var i in outlineRenderer)
            {
                i.material.DOFloat(1, "DissolutionValue", _duration);
            }
        }
        else
        {
            foreach (var i in mainRenderer)
            {
                i.material.DOFloat(0, "DissolutionValue", _duration).OnComplete(() => { if (_callbackFun != null) _callbackFun(); });
            }
            foreach (var i in outlineRenderer)
            {
                i.material.DOFloat(0, "DissolutionValue", _duration);
            }
        }

    }

    public void SetEnableStealth(bool _v,float _duration = 1f,TweenCallback _callbackFun = null)
    {
        if (_v)
        {
            SetActiveOutlineSprites(true);

            foreach (var i in mainRenderer)
            {
                i.material.DOFloat(1, "StealthValue", _duration).OnComplete(() => { if (_callbackFun != null) _callbackFun(); });
            }
        }
        else
        {
            foreach (var i in mainRenderer)
            {
                i.material.DOFloat(0, "StealthValue", _duration).OnComplete(() => { if (_callbackFun != null) _callbackFun(); });
            }

            SetActiveOutlineSprites(false);
        }
    }

    /// <summary>
    /// 设置开关边缘渲染器
    /// </summary>
    /// <param name="_v"></param>
    public void SetActiveOutlineSprites(bool _v)
    {
        foreach(var i in outlineRenderer)
        {
            i.material.SetFloat("AlphaClip", _v==true?1.5f:1);
        }
    }
}
