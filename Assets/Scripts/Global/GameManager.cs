using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // �״����� ����
        if (PlayerManager.instance.data.firstTimeRunning)
            GUIManager.instance.LockButton_Continue(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // ��ʼ����������-�¿���Ϸʱ����
    public void InitializeAllData()
    {
        PlayerManager.instance.data.firstTimeRunning = false;

        PlayerManager.instance.InitializeData(); // ��ʼ���������
        CardManager.instance.InitializeAllCards();
    }

    // ������ҺͿ�������
    public void LoadData()
    {
        CardManager.instance.LoadCardInstance(); // �������п���ʵ��
        CardManager.instance.LoadCardLibrary(); // �����п�����Ϣ���뵽��Ϸ��
        BuffManager.instance.LoadAllBuffInstances(); // ����BUFFʵ��
    }

}
