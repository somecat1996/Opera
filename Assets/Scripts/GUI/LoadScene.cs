using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/// <summary>
/// ���س��� ���� �ű�����ǰ����
/// </summary>
public class LoadScene : MonoBehaviour
{
    public Slider slider;          //������
    int currentProgress; //��ǰ����
    int targetProgress;  //Ŀ�����


    private void Start()
    {
        currentProgress = 0;
        targetProgress = 0;
        StartCoroutine(LoadingScene()); //����Э��
    }


    /// <summary>
    /// �첽���س���
    /// </summary>
    /// <returns>Э��</returns>
    private IEnumerator LoadingScene()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1); //�첽����1�ų���
        asyncOperation.allowSceneActivation = false;                          //����������������//�첽������ allowSceneActivation= falseʱ���Ῠ��0.89999��һ��ֵ���������100ת����
        while (asyncOperation.progress < 0.9f)                                //���첽����С��0.9f��ʱ��
        {
            targetProgress = (int)(asyncOperation.progress * 100); //�첽������ allowSceneActivation= falseʱ���Ῠ��0.89999��һ��ֵ���������100ת����
            yield return LoadProgress();
        }
        targetProgress = 100; //ѭ���󣬵�ǰ�����Ѿ�Ϊ90�ˣ�������Ҫ����Ŀ����ȵ�100������ѭ��
        yield return LoadProgress();
        asyncOperation.allowSceneActivation = true; //������ϣ����Ｄ��� ���� ��ת�����ɹ�
    }


    /// <summary>
    /// ������Ҫ���ε��ã���������м򵥷�װ
    /// </summary>
    /// <returns>��һ֡</returns>
    private IEnumerator<WaitForEndOfFrame> LoadProgress()
    {
        while (currentProgress < targetProgress) //��ǰ���� < Ŀ�����ʱ
        {
            ++currentProgress;                            //��ǰ���Ȳ����ۼ� ��Chinar��ܰ��ʾ�����������С�����Ե��������ֵ ���磺+=10 +=20�������ڼ����ٶȣ�
            slider.value = (float)currentProgress / 100; //��UI��������ֵ
            yield return new WaitForEndOfFrame();         //��һ֡
        }
    }
}