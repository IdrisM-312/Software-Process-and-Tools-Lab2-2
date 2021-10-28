using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEagle : MonoBehaviour
{
    private float waitTime;
    private Animator anim;

    public float moveSpeed;
    public float startWaitTime;

    public Transform movePos;
    public Transform minPos;
    public Transform maxPos;

    // Start is called before the first frame update
    void Start()
    {
        waitTime = startWaitTime;
        movePos.position = GetRandomPos();
        anim = GetComponent<Animator>();
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
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
        
    }

    Vector2 GetRandomPos()
    {
        Vector2 randPos = new Vector2(Random.Range(minPos.position.x, maxPos.position.x), Random.Range(minPos.position.y,maxPos.position.y));
        return randPos;
    }

    public void SetDeadStatus()
    {
        anim.SetBool("Dead", true);
        AudioManager.PlayBoom();
        Invoke("Kill",0.35f);
    }

    private void Kill()
    {
        Destroy(gameObject);
    }
}
