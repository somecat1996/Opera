using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    [HideInInspector]
    public static BuffManager instance;

    public Dictionary<int, GameObject> buffLibrary = new Dictionary<int, GameObject>(); // BUFFʵ��⡪����������

    public List<int> deactivateBuffList = new List<int>(); // δ����BUFF��ID��
    public Dictionary<int, GameObject> activiatedBuffList = new Dictionary<int, GameObject>(); // ����Ѿ������Buff��Ϣ

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadAllBuffInstances();

      
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ReflashAllBuffData();
        }else if (Input.GetKeyDown(KeyCode.S))
        {
            EnableBuff(208);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            DisableBuff(208);
        }
    }

    // ��������Buffʵ�嵽����
    public void LoadAllBuffInstances()
    {
        foreach(var i in Resources.LoadAll<GameObject>("BuffInstances"))
        {
            buffLibrary.Add(i.GetComponent<BuffPrototype>().GetID(), i);
            deactivateBuffList.Add(i.GetComponent<BuffPrototype>().GetID());

            i.GetComponent<BuffPrototype>().ReflashData(); // ˢ������
        }
    }

    // ˢ������δ����buff����Ϣ���ݡ���*�ڸ�GUI�����������ǰ����*
    public void ReflashAllBuffData()
    {
        foreach(var i in deactivateBuffList)
        {
            buffLibrary[i].GetComponent<BuffPrototype>().ReflashData();
        }
    }

    // ����BuffЧ��
    public void EnableBuff(int _id)
    {
        if (buffLibrary.ContainsKey(_id))
        {
            GameObject go = Instantiate(buffLibrary[_id]);
            go.transform.parent = transform;
            activiatedBuffList.Add(_id, go);

            deactivateBuffList.Remove(_id);
        }
    }

    // �ر�BuffЧ��
    public void DisableBuff(int _id)
    {
        if (activiatedBuffList.ContainsKey(_id))
        {
            Destroy(activiatedBuffList[_id]);
            activiatedBuffList.Remove(_id);

            deactivateBuffList.Add(_id);
        }
    }

    // �ر�����Buff
    public void DiableAllBuff()
    {
        foreach(var i in activiatedBuffList.Keys)
        {
            DisableBuff(i);
        }
    }
}
