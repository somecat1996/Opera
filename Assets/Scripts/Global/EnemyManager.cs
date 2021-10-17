using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    // ��������λ�ã�3*4���󣬴��ϵ��´���������
    // 9  6  3  0
    // 10 7  4  1
    // 11 8  5  2
    public List<Transform> generationPoint;
    // ��ͼ����λ��
    public Transform middleGenerationPoint;
    // ���Ե�����
    public GameObject enemyPrefab;

    // ��¼״̬
    private EnemyStatus [] generationPointStatus;
    private LevelItemInterface itemInterface;

    // �ؿ�����
    public List<GameObject> bossPrefabs;

    private PlayerStatus playerStatus;

    public Image background;
    public List<Sprite> backgroundImages;
    public MeshRenderer ground;
    public List<Material> groundMaterials;

    public bool pause;
    private void Awake()
    {
        instance = this;
        pause = true;
        itemInterface = null;
    }
    // Start is called before the first frame update
    void Start()
    {
        generationPointStatus = new EnemyStatus[generationPoint.Count];

        playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();

        // ������
        // EnterLevel(0);
    }

    // Update is called once per frame
    void Update()
    {
        // ����
        if (Input.GetKeyDown(KeyCode.Space))
            PlayerManager.instance.EnterLevel(0);
    }

    public void EnterLevel(int bossIndex)
    {
        /// test
        /// GameManager.instance.SetStartGame(true);

        background.gameObject.SetActive(true);
        background.sprite = backgroundImages[bossIndex];
        ground.material = groundMaterials[bossIndex];
        SummonBoss(bossPrefabs[bossIndex]);
        playerStatus.RestartPlaying();

        pause = false;
    }

    public void FinishLevel(bool result)
    {
        if (!pause)
        {
            playerStatus.StopPlaying();
            background.gameObject.SetActive(false);

            BattleDataManager.instance.EvaluateGameResult(result);
            pause = true;
        }
    }

    public int SummonOne()
    {
        int p = -1;
        List<int> avaliablePosition = new List<int>();
        for (int i = 0; i < generationPoint.Count; i++)
        {
            if (!generationPointStatus[i])
                avaliablePosition.Add(i);
        }
        if (avaliablePosition.Count > 0)
        {
            Debug.Log(avaliablePosition.Count);
            p = avaliablePosition[Random.Range(0, avaliablePosition.Count - 1)];
            Debug.Log(p);

            GameObject tmp = Instantiate(enemyPrefab, generationPoint[p]);
            tmp.transform.localPosition = new Vector3(0, 0, 0);
            generationPointStatus[p] = tmp.GetComponent<EnemyStatus>();
            BattleDataManager.instance.AddEnemyData(generationPointStatus[p]);
            generationPointStatus[p].position = p;
        }
        return p;
    }

    // �ٻ�С�֣�λ�ò������޷��ٻ�
    public int SummonMinion(GameObject prefab)
    {
        int p = -1;
        List<int> avaliablePosition = new List<int>();
        for (int i = 0; i < generationPoint.Count; i++)
        {
            if (!generationPointStatus[i])
                avaliablePosition.Add(i);
        }
        if (avaliablePosition.Count > 0)
        {
            p = avaliablePosition[Random.Range(0, avaliablePosition.Count - 1)];

            GameObject tmp = Instantiate(prefab, generationPoint[p]);
            tmp.transform.localPosition = new Vector3(0, 0, 0);
            generationPointStatus[p] = tmp.GetComponent<EnemyStatus>();
            BattleDataManager.instance.AddEnemyData(generationPointStatus[p]);
            generationPointStatus[p].position = p;
        }
        return p;
    }
    public void SummonMinionAt(GameObject prefab, int p)
    {
        if (!generationPointStatus[p])
        {
            GameObject tmp = Instantiate(prefab, generationPoint[p]);
            tmp.transform.localPosition = new Vector3(0, 0, 0);
            generationPointStatus[p] = tmp.GetComponent<EnemyStatus>();
            BattleDataManager.instance.AddEnemyData(generationPointStatus[p]);
            generationPointStatus[p].position = p;
        }
    }

    // �ٻ�Boss���ٻ���λ��4
    public int SummonBoss(GameObject prefab)
    {
        int p = -1;
        if (!generationPointStatus[4])
        {
            p = 4;
            GameObject tmp = Instantiate(prefab, generationPoint[p]);
            tmp.transform.localPosition = new Vector3(0, 0, 0);
            generationPointStatus[p] = tmp.GetComponent<EnemyStatus>();
            BattleDataManager.instance.AddEnemyData(generationPointStatus[p]);
            generationPointStatus[p].position = p;
        }
        return p;
    }

    // ������λ���ٻ�
    public GameObject SummonInMiddle(GameObject prefab)
    {
        GameObject tmp = Instantiate(prefab, middleGenerationPoint);
        tmp.transform.localPosition = new Vector3(0, 0, 0);
        itemInterface = tmp.GetComponent<LevelItemInterface>();
        return tmp;
    }

    public void Die(int p)
    {
        BattleDataManager.instance.RemoveEnemyData(generationPointStatus[p]);
        generationPointStatus[p] = null;
    }

    public void RemoveMinions()
    {
        for (int i = 0; i < generationPoint.Count; i++)
        {
            if (i!=4 && generationPointStatus[i])
                generationPointStatus[i].Kill();
        }
    }

    public void RemovAllEnemy()
    {
        for (int i = 0; i < generationPoint.Count; i++)
        {
            if (generationPointStatus[i])
                generationPointStatus[i].Kill();
        }
    }

    public void Clear()
    {
        for (int i = 0; i < generationPoint.Count; i++)
        {
            if (generationPointStatus[i])
                generationPointStatus[i].Kill();
        }
        playerStatus.StopPlaying();
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("SummonedObject"))
        {
            Destroy(i);
        }
    }

    public void HurtAll(float damage)
    {
        for (int i = 0; i < generationPoint.Count; i++)
        {
            if (generationPointStatus[i])
                generationPointStatus[i].Hurt(damage);
        }
    }

    public void RemoveShieldAll()
    {
        for (int i = 0; i < generationPoint.Count; i++)
        {
            if (generationPointStatus[i])
                generationPointStatus[i].RemoveShield();
        }
    }

    public void PercentHurtAll(float percent, float max = Mathf.Infinity)
    {
        for (int i = 0; i < generationPoint.Count; i++)
        {
            if (generationPointStatus[i])
                generationPointStatus[i].PercentHurt(percent, max);
        }
    }

    public void Pause()
    {
        pause = true;
    }

    public void Resume()
    {
        pause = false;
    }

    public bool ActivateItem()
    {
        if (itemInterface != null)
        {
            itemInterface.Activate();
            return true;
        }
        else
            return false;
    }

    public void Walk()
    {
        if (itemInterface != null)
        {
            itemInterface.Walk();
        }
    }

    public List<int> FindPositionInRange(int p, float range)
    {
        List<int> targetList = new List<int>();
        Vector3 sPosition = generationPoint[p].position;
        for (int i = 0; i < generationPoint.Count; i++)
        {
            if (i != p && generationPointStatus[i] && range > (sPosition - generationPoint[i].position).magnitude)
                targetList.Add(i);
        }

        return targetList;
    }

    public GameObject GetGameObjectAt(int p)
    {
        if (generationPointStatus[p])
            return generationPointStatus[p].gameObject;
        else
            return null;
    }
}
