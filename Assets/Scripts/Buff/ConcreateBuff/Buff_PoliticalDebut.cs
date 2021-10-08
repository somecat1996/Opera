using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Buff_PoliticalDebut : BuffPrototype
{
    [System.Serializable]
    public struct KVPair
    {
        public float probability;
        public float increment;
    }


    private int counter_Origin = 13;
    public int counter;

    public float decrement = 1f;

    public List<KVPair> probabilityDict = new List<KVPair>();

    public CardPrototype lastUsesdCard = null;

    private void Update()
    {
        // ��⵽ʹ�ÿ������Ѹ���
        if (lastUsesdCard != BattleDataManager.instance.lastUsedCard)
        {
            lastUsesdCard = BattleDataManager.instance.lastUsedCard;
            counter -= lastUsesdCard.cardInfo.cost;

            // �������ԴﵽԤ��ֵ
            if (counter == 0)
            {
                // ��λ
                counter = counter_Origin;
                activated = true;

                float random = Random.Range(0, 1f);

                if(random >= 0 && random < (probabilityDict[0].probability - 3*GlobalValue.probabilityIncrement_Event))
                {
                    PlayerManager.instance.ChangePowerPoint(probabilityDict[0].increment);
                    Debug.Log("�ظ� " + probabilityDict[0].increment);
                }
                else if(random >= (probabilityDict[0].probability - 3 * GlobalValue.probabilityIncrement_Event) && random < probabilityDict[1].probability + GlobalValue.probabilityIncrement_Event)
                {
                    PlayerManager.instance.ChangePowerPoint(probabilityDict[1].increment);
                    Debug.Log("�ظ� " + probabilityDict[1].increment);
                }
                else if (random >= probabilityDict[1].probability + GlobalValue.probabilityIncrement_Event && random < probabilityDict[2].probability + GlobalValue.probabilityIncrement_Event)
                {
                    PlayerManager.instance.ChangePowerPoint(probabilityDict[2].increment);
                    Debug.Log("�ظ� " + probabilityDict[2].increment);
                }
                else if (random >= probabilityDict[2].probability + GlobalValue.probabilityIncrement_Event && random < probabilityDict[3].probability + GlobalValue.probabilityIncrement_Event)
                {
                    PlayerManager.instance.ChangePowerPoint(probabilityDict[3].increment);
                    Debug.Log("�ظ� " + probabilityDict[3].increment);
                }
            }
            else if(counter < 0)
            {
                counter = counter_Origin;
            }

        }
    }

    private void OnEnable()
    {
        if (!activated)
        {
            activated = true;
            counter = counter_Origin;
            lastUsesdCard = BattleDataManager.instance.lastUsedCard;
        }
    }

    private void OnDisable()
    {
        if (activated)
        {
            activated = false;
        }
    }
}
