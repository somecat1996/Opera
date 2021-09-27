using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CardCreator : EditorWindow
{
    int id;
    string cardName;
    CharacterType.Character belongner;

    [MenuItem("Window/Card Creator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CardCreator));
    }

    private void OnGUI()
    {
        GUILayout.Label("Card Infomation Configuration");
        id = EditorGUILayout.IntField("Card ID",id);
        cardName = EditorGUILayout.TextField("Card Name",cardName);
        belongner = (CharacterType.Character)EditorGUILayout.EnumPopup("Belonger",belongner);

        if (GUILayout.Button("Create"))
        {
            CreateCardInfo();
        }
    }

    public void CreateCardInfo()
    {

        CardBasicInfomation cardInfoTemplate = (CardBasicInfomation)CreateInstance("CardBasicInfomation");
        cardInfoTemplate.id = id;
        cardInfoTemplate.cardName = cardName;
        cardInfoTemplate.belonger = belongner;

        string cardInfo_Path = "Assets/Resources/CardInfomation/" + belongner.ToString() + "/" + id + "_" + cardName + ".asset";
        CardBasicInfomation cardInfo = Object.Instantiate<CardBasicInfomation>(cardInfoTemplate);
        AssetDatabase.CreateAsset(cardInfo, cardInfo_Path);
        AssetDatabase.Refresh();
    }
}
