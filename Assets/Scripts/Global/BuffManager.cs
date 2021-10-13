using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    [HideInInspector]
    public static BuffManager instance;

    public Dictionary<int, GameObject> buffLibrary = new Dictionary<int, GameObject>(); // BUFF实体库——用于载入

    public List<int> deactivateBuffList = new List<int>(); // 未激活BUFF的ID库
    public Dictionary<int, GameObject> activiatedBuffList = new Dictionary<int, GameObject>(); // 存放已经激活的Buff信息

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

    // 载入所有Buff实体到库中
    public void LoadAllBuffInstances()
    {
        foreach(var i in Resources.LoadAll<GameObject>("BuffInstances"))
        {
            buffLibrary.Add(i.GetComponent<BuffPrototype>().GetID(), i);
            deactivateBuffList.Add(i.GetComponent<BuffPrototype>().GetID());

            i.GetComponent<BuffPrototype>().ReflashData(); // 刷新数据
        }
    }

    // 刷新所有未激活buff的信息数据——*在给GUI发送随机抽样前调用*
    public void ReflashAllBuffData()
    {
        foreach(var i in deactivateBuffList)
        {
            buffLibrary[i].GetComponent<BuffPrototype>().ReflashData();
        }
    }

    // 启用Buff效果
    public void EnableBuff(int _id)
    {
        if (buffLibrary.ContainsKey(_id))
        {
            GameObject go = Instantiate(buffLibrary[_id]);
            go.transform.parent = transform;
            activiatedBuffList.Add(_id, go);

            deactivateBuffList.Remove(_id);

            // 在GUI上显示Buff
            if (!go.GetComponent<BuffPrototype>().buffInfo.hideInGUI)
                go.GetComponent<BuffPrototype>().buffGUIicon = GUIManager.instance.SpawnBuffIcon(go.GetComponent<BuffPrototype>());
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

    // 关闭所有Buff
    public void DiableAllBuff()
    {
        foreach(var i in activiatedBuffList.Keys)
        {
            DisableBuff(i);
        }
    }

    // 启用角色Buff
    public void EnableCharacterBuff(CharacterBasicInfomation _info)
    {
        foreach(var i in _info.buffID)
        {
            EnableBuff(i);
        }
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
}
