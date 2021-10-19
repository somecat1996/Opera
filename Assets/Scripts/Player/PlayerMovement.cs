using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��ɫ����ƶ��ű�

public class PlayerMovement : MonoBehaviour
{
    // �ƶ��ٶ�
    public int moveSpeed;
    // ʹ�������������������ƶ���Χ
    public List<Transform> moveAera;
    // �ƶ�ʱ�䷶Χ
    public List<float> walkTimeRange;
    // ��ֹʱ�䷶Χ
    public List<float> stayTimeRange;
    private Transform spriteTransform;

    // ״̬��ʱ��
    private float timer;
    // ��ǰ״̬
    private bool walk;
    // �ƶ�����
    private Vector3 moveDirection;

    private Rigidbody rigidbody;
    private Vector3 spriteOriginSacle;

    private PlayerStatus playerStatus;

    private bool started;
    public Vector3 startPoint;
    private void Awake()
    {
        spriteTransform = gameObject.GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();
        spriteOriginSacle = spriteTransform.localScale;
        // ��ʼ��Ϊ�˶�״̬
        walk = true;
        timer = Random.Range(walkTimeRange[0], walkTimeRange[1]);

        playerStatus = gameObject.GetComponent<PlayerStatus>();
    }
    // Start is called before the first frame update
    void Start()
    {
        StopMoving();
    }

    // Update is called once per frame
    void Update()
    {
        if (started && !EnemyManager.instance.pause)
        {
            timer -= Time.deltaTime;
            // ��ʱ�������ı�״̬
            if (timer <= 0)
            {
                if (walk)
                {
                    Player.instance.Walk(false);
                    playerStatus.StopMoving();
                    walk = false;
                    timer = Random.Range(stayTimeRange[0], stayTimeRange[1]);
                    rigidbody.velocity = Vector3.zero * moveSpeed;
                }
                else
                {
                    Player.instance.Walk(true);
                    playerStatus.StartMoving();
                    walk = true;
                    timer = Random.Range(walkTimeRange[0], walkTimeRange[1]);
                    RandomDirection();
                    rigidbody.velocity = moveDirection * moveSpeed;

                    EnemyManager.instance.Walk();
                }
            }
            // �жϷ�Χ
            if (transform.position.x < moveAera[0].position.x && moveDirection.x < 0 || transform.position.x > moveAera[1].position.x && moveDirection.x > 0)
            {
                moveDirection.x = -moveDirection.x;
                rigidbody.velocity = moveDirection * moveSpeed;
                Flip();
            }
            if (transform.position.z < moveAera[0].position.z && moveDirection.z < 0 || transform.position.z > moveAera[1].position.z && moveDirection.z > 0)
            {
                moveDirection.z = -moveDirection.z;
                rigidbody.velocity = moveDirection * moveSpeed;
                Flip();
            }
        }
    }

    void RandomDirection()
    {
        // ����ƶ�����
        moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        moveDirection = moveDirection.normalized;
        Flip();
    }

    void Flip()
    {
        // �ı����ﳯ��
        if (moveDirection.x < 0)
            spriteTransform.localScale = new Vector3(-spriteOriginSacle.x, spriteOriginSacle.y, spriteOriginSacle.z);
        else
            spriteTransform.localScale = new Vector3(spriteOriginSacle.x, spriteOriginSacle.y, spriteOriginSacle.z);
    }

    public void StartMoving()
    {
        Player.instance.Walk(true);
        started = true;
        RandomDirection();
        rigidbody.velocity = moveDirection * moveSpeed;
    }

    public void StopMoving()
    {
        Player.instance.Walk(false);
        started = false;
        rigidbody.velocity = Vector3.zero;
    }

    public void StartMovingAt()
    {
        transform.position = startPoint;
        StartMoving();
    }
}
