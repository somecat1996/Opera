using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void callbackFun();
public delegate void callbackFun_Int(int _v);

public class Curtain : MonoBehaviour
{
    public static Curtain instance;

    // �޲λص�����
    callbackFun callback_Open;
    callbackFun callback_Close;
    callbackFun callback_CloseAndOpen;

    // ���õ�ǰĻ���Ƿ�ɽ���
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
    /// ����Ļ�� ��GUIִ��
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

    // ���ûص�����
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
    /// ������ȫ����Ļ����Ļص�����
    /// </summary>
    /// <param name="_fun"></param>
    public void SetCallbackFun_Close(callbackFun _fun)
    {
        callback_Close = _fun;
        activatable = false;
    }

    /// <summary>
    /// ������ȫ����Ļ����Ļص�����
    /// </summary>
    /// <param name="_fun"></param>
    public void SetCallbackFun_Open(callbackFun _fun)
    {
        callback_Open = _fun;
        activatable = false;
    }

    /// <summary>
    /// ������������Ļ���Ļص�����
    /// </summary>
    /// <param name="_fun"></param>
    public void SetCallbackFun_CloseAndOpen(callbackFun _fun)
    {
        callback_CloseAndOpen = _fun;
        activatable = false;
    }
}
