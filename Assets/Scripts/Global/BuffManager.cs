using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    [HideInInspector]
    public static BuffManager instance;

    public Dictionary<int, BuffBasicInfomation> buffLibrary = new Dictionary<int, BuffBasicInfomation>();
    public Dictionary<int, GameObject> buffInstanceLibrary = new Dictionary<int, GameObject>(); // BUFFʵ��⡪����������

    public List<int> deactivateBuffList = new List<int>(); // δ����BUFF��ID��
    public Dictionary<int, GameObject> activiatedBuffList = new Dictionary<int, GameObject>(); // ����Ѿ������Buff��Ϣ
    public List<int> selectedBuff = new List<int>(); // ����Ѿ�ѡ���BUFF��ID

    private void Awake()
    {
        instance = this;

        // ����buff��Ϣ
        LoadAllBuffInfo();
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

    /// <summary>
    /// ��������Buff��Ϣ
    /// </summary>
    public void LoadAllBuffInfo()
    {
        foreach (var i in Resources.LoadAll<BuffBasicInfomation>("BuffBasicInfomation"))
        {
             buffLibrary.Add(i.id, i);
        }
    }

    /// <summary>
    ///  ��������Buffʵ�嵽����
    /// </summary>
    public void LoadAllBuffInstances()
    {
        foreach(var i in Resources.LoadAll<GameObject>("BuffInstances"))
        {
            buffInstanceLibrary.Add(i.GetComponent<BuffPrototype>().GetID(), i);
            deactivateBuffList.Add(i.GetComponent<BuffPrototype>().GetID());

            i.GetComponent<BuffPrototype>().ReflashData(); // ˢ������
        }
    }

    // ˢ������δ����buff����Ϣ���ݡ���*�ڸ�GUI�����������ǰ����*
    public void ReflashAllBuffData()
    {
        foreach(var i in deactivateBuffList)
        {
            buffInstanceLibrary[i].GetComponent<BuffPrototype>().ReflashData();
        }
    }

    /// <summary>
    ///  ���õ���BuffЧ��
    /// </summary>
    /// <param name="_id">BUFF ID</param>
    public void EnableBuff(int _id)
    {
        if (buffInstanceLibrary.ContainsKey(_id))
        {
            GameObject go = Instantiate(buffInstanceLibrary[_id]);
            go.transform.parent = transform;
            activiatedBuffList.Add(_id, go);

            deactivateBuffList.Remove(_id);

            // ��GUI����ʾBuff
            if (!go.GetComponent<BuffPrototype>().buffInfo.hideInGUI)
            {
                go.GetComponent<BuffPrototype>().buffGUIicon = GUIManager.instance.SpawnBuffIcon(go.GetComponent<BuffPrototype>());
                go.GetComponent<BuffPrototype>().buffGUIScript = go.GetComponent<BuffPrototype>().buffGUIicon.GetComponent<BuffInfoDisplayer>();
            }
                
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
        List<int> keyList = new List<int>();
        foreach(var i in activiatedBuffList)
        {
            keyList.Add(i.Key);
        }

        for (int i = 0; i < keyList.Count; i++)
        {
            DisableBuff(keyList[i]);
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

    /// <summary>
    /// ������δ����Buff��Ϣ
    /// </summary>
    public List<BuffBasicInfomation> GetBuffInfoRandomly(int _count)
    {
        // ˢ�¿�������
        ReflashAllBuffData();

        List<BuffBasicInfomation> tempBuffInfoList = new List<BuffBasicInfomation>();
        List<int> idList = new List<int>();

        // �Ƴ���ɫ��ר��buff
        for(int i = 0; i < deactivateBuffList.Count; i++)
        {
            if(deactivateBuffList[i] < 300)
            {
                idList.Add(deactivateBuffList[i]);
            }
        }

        while(idList.Count > _count)
        {
            idList.RemoveAt(Random.Range(0, idList.Count));
        }

        foreach(var i in idList)
        {
            tempBuffInfoList.Add(buffLibrary[i]);
        }

        return tempBuffInfoList;
    }
}
