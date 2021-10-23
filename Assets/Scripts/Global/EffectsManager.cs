using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public bool test = false;
    public static EffectsManager instance;
    public List<GameObject> effects;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (test)
            Test();
    }


    /// <summary>
    ///  ����һ���̶�����Ч
    /// </summary>
    /// <param name="id">��ЧID</param>
    /// <param name="_life">����ʱ��</param>
    /// <param name="position">λ��</param>
    /// <param name="offset">��λ�õ�ƫ��</param>
    public void CreateEffect(int id, float _life, Vector3 position, Vector3 offset)
    {
        GameObject tmp = Instantiate(effects[id]);
        tmp.GetComponent<EffectBase>().Instantiate(_life, position, offset);
    }

    /// <summary>
    ///  ����һ������Ŀ�����Ч
    /// </summary>
    /// <param name="id">��ЧID</param>
    /// <param name="_life">����ʱ��</param>
    /// <param name="_target">Ŀ��transform</param>
    /// <param name="offset">��λ�õ�ƫ��</param>
    public void CreateEffectFollow(int id, float _life, Transform _target, Vector3 _offset)
    {
        GameObject tmp = Instantiate(effects[id]);
        tmp.GetComponent<EffectFollow>().InstantiateFollow(_life, _target, _offset);
    }

    /// <summary>
    ///  ����һ��������ҵ���Ч
    /// </summary>
    /// <param name="id">��ЧID</param>
    /// <param name="_life">����ʱ��</param>
    /// <param name="offset">��λ�õ�ƫ��</param>
    public void CreateEffectFollowPlayer(int id, float _life, Vector3 _offset)
    {
        GameObject tmp = Instantiate(effects[id]);
        tmp.GetComponent<EffectFollow>().InstantiateFollow(_life, PlayerManager.instance.player.transform, _offset);
    }

    private void Test()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            CreateEffectFollow(0, 3f, Player.instance.transform, Vector3.zero);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CreateEffectFollow(1, 3f, Player.instance.transform, Vector3.zero);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CreateEffectFollow(2, 3f, Player.instance.transform, Vector3.zero);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CreateEffectFollow(3, 10f, Player.instance.transform, Vector3.zero);
        }
    }
}
