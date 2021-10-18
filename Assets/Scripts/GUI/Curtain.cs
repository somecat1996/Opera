using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void callbackFun();
public delegate void callbackFun_Int(int _v);

public class Curtain : MonoBehaviour
{
    public static Curtain instance;

    // 无参回调函数
    callbackFun callback_Open;
    callbackFun callback_Close;
    callbackFun callback_CloseAndOpen;

    // 设置当前幕布是否可交互
    public bool activatable = false;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActivatable(bool _v)
    {
        activatable = _v;
    }

    /// <summary>
    /// 拉开幕布 由GUI执行
    /// </summary>
    public void OpenCurtain()
    {
        if (activatable)
        {
            GUIManager.instance.SetDisplayCurtain(false);
            GameManager.instance.SetPauseGame(false);
        }
    }

    public void DisableGameobject()
    {
        gameObject.SetActive(false);
    }

    // 调用回调函数
    public void Callback_Close()
    {
        if (callback_Close != null)
            callback_Close();
        //activatable = true;
    }

    public void Callback_Open()
    {
        if (callback_Open != null)
            callback_Open();
        //activatable = true;
    }

    public void Callback_CloseAndOpen()
    {
        if (callback_CloseAndOpen != null)
            callback_CloseAndOpen();
        //activatable = true;
    }

    /// <summary>
    /// 设置完全拉上幕布后的回调函数
    /// </summary>
    /// <param name="_fun"></param>
    public void SetCallbackFun_Close(callbackFun _fun)
    {
        callback_Close = _fun;
        activatable = false;
    }

    /// <summary>
    /// 设置完全拉开幕布后的回调函数
    /// </summary>
    /// <param name="_fun"></param>
    public void SetCallbackFun_Open(callbackFun _fun)
    {
        callback_Open = _fun;
        activatable = false;
    }

    /// <summary>
    /// 设置往返拉开幕布的回调函数
    /// </summary>
    /// <param name="_fun"></param>
    public void SetCallbackFun_CloseAndOpen(callbackFun _fun)
    {
        callback_CloseAndOpen = _fun;
        activatable = false;
    }
}
