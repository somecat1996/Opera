using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class CardCreator : EditorWindow
{
    int id;
    string cardName;
    string description;
    string story;
    Sprite illustration;

    int rarity = 1;
    CharacterType.CharacterTag belongner;

    public float mainValue;
    public int cost;
    public float duration;
    public float radius;

    [MenuItem("Window/Card Creator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CardCreator));
    }

    private void OnGUI()
    {
        GUI.skin.label.fontStyle = FontStyle.Bold;
        GUILayout.Space(5);
        GUILayout.Label("注意:\n1.EventTrigger需要手动配置事件\n2.脚本需要手动配置\n3.背景暂时需要手动配置");
        GUILayout.Space(5);
        GUILayout.Label("卡牌基本信息");
        id = EditorGUILayout.IntField("Card ID",id);
        cardName = EditorGUILayout.TextField("Card Name",cardName);

        description = EditorGUILayout.TextField("Description", description);
        story = EditorGUILayout.TextField("Story", story);

        GUILayout.Space(5);
        illustration = EditorGUILayout.ObjectField("Illustration", illustration,typeof(Sprite),true) as Sprite;

        GUILayout.Label("1-基础 2-稀有 3-实施 4-传说");
        rarity = EditorGUILayout.IntSlider("Rarity", rarity, 1, 4);
        belongner = (CharacterType.CharacterTag)EditorGUILayout.EnumPopup("Belonger", belongner);

        mainValue = EditorGUILayout.FloatField("Main Value", mainValue);
        cost = EditorGUILayout.IntField("Cost", cost);
        duration = EditorGUILayout.FloatField("Duration", duration);
        radius = EditorGUILayout.FloatField("Radius", radius);

        GUILayout.Space(5);
        if (GUILayout.Button("Create"))
        {
            CreateCardInfo();
            CreateCardScript();
            CreateCardObject();
        }
    }

    public void CreateCardInfo()
    {
        // 生成脚本信息
        CardBasicInfomation cardInfoTemplate = (CardBasicInfomation)CreateInstance("CardBasicInfomation");  

        cardInfoTemplate.id = id;
        cardInfoTemplate.cardName = cardName;
        cardInfoTemplate.belongner = belongner;
        cardInfoTemplate.description = description;
        cardInfoTemplate.story = story;
        cardInfoTemplate.rarity = rarity;
        cardInfoTemplate.mainValue_Origin = mainValue;
        cardInfoTemplate.cost = cost;
        cardInfoTemplate.duration = duration;
        cardInfoTemplate.radius = radius;
        cardInfoTemplate.illustration = illustration;

        string cardInfo_Path = "Assets/Resources/CardInfomation/" + belongner.ToString() + "/" + id + "_" + cardName + ".asset";
        CardBasicInfomation cardInfo = Object.Instantiate<CardBasicInfomation>(cardInfoTemplate);
        AssetDatabase.CreateAsset(cardInfo, cardInfo_Path);
        AssetDatabase.Refresh();
    }
    
    // 创建具体卡牌脚本
    public void CreateCardScript()
    {
        var templateSource = EditorGUIUtility.Load("ConcreateCardTemplate.bytes") as TextAsset;

        string codeSrc = templateSource.text;

        codeSrc = templateSource.text.Replace("#ClassName", "Card_" + cardName);

        var assetPath = string.Format("Assets/Scripts/Cards/ConcreateCards/" + belongner.ToString()+ "/Card_" +cardName + ".cs");
        var filePath = Application.dataPath.Replace("Assets", assetPath);

        File.WriteAllText(filePath, codeSrc);
        AssetDatabase.ImportAsset(assetPath);
    }

    // 创建卡牌游戏对象
    public void CreateCardObject()
    {
        GameObject prefab = Resources.Load("Prefabs/CardObjectTemplate") as GameObject;
        GameObject go = GameObject.Instantiate(prefab);
        go.name = "Card_" + cardName;
        var type = System.Reflection.Assembly.Load("Assembly-CSharp").GetType("Card_" + cardName);
        var script = go.AddComponent(type);
        string path = "Assets/Resources/CardInstances/" + belongner.ToString() + "/Card_" + cardName + ".prefab"; ;
        PrefabUtility.SaveAsPrefabAsset(go, path);

        Destroy(go);
    }

    void InvokeMouseUp(BaseEventData arg0)
    {

    }
}
