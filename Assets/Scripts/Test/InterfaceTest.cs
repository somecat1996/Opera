using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.name);

                GameObjectBase healthManager = hit.transform.GetComponent<GameObjectBase>();
                if (healthManager)
                {
                    Debug.Log(healthManager);
                    healthManager.Poisoning();
                    // 使用完毕 卡牌扔回等待队列或者直接销毁
                    CardManager.instance.SendToDiscardedCardGroup(gameObject);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.name);

                GameObjectBase healthManager = hit.transform.GetComponent<GameObjectBase>();
                if (healthManager)
                {
                    Debug.Log(healthManager);
                    healthManager.InstantHealing(20);
                    // 使用完毕 卡牌扔回等待队列或者直接销毁
                    CardManager.instance.SendToDiscardedCardGroup(gameObject);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.name);

                GameObjectBase healthManager = hit.transform.GetComponent<GameObjectBase>();
                if (healthManager)
                {
                    Debug.Log(healthManager);
                    healthManager.AddShield(100, 60);
                    // 使用完毕 卡牌扔回等待队列或者直接销毁
                    CardManager.instance.SendToDiscardedCardGroup(gameObject);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.name);

                GameObjectBase healthManager = hit.transform.GetComponent<GameObjectBase>();
                if (healthManager)
                {
                    Debug.Log(healthManager);
                    healthManager.Hurt(50, false, 0);
                    // 使用完毕 卡牌扔回等待队列或者直接销毁
                    CardManager.instance.SendToDiscardedCardGroup(gameObject);
                }
            }
        }
    }
}
