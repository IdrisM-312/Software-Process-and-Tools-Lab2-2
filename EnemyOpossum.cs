using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOpossum : MonoBehaviour
{
    private Animator anim;

    private float waitTime;//等待时间

    public float moveSpeed;
    public float startWaitTime;//再次移动前需要等待的时间

    public Transform movePos;
    public Transform minPos;//移动区域的最小位置
    public Transform maxPos;//移动区域的最大位置

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        waitTime = startWaitTime;
        movePos.position = GetRandomPos();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, movePos.position, moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, movePos.position) < 0.1f)
        {
            if(waitTime <= 0)
            {
                movePos.position = GetRandomPos();
                if(movePos.position.x - transform.position.x <= 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                waitTime = startWaitTime;
                anim.SetBool("Move",true);
            }
            else
            {
                waitTime -= Time.deltaTime;
                anim.SetBool("Move",false);
            }
        }
        
    }

    //获取随机位置
    Vector2 GetRandomPos()
    {
        Vector2 randPos = new Vector2(Random.Range(minPos.position.x, maxPos.position.x), gameObject.transform.position.y);
        return randPos;
    }

    //player调用，设为死亡状态，播放死亡动画
    public void SetDeadStatus()
    {
        anim.SetBool("Dead", true);
        AudioManager.PlayBoom();
        Invoke("Kill", 0.35f);
    }

    //销毁对象
    private void Kill()
    {
        Destroy(gameObject);
    }
}
