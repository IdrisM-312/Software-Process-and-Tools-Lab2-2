using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOpossum : MonoBehaviour
{
    private Animator anim;

    private float waitTime;//�ȴ�ʱ��

    public float moveSpeed;
    public float startWaitTime;//�ٴ��ƶ�ǰ��Ҫ�ȴ���ʱ��

    public Transform movePos;
    public Transform minPos;//�ƶ��������Сλ��
    public Transform maxPos;//�ƶ���������λ��

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

    //��ȡ���λ��
    Vector2 GetRandomPos()
    {
        Vector2 randPos = new Vector2(Random.Range(minPos.position.x, maxPos.position.x), gameObject.transform.position.y);
        return randPos;
    }

    //player���ã���Ϊ����״̬��������������
    public void SetDeadStatus()
    {
        anim.SetBool("Dead", true);
        AudioManager.PlayBoom();
        Invoke("Kill", 0.35f);
    }

    //���ٶ���
    private void Kill()
    {
        Destroy(gameObject);
    }
}
