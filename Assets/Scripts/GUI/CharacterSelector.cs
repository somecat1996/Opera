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
    // Start is called before the first frame update

    private void Awake()
    {
        foreach (var i in Resources.LoadAll<CharacterBasicInfomation>("CharacterInfomation"))
        {
            charInfo.Add(i.id, i);
        }
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        foreach(var i in charInfo)
        {
            if(i.Value.charTag == PlayerManager.instance.cur_Character)
            {
                Debug.Log(selectedId);
                selectedId = i.Key;
                break;
            }
        }
        DisplayCharInfo(selectedId);

        ConfirmCharacter();
    }

    public void DisplayCharInfo(int _id)
    {
        if (!charInfo.ContainsKey(_id))
            return;

        charName.text = charInfo[_id].charName;
        charDesc.text = charInfo[_id].description;
        charStory.text = charInfo[_id].story;

        selectedId = _id;
    }

    public void ConfirmCharacter()
    {

        charIcon.sprite = charInfo[selectedId].icon;
        PlayerManager.instance.SwitchCharacter(charInfo[selectedId].charTag);
    }
}
