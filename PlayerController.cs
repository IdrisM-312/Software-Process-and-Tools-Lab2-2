using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D feet;
    private CapsuleCollider2D body;
    private Animator anim;

    private Transform stampPoint;

    private float jumpTimeCounter;//��Ծʱ���ʱ��
    private float clickTimer;//˫����ʱ��
    private float playerGravity;//��ҳ�ʼ����

    private bool onGround;//�Ƿ���ײ��Ground��
    private bool onOneWayPlatform;//�Ƿ���ײ��OneWayPlatfrom��
    private bool onLadder;//�Ƿ���ײLadder��
    private bool isJumping;//�Ƿ�����Ծ״̬
    private bool isFalling;//�Ƿ�������״̬
    private bool isClicked;//�Ƿ�˫������
    private bool isClimbing;//�Ƿ�������״̬
    private bool isDead = false;//����Ƿ�������Ĭ��Ϊfalse

    public float moveSpeed;//�ƶ��ٶ�
    public float jumpForce;//��Ծ��
    public float jumpTime;//�����Ծʱ��
    public float climbSpeed;//�����ٶ�
    public float RestoreTime;//�ָ�ͼ���ٶ�

    // Start is called before the first frame update
    // ��ȡPlayer��ʼ״̬������
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //��ȡ����
        feet = GetComponent<BoxCollider2D>(); //��ȡ�㲿������ײ��
        anim = GetComponent<Animator>(); //��ȡ������
        body = GetComponent<CapsuleCollider2D>();
        stampPoint = transform.Find("StampPoint");
        playerGravity = rb.gravityScale; //��ȡ��ʼ������С
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        StampCheck();
        Move();
    }

    void Update()
    {
        CheckOnGround();
        Jump();
        OneWayPlatformJump();
        CheckLadder();
        Climb();
        CheckFallingStatus();
    }

    //�÷���ʵ��Player��ˮƽ�ƶ���ˮƽ�ƶ���������
    void Move()
    {
        if (!isDead)
        {
            float x = Input.GetAxisRaw("Horizontal");
            if (x == 1)
                transform.localRotation = Quaternion.Euler(0, 0, 0);//y�᲻��ת
            else if (x == -1)
                transform.localRotation = Quaternion.Euler(0, 180, 0);//y�ᷭת180

            rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
            bool playerHasXAxisSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;    //�ж��Ƿ��ɫ�Ƿ���ˮƽ�ٶ�
            anim.SetBool("Run", playerHasXAxisSpeed);   //���ݽ�ɫ�Ƿ���ˮƽ�ٶȾ����Ƿ񲥷��ƶ�����
        }
    }

    //�жϽ�ɫ�Ƿ�����Ӧ��ƽ̨�ϣ���ͼ��Layer�жϣ�
    void CheckOnGround()
    {
        onGround = feet.IsTouchingLayers(LayerMask.GetMask("Ground")) ||
                   feet.IsTouchingLayers(LayerMask.GetMask("OneWayPlatform"));
        onOneWayPlatform = feet.IsTouchingLayers(LayerMask.GetMask("OneWayPlatform"));
    }

    //�жϽ�ɫ�Ƿ�������ͼ��
    void CheckLadder()
    {
        onLadder = feet.IsTouchingLayers(LayerMask.GetMask("Ladder"));
    }

    //�÷���ʵ�ֽ�ɫ����Ծ����
    void Jump()
    {
        if (!isDead)
        {
            if (onGround == true && Input.GetKeyDown(KeyCode.Space))
            {
                isJumping = true;
                //isFalling = false;
                jumpTimeCounter = jumpTime;
                rb.velocity = Vector2.up * jumpForce;
            }

            if (Input.GetKey(KeyCode.Space) && isJumping == true)//�������ø���
            {
                if (jumpTimeCounter > 0)
                {
                    rb.velocity = Vector2.up * jumpForce;
                    jumpTimeCounter -= Time.deltaTime;
                }
                else if (jumpTimeCounter < 0)
                {
                    isJumping = false;
                    //isFalling = true;
                }
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isJumping = false;
                //isFalling = true;
            }
            isFalling = CheckFallingStatus();
            anim.SetBool("OnGround", onGround);
            anim.SetBool("Jumping", isJumping);
            anim.SetBool("Fall", isFalling);
        }
    }

    //�÷�������ʵ�ֵ���ƽ̨����Ծ�����书�ܡ�ͨ���ı�Player��ͼ����OneWayPlatformͼ��һ��ʵ�����䲢Ѹ�ٻָ�Playerͼ��ΪPlayer�Ա�����������
    void OneWayPlatformJump()
    {
        if (!isDead)
        {
            if (onOneWayPlatform)
            {
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))//��˫�����°���ʱ����
                {
                    isClicked = true;
                }
                if (isClicked)
                {
                    if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && clickTimer >= 0.1f)
                    {
                        gameObject.layer = LayerMask.NameToLayer("OneWayPlatform");
                        Invoke("RestorePlayerLayer", RestoreTime);//�÷�����ʾ��Restoreʱ�����á����е�RestorePlayerLayer()����
                        clickTimer = 0;
                        isClicked = false;
                    }
                    clickTimer += Time.deltaTime;
                    if (clickTimer >= 0.3f)
                    {
                        clickTimer = 0;
                        isClicked = false;
                    }
                }
                anim.SetBool("OnGround", onGround);
                anim.SetBool("Fall", CheckFallingStatus());
            }
        }
    }

    //�÷������OneWayPlatformJump���������ڻָ�Player��ͼ��
    void RestorePlayerLayer()
    {
        if (!onGround && gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }

    //�÷���ʵ��Player�������ӵĹ���
    void Climb()
    {
        if (!isDead)
        {
            if (onLadder)
            {
                rb.gravityScale = 0.0f;//������Ϊ0��ֹ���������ϻ���
                float moveY = Input.GetAxisRaw("Vertical");
                if (moveY > 0.5f || moveY < -0.5f)//��Player��ֱ�����ٶȺ�Сʱ����Ϊ������
                {
                    isClimbing = true;
                    anim.SetBool("Climbing", isClimbing);
                    rb.velocity = new Vector2(rb.velocity.x, moveY * climbSpeed);//�����������ֱ������ٶȣ�ˮƽ���򱣳ֲ���
                }
                else //û������������������
                {
                    if (isJumping)
                    {
                        isClimbing = false;
                        isJumping = false;
                        anim.SetBool("Climbing", isClimbing);
                        anim.SetBool("Jumping", isJumping);
                        rb.velocity = new Vector2(rb.velocity.x, 0.0f);//û�����������ǿ���ͣ��������
                    }
                    else if (isFalling)
                    {
                        isClimbing = false;
                        isFalling = CheckFallingStatus();
                        anim.SetBool("Climbing", isClimbing);
                        anim.SetBool("Fall", isFalling);
                        rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                    }
                    else
                    {
                        anim.SetBool("Climbing", false);
                        rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                    }
                }
            }
            else//����������ʱ�����ָ�
            {
                anim.SetBool("Climbing", false);
                rb.gravityScale = playerGravity;
            }
        }
    }

    //���Player�Ƿ�������
    bool CheckFallingStatus()
    {
        bool falling;
        if (onGround || onLadder)
        {
            falling = false;
        }
        else if (isJumping)
        {
            falling = false;
        }
        else
        {
            falling = true;
        }
        return falling;
    }

    //player�㲿��һԲ��������ײ��������ײ��
    void StampCheck()
    {
        if (!isDead)
        {
            Collider2D c = Physics2D.OverlapCircle(stampPoint.position, 0.2f, LayerMask.GetMask("Enemy"));
            if (c == null)
            {
                return;
            }
            else
            {
                Debug.Log("�ȵ���" + c.name);
                if (c.CompareTag("EnemyEagle"))
                {
                    c.gameObject.GetComponent<EnemyEagle>().SetDeadStatus();
                }
                if (c.CompareTag("EnemyOpossum"))
                {
                    c.gameObject.GetComponent<EnemyOpossum>().SetDeadStatus();
                }
                //player��̤���з�����
                rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                rb.AddForce(new Vector2(0.0f, 400f));
            }
        }
    }

    //player����Ӵ�������
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Enemy"))
        {
            return;
        }
        else
        {
            if (body.IsTouchingLayers(LayerMask.GetMask("Enemy")))
            {
                anim.SetBool("Dead", true);
                isDead = true;
                AudioManager.PlayBoom();
                Invoke("Restart", 1.5f);
            }
        }
    }

    //���¼��عؿ�
    void Restart()
    {
        SceneManager.LoadScene("Level-1");
    }

    //������һ����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Door_1"))
        {
            SceneManager.LoadScene("PassedScene");
        }
    }
}
