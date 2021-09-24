using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardEffectTrigger
{
    public void TriggerEffect();// 无参主要用于更改玩家状态

    /*
    public void TriggerEffect(); 用于获取单数对象
    public void TriggerEffect(); 用于获取复数对象
    */

    public void RevokeEffect(); // 效果回收 用于玩家
    /*
    public void RevokeEffect(); 用于获取单数对象
    public void RevokeEffect(); 用于获取复数对象
    */
}
