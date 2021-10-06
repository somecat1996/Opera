using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 角色随机移动脚本

public class PlayerMovement : MonoBehaviour
{
    // 移动速度
    public int moveSpeed;
    // 使用左下右上两点限制移动范围
    public List<Transform> moveAera;
    // 移动时间范围
    public List<float> walkTimeRange;
    // 静止时间范围
    public List<float> stayTimeRange;
    public Transform spriteTransform;

    // 状态计时器
    private float timer;
    // 当前状态
    private bool walk;
    // 移动方向
    private Vector3 moveDirection;

    private Rigidbody rigidbody;
    private Vector3 spriteOriginSacle;

    private PlayerStatus playerStatus;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        spriteOriginSacle = spriteTransform.localScale;
        // 初始化为运动状态
        walk = true;
        timer = Random.Range(walkTimeRange[0], walkTimeRange[1]);
        RandomDirection();
        rigidbody.velocity = moveDirection * moveSpeed;

        playerStatus = gameObject.GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        // 计时器归零后改变状态
        if (timer <= 0)
        {
            if (walk)
            {
                playerStatus.StopMoving();
                walk = false;
                timer = Random.Range(stayTimeRange[0], stayTimeRange[1]);
                rigidbody.velocity = Vector3.zero * moveSpeed;
            }
            else
            {
                playerStatus.StartMoving();
                walk = true;
                timer = Random.Range(walkTimeRange[0], walkTimeRange[1]);
                RandomDirection();
                rigidbody.velocity = moveDirection * moveSpeed;
            }
        }
        // 判断范围
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

    void RandomDirection()
    {
        // 随机移动方向
        moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        moveDirection = moveDirection.normalized;
        Flip();
    }

    void Flip()
    {
        // 改变人物朝向
        if (moveDirection.x < 0)
            spriteTransform.localScale = new Vector3(spriteOriginSacle.x, -spriteOriginSacle.y, spriteOriginSacle.z);
        else
            spriteTransform.localScale = new Vector3(spriteOriginSacle.x, spriteOriginSacle.y, spriteOriginSacle.z);
    }
}
