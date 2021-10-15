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
        pause = false;
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
            EnterLevel(0);
    }

    public void EnterLevel(int bossIndex)
    {
        /// test
        GameManager.instance.SetStartGame(true);

        background.gameObject.SetActive(true);
        background.sprite = backgroundImages[bossIndex];
        ground.material = groundMaterials[bossIndex];
        SummonBoss(bossPrefabs[bossIndex]);
        playerStatus.RestartPlaying();
    }

    public void FinishLevel(bool result)
    {
        playerStatus.StopPlaying();
        background.gameObject.SetActive(false);

        BattleDataManager.instance.EvaluateGameResult(result);
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
        return tmp;
    }

    public void Die(int p)
    {
        BattleDataManager.instance.RemoveEnemyData(generationPointStatus[p]);
        generationPointStatus[p] = null;

        int count = 0;
        for (int i = 0; i < generationPoint.Count; i++)
        {
            if (generationPointStatus[i])
                count += 1;
        }
        if (count == 0)
        {
            FinishLevel(true);
        }
    }

    public void RemoveMinions()
    {
        for (int i = 0; i < generationPoint.Count; i++)
        {
            if (i!=4 && generationPointStatus[i])
                generationPointStatus[i].Die();
        }
    }

    public void RemovAll()
    {
        for (int i = 0; i < generationPoint.Count; i++)
        {
            if (i != 4 && generationPointStatus[i])
                generationPointStatus[i].Die();
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
}
