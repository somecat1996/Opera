using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardEffectTrigger
{
    public void TriggerEffect();// �޲���Ҫ���ڸ������״̬
    public void TriggerEffect(GameObjectBase _go); //���ڻ�ȡ��������
    public void TriggerEffect(GameObjectBase[] _gos); //���ڻ�ȡ��������

    public void RevokeEffect(); // Ч������ �������
    /*
    public void RevokeEffect(); ���ڻ�ȡ��������
    public void RevokeEffect(); ���ڻ�ȡ��������
    */
}
