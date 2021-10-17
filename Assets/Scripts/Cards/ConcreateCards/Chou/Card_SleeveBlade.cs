using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_SleeveBlade : CardPrototype,ICardOperation,ICardEffectTrigger
{
    public float angle = 30;

    public void mouseDrag()
    {
        transform.position = Input.mousePosition;

        SetFadeOutAndShowDirectionPointer(true);
    }

    public void mouseEnter()
    {
        SetOnSelected(true);
    }

    public void mouseExit()
    {
        // 当未检测到目标或因其他原因失效时 返回位置
        CardManager.instance.ReflashLayoutGroup();
        SetOnSelected(false);
    }

    public void mouseDown()
    {
        GUIManager.instance.DisableCardDesc();
    }

    public void mouseUp()
    {
        if (CheckAvaliablity())
        {
            GUIManager.instance.SpawnSystemText("无法释放卡牌!");
            mouseExit();
            return;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (CheckOnValidArea() && Physics.Raycast(ray, out hit, 1000, CardManager.instance.groundLayer))
        {
            if (PlayerManager.instance.ChangePowerPoint(-cardInfo.cost))
            {
                Vector3 origin = PlayerManager.instance.player.transform.position;
                origin.y = 0;

                Vector3 dir = hit.point - origin;

                Vector3 dir_Left = dir;
                Vector3 dir_Right = dir;

                float trueAngle = angle / 180;

                dir_Left.x = dir.x * Mathf.Cos(-trueAngle) + dir.z * Mathf.Sin(-trueAngle);
                dir_Left.z = -dir.x * Mathf.Sin(-trueAngle) + dir.z * Mathf.Cos(-trueAngle);
                dir_Right.x = dir.x * Mathf.Cos(trueAngle) + dir.z * Mathf.Sin(trueAngle);
                dir_Right.z = -dir.x * Mathf.Sin(trueAngle) + dir.z * Mathf.Cos(trueAngle);

                Debug.Log(dir_Left);
                Debug.Log(dir);
                Debug.Log(dir_Right);

                SummonedObjectManager.instance.SummonKnife(GlobalValue.GetTruePhysicsDamage_ToEnemy(cardInfo.mainValue_Cur), origin, dir);
                SummonedObjectManager.instance.SummonKnife(GlobalValue.GetTruePhysicsDamage_ToEnemy(cardInfo.mainValue_Cur), origin, dir_Left);
                SummonedObjectManager.instance.SummonKnife(GlobalValue.GetTruePhysicsDamage_ToEnemy(cardInfo.mainValue_Cur), origin, dir_Right);
                CardManager.instance.SendToDiscardedCardGroup(gameObject);
                TriggerEffect();
            }
            else
            {
                mouseExit();
            }
        }
        mouseExit();
    }

    public void RevokeEffect()
    {
        throw new System.NotImplementedException();
    }

    public void TriggerEffect()
    {
        
    }

    public void TriggerEffect(GameObjectBase _go)
    {
        throw new System.NotImplementedException();
    }

    public void TriggerEffect(GameObjectBase[] _gos)
    {

    }
}
