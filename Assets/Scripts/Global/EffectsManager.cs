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
    ///  创建一个固定的特效
    /// </summary>
    /// <param name="id">特效ID</param>
    /// <param name="_life">生存时间</param>
    /// <param name="position">位置</param>
    /// <param name="offset">对位置的偏移</param>
    public GameObject CreateEffect(int id, float _life, Vector3 position, Vector3 offset)
    {
        GameObject tmp = Instantiate(effects[id]);
        tmp.GetComponent<EffectBase>().Instantiate(_life, position, offset);
        return tmp;
    }

    /// <summary>
    ///  创建一个跟随目标的特效
    /// </summary>
    /// <param name="id">特效ID</param>
    /// <param name="_life">生存时间</param>
    /// <param name="_target">目标transform</param>
    /// <param name="offset">对位置的偏移</param>
    public GameObject CreateEffectFollow(int id, float _life, Transform _target, Vector3 _offset)
    {
        GameObject tmp = Instantiate(effects[id]);
        tmp.GetComponent<EffectFollow>().InstantiateFollow(_life, _target, _offset);
        return tmp;
    }

    /// <summary>
    ///  创建一个跟随玩家的特效
    /// </summary>
    /// <param name="id">特效ID</param>
    /// <param name="_life">生存时间</param>
    /// <param name="offset">对位置的偏移</param>
    public GameObject CreateEffectFollowPlayer(int id, float _life, Vector3 _offset)
    {
        GameObject tmp = Instantiate(effects[id]);
        tmp.GetComponent<EffectFollow>().InstantiateFollow(_life, PlayerManager.instance.player.transform, _offset);
        return tmp;
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
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            CreateEffect(19, 10f, Player.instance.transform.position, new Vector3(0, 0, 2));
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            CreateEffectFollowPlayer(17, 10f, new Vector3(0, 0, 2));
        }
    }
}
