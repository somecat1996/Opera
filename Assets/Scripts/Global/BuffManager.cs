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
        }
    }

    // 关闭Buff效果
    public void DisableBuff(int _id)
    {
        if (activiatedBuffList.ContainsKey(_id))
        {
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
}
