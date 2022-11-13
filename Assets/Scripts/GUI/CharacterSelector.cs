using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    [Header("Real-Time Data")]
    public int selectedId = 1;
    public int selectedBuffIndex = 0;

    [Header("Objects")]
    public TextMeshProUGUI charName;
    public TextMeshProUGUI charDesc;
    public TextMeshProUGUI charStory;
    [Space]
    public Image characterIcon;
    [Space]
    public Image[] image_BuffSelector = new Image[2];
    public Image[] image_BuffIcon = new Image[2];
    public TextMeshProUGUI[] text_BuffName = new TextMeshProUGUI[2];
    public Sprite sprite_SelectedBuff;
    public Sprite sprite_UnselectedBuff;

    private Dictionary<int,CharacterBasicInfomation> charInfo = new Dictionary<int, CharacterBasicInfomation>();
    public List<GameObject> selectedCharTag = new List<GameObject>();
    // Start is called before the first frame update

    private void Awake()
    {
        foreach (var i in Resources.LoadAll<CharacterBasicInfomation>("CharacterInfomation"))
        {
            charInfo.Add(i.id, i);
        }

        selectedId = ((int)PlayerManager.instance.cur_Character); // ѡȡĬ��ֵ
    }

    private void Start()
    {
        
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
        DisplayBuffInfo(selectedBuffIndex);
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
        selectedId = _id;
        characterIcon.sprite = charInfo[selectedId].icon;
    }

    /// <summary>
    /// ��ʾ��ѡ��ɫ�ı���Ч��
    /// </summary>
    /// <param name="_index">�������±�</param>
    public void DisplayBuffInfo(int _index = 1)
    {
        int buffID = charInfo[selectedId].buffID[_index];
        selectedBuffIndex = _index;
        charDesc.text = BuffManager.instance.buffInstanceLibrary[buffID].GetComponent<BuffPrototype>().buffInfo.description;

        for(int i = 0; i < image_BuffSelector.Length; i++)
        {
            if(i == selectedBuffIndex)
            {
                image_BuffSelector[i].sprite = sprite_SelectedBuff;
            }
            else
            {
                image_BuffSelector[i].sprite = sprite_UnselectedBuff;
            }
        }

        for (int i = 0; i < text_BuffName.Length; i++)
        {
            text_BuffName[i].text = BuffManager.instance.buffInstanceLibrary[charInfo[selectedId].buffID[i]].GetComponent<BuffPrototype>().buffInfo.buffName;
            image_BuffIcon[i].sprite = BuffManager.instance.buffInstanceLibrary[charInfo[selectedId].buffID[i]].GetComponent<BuffPrototype>().buffInfo.icon;
        }
    }

    /// <summary>
    /// ȷ��ѡ����Ϣ
    /// </summary>
    public void ConfirmCharacter()
    {
        Debug.Log(selectedId);
        characterIcon.sprite = charInfo[selectedId].icon;

        if(charInfo[selectedId].charTag != PlayerManager.instance.cur_Character)
            GUIManager.instance.SpawnSystemText("�ɹ��л�����ɫ \"" + charInfo[selectedId].charName+"\"");
        else if(PlayerManager.instance.cur_CharBuffID != charInfo[selectedId].buffID[selectedBuffIndex])
            GUIManager.instance.SpawnSystemText("�ɹ��л���������");

        PlayerManager.instance.SwitchCharacter(charInfo[selectedId].charTag, charInfo[selectedId], selectedBuffIndex);
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
