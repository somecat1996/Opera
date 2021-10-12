using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    [Header("Real-Time Data")]
    public int selectedId = 1;

    [Header("Objects")]
    public TextMeshProUGUI charName;
    public TextMeshProUGUI charDesc;
    public TextMeshProUGUI charStory;
    [Space]
    public Image charIcon;

    private Dictionary<int,CharacterBasicInfomation> charInfo = new Dictionary<int, CharacterBasicInfomation>();
    public List<GameObject> selectedCharTag = new List<GameObject>();
    // Start is called before the first frame update

    private void Awake()
    {
        foreach (var i in Resources.LoadAll<CharacterBasicInfomation>("CharacterInfomation"))
        {
            charInfo.Add(i.id, i);
        }
    }

    private void Start()
    {
        selectedId = ((int)PlayerManager.instance.cur_Character); // ѡȡĬ��ֵ

        DisplayCharInfo(selectedId);
        DisplayTag(selectedId);
        ConfirmCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        // δ������ɫʱ ��������
        foreach(var i in charInfo)
        {
            if(i.Value.charTag == PlayerManager.instance.cur_Character)
            {
                selectedId = i.Key;
                break;
            }
        }

        DisplayCharInfo(selectedId);
        DisplayTag(selectedId);
        ConfirmCharacter();
    }

    /// <summary>
    /// ��ʾѡ�н�ɫ����Ϣ
    /// </summary>
    /// <param name="_id"></param>
    public void DisplayCharInfo(int _id)
    {
        if (!charInfo.ContainsKey(_id))
            return;

        charName.text = charInfo[_id].charName;
        charStory.text = charInfo[_id].story;
        DisplayBuffInfo();

        selectedId = _id;
    }

    /// <summary>
    /// ��ʾ��ѡ��ɫ�ı���Ч��
    /// </summary>
    /// <param name="_index">�������±�</param>
    public void DisplayBuffInfo(int _index = 0)
    {
        charDesc.text = BuffManager.instance.buffLibrary[charInfo[selectedId].buffID[_index]].GetComponent<BuffPrototype>().buffInfo.description;
    }

    /// <summary>
    /// ȷ��ѡ����Ϣ
    /// </summary>
    public void ConfirmCharacter()
    {
        charIcon.sprite = charInfo[selectedId].icon;
        PlayerManager.instance.SwitchCharacter(charInfo[selectedId].charTag,charInfo[selectedId]);
    }

    // ��ʾѡ�н�ɫ�ı�ǩ
    public void DisplayTag(int _id)
    {
        foreach(var i in selectedCharTag)
        {
            i.SetActive(false);
        }

        selectedCharTag[_id].SetActive(true);
    }

    public void ReflashButton()
    {

    }
}
