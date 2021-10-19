using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    [HideInInspector]
    public static BuffManager instance;

    public Dictionary<int, BuffBasicInfomation> buffLibrary = new Dictionary<int, BuffBasicInfomation>();
    public Dictionary<int, GameObject> buffInstanceLibrary = new Dictionary<int, GameObject>(); // BUFF实体库——用于载入

    public List<int> deactivateBuffList = new List<int>(); // 未激活BUFF的ID库
    public Dictionary<int, GameObject> activiatedBuffList = new Dictionary<int, GameObject>(); // 存放已经激活的Buff信息
    public List<int> selectedBuff = new List<int>(); // 存放已经选择的BUFF的ID

    private void Awake()
    {
        instance = this;

        // 载入buff信息
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
    /// 载入所有Buff信息
    /// </summary>
    public void LoadAllBuffInfo()
    {
        foreach (var i in Resources.LoadAll<BuffBasicInfomation>("BuffBasicInfomation"))
        {
             buffLibrary.Add(i.id, i);
        }
    }

    /// <summary>
    ///  载入所有Buff实体到库中
    /// </summary>
    public void LoadAllBuffInstances()
    {
        foreach(var i in Resources.LoadAll<GameObject>("BuffInstances"))
        {
            buffInstanceLibrary.Add(i.GetComponent<BuffPrototype>().GetID(), i);
            deactivateBuffList.Add(i.GetComponent<BuffPrototype>().GetID());

            i.GetComponent<BuffPrototype>().ReflashData(); // 刷新数据
        }
    }

    // 刷新所有未激活buff的信息数据——*在给GUI发送随机抽样前调用*
    public void ReflashAllBuffData()
    {
        foreach(var i in deactivateBuffList)
        {
            buffInstanceLibrary[i].GetComponent<BuffPrototype>().ReflashData();
        }
    }

    /// <summary>
    ///  启用单个Buff效果
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

            // 在GUI上显示Buff
            if (!go.GetComponent<BuffPrototype>().buffInfo.hideInGUI)
            {
                go.GetComponent<BuffPrototype>().buffGUIicon = GUIManager.instance.SpawnBuffIcon(go.GetComponent<BuffPrototype>());
                go.GetComponent<BuffPrototype>().buffGUIScript = go.GetComponent<BuffPrototype>().buffGUIicon.GetComponent<BuffInfoDisplayer>();
            }
                
        }
    }

    // 关闭Buff效果
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
    /// 启用所有玩家所选择的BUFF
    /// </summary>
    public void EnableAllSelectedBuff()
    {
        foreach(var i in selectedBuff)
        {
            EnableBuff(i);
        }
    }

    // 关闭所有Buff
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
    /// 移除所选择的BUFF ID
    /// </summary>
    /// <param name="_id"></param>
    public void RemoveSelectedBuff(int _id)
    {
        if (selectedBuff.Contains(_id))
            selectedBuff.Remove(_id);
    }
    /// <summary>
    /// 选择BUFF 将其ID放入列表
    /// </summary>
    /// <param name="_id"></param>
    public void SelectBuff(int _id)
    {
        if (!selectedBuff.Contains(_id))
            selectedBuff.Add(_id);
    }
    /// <summary>
    /// 清除所有所选择的BUFF
    /// </summary>
    public void ClearAllSelectedBuff()
    {
        selectedBuff.Clear();
    }

    /// <summary>
    /// 获得生成巫毒娃娃的概率
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
    /// 随机获得未激活Buff信息
    /// </summary>
    public List<BuffBasicInfomation> GetBuffInfoRandomly(int _count)
    {
        // 刷新卡牌数据
        ReflashAllBuffData();

        List<BuffBasicInfomation> tempBuffInfoList = new List<BuffBasicInfomation>();
        List<int> idList = new List<int>();

        // 移除角色的专属buff
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
