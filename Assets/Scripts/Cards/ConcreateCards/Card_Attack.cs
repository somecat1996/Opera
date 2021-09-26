using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 具体卡牌样例
// 使用卡牌操作接口以及效果实现接口且声明卡牌信息类
public class Card_Attack : MonoBehaviour, ICardOperation,ICardEffectTrigger
{
    // ***卡牌信息动态读取***
    // 每当新开游戏时 CardManager读取所有卡牌基本信息 将所有卡牌等级设为0 再将特定卡牌等级设为1
    // 若有存档则Card读取所有卡牌信息且根据存档一一修改对应等级
    public CardBasicInfomation cardInfo;

    // 每张卡具体造成效果数值放在具体类上

    private void Awake()
    {
        
    }

    private void Start()
    {

    }

    public void mouseDrag()
    {
        transform.position = Input.mousePosition;
    }

    public void mouseEnter()
    {
        Vector3 scale = new Vector3(1.2f, 1.2f, 1.2f);
        transform.localScale = scale;
    }

    // 松开卡牌时调用操作
    // 根据卡牌不同检测场景物体方式不同
    public void mouseUp()
    {
        // 消耗心流值
        /*
            if(xx - cardInfo.cost)
                ...
         */

        // 此处暂时将TriggerEffect函数内容调用至此
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.transform.name);

            HealthManager healthManager = hit.transform.GetComponent<HealthManager>();
            if (healthManager)
            {
                Debug.Log(healthManager);
                healthManager.Hurt(5);
                // 使用完毕 卡牌扔回等待队列或者直接销毁
                CardManager.instance.SendToDiscardedCardGroup(gameObject);
            }

            // 场景2 测试使用
            Enemy e = hit.transform.GetComponent<Enemy>();
            if (e)
            {
                e.cur_healthPoint -= 5;
                CardManager.instance.SendToDiscardedCardGroup(gameObject);
            }
        }

        // 当未检测到目标或因其他原因失效时 返回位置
        CardManager.instance.ReflashLayoutGroup();
        Vector3 scale = Vector3.one;
        transform.localScale = scale;
    }

    public void mouseExit()
    {
        // 当未检测到目标或因其他原因失效时 返回位置
        CardManager.instance.ReflashLayoutGroup();
        Vector3 scale = Vector3.one;
        transform.localScale = scale;
    }

    public void RevokeEffect()
    {
        throw new System.NotImplementedException();
    }

    public void TriggerEffect()
    {
        throw new System.NotImplementedException();
    }


}
