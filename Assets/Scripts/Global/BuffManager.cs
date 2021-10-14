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
    public List<int> selectedBuff = new List<int>(); // ����Ѿ�ѡ���BUFF��ID

    private void Awake()
    {
        instance = this;

        LoadAllBuffInstances();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    List<int> test = new List<int>();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ReflashAllBuffData();
        }else if (Input.GetKeyDown(KeyCode.S))
        {
            int index = deactivateBuffList[Random.Range(0, deactivateBuffList.Count)];
            EnableBuff(index);
            test.Add(index);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            DisableBuff(test[0]);
            test.RemoveAt(0);
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

    /// <summary>
    ///  ���õ���BuffЧ��
    /// </summary>
    /// <param name="_id">BUFF ID</param>
    public void EnableBuff(int _id)
    {
        if (buffLibrary.ContainsKey(_id))
        {
            GameObject go = Instantiate(buffLibrary[_id]);
            go.transform.parent = transform;
            activiatedBuffList.Add(_id, go);

            deactivateBuffList.Remove(_id);

            // ��GUI����ʾBuff
            if (!go.GetComponent<BuffPrototype>().buffInfo.hideInGUI)
                go.GetComponent<BuffPrototype>().buffGUIicon = GUIManager.instance.SpawnBuffIcon(go.GetComponent<BuffPrototype>());
        }
    }

    // �ر�BuffЧ��
    public void DisableBuff(int _id)
    {
        if (activiatedBuffList.ContainsKey(_id))
        {
            Destroy(activiatedBuffList[_id].GetComponent<BuffPrototype>().buffGUIicon);
            Destroy(activiatedBuffList[_id]);
            activiatedBuffList.Remove(_id);

            deactivateBuffList.Add(_id);
        }
    }

    /// <summary>
    /// �������������ѡ���BUFF
    /// </summary>
    public void EnableAllSelectedBuff()
    {
        foreach(var i in selectedBuff)
        {
            EnableBuff(i);
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

    /// <summary>
    /// �Ƴ���ѡ���BUFF ID
    /// </summary>
    /// <param name="_id"></param>
    public void RemoveSelectedBuff(int _id)
    {
        if (selectedBuff.Contains(_id))
            selectedBuff.Remove(_id);
    }
    /// <summary>
    /// ѡ��BUFF ����ID�����б�
    /// </summary>
    /// <param name="_id"></param>
    public void SelectBuff(int _id)
    {
        if (!selectedBuff.Contains(_id))
            selectedBuff.Add(_id);
    }
    /// <summary>
    /// ���������ѡ���BUFF
    /// </summary>
    public void ClearAllSelectedBuff()
    {
        selectedBuff.Clear();
    }

    /// <summary>
    /// ��������׶����޵ĸ���
    /// </summary>
    /// <returns></returns>
    public float GetProbability_SpawnVoodoo()
    {
        if (!activiatedBuffList.ContainsKey(303))
        {
            return 0;
        }
        else
        {
            return activiatedBuffList[303].GetComponent<BuffPrototype>().GetTrueMainValue();
        }
    }
}
