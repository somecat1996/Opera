using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPrototype : MonoBehaviour
{
    public CardBasicInfomation cardInfo;

    public int GetID()
    {
        return cardInfo.id;
    }
}
