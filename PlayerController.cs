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

    private float jumpTimeCounter;//跳跃时间计时器
    private float clickTimer;//双击计时器
    private float playerGravity;//玩家初始重力

    private bool onGround;//是否碰撞在Ground层
    private bool onOneWayPlatform;//是否碰撞在OneWayPlatfrom层
    private bool onLadder;//是否碰撞Ladder层
    private bool isJumping;//是否处于跳跃状态
    private bool isFalling;//是否处于下落状态
    private bool isClicked;//是否双击按键
    private bool isClimbing;//是否处于攀爬状态
    private bool isDead = false;//玩家是否死亡，默认为false

    public float moveSpeed;//移动速度
    public float jumpForce;//跳跃力
    public float jumpTime;//最大跳跃时间
    public float climbSpeed;//攀爬速度
    public float RestoreTime;//恢复图层速度

    // Start is called before the first frame update
    // 获取Player初始状态与属性
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //获取刚体
        feet = GetComponent<BoxCollider2D>(); //获取足部盒型碰撞体
        anim = GetComponent<Animator>(); //获取动画器
        body = GetComponent<CapsuleCollider2D>();
        stampPoint = transform.Find("StampPoint");
        playerGravity = rb.gravityScale; //获取初始重力大小
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

    //该方法实现Player的水平移动与水平移动动画播放
    void Move()
    {
        if (!isDead)
        {
            float x = Input.GetAxisRaw("Horizontal");
            if (x == 1)
                transform.localRotation = Quaternion.Euler(0, 0, 0);//y轴不旋转
            else if (x == -1)
                transform.localRotation = Quaternion.Euler(0, 180, 0);//y轴翻转180

            rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
            bool playerHasXAxisSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;    //判断是否角色是否有水平速度
            anim.SetBool("Run", playerHasXAxisSpeed);   //根据角色是否有水平速度决定是否播放移动动画
        }
    }

    //判断角色是否处于相应的平台上（用图层Layer判断）
    void CheckOnGround()
    {
        onGround = feet.IsTouchingLayers(LayerMask.GetMask("Ground")) ||
                   feet.IsTouchingLayers(LayerMask.GetMask("OneWayPlatform"));
        onOneWayPlatform = feet.IsTouchingLayers(LayerMask.GetMask("OneWayPlatform"));
    }

    //判断角色是否处于梯子图层
    void CheckLadder()
    {
        onLadder = feet.IsTouchingLayers(LayerMask.GetMask("Ladder"));
    }

    //该方法实现角色的跳跃功能
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

            if (Input.GetKey(KeyCode.Space) && isJumping == true)//长按跳得更高
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

    //该方法用于实现单向平台的跳跃与下落功能。通过改变Player的图层与OneWayPlatform图层一致实现下落并迅速恢复Player图层为Player以避免无限下落
    void OneWayPlatformJump()
    {
        if (!isDead)
        {
            if (onOneWayPlatform)
            {
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))//当双击向下按键时下落
                {
                    isClicked = true;
                }
                if (isClicked)
                {
                    if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && clickTimer >= 0.1f)
                    {
                        gameObject.layer = LayerMask.NameToLayer("OneWayPlatform");
                        Invoke("RestorePlayerLayer", RestoreTime);//该方法表示在Restore时间后调用“”中的RestorePlayerLayer()方法
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

    //该方法配合OneWayPlatformJump方法，用于恢复Player的图层
    void RestorePlayerLayer()
    {
        if (!onGround && gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }

    //该方法实现Player攀爬梯子的功能
    void Climb()
    {
        if (!isDead)
        {
            if (onLadder)
            {
                rb.gravityScale = 0.0f;//重力设为0防止其在梯子上滑落
                float moveY = Input.GetAxisRaw("Vertical");
                if (moveY > 0.5f || moveY < -0.5f)//当Player竖直方向速度很小时不认为它在爬
                {
                    isClimbing = true;
                    anim.SetBool("Climbing", isClimbing);
                    rb.velocity = new Vector2(rb.velocity.x, moveY * climbSpeed);//给刚体添加竖直方向加速度，水平方向保持不变
                }
                else //没在爬不启用攀爬动画
                {
                    if (isJumping)
                    {
                        isClimbing = false;
                        isJumping = false;
                        anim.SetBool("Climbing", isClimbing);
                        anim.SetBool("Jumping", isJumping);
                        rb.velocity = new Vector2(rb.velocity.x, 0.0f);//没有在爬，但是可以停在梯子上
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
            else//不在梯子上时重力恢复
            {
                anim.SetBool("Climbing", false);
                rb.gravityScale = playerGravity;
            }
        }
    }

    //检查Player是否在下落
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

    //player足部加一圆形区域，碰撞则消灭碰撞体
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
                Debug.Log("踩到了" + c.name);
                if (c.CompareTag("EnemyEagle"))
                {
                    c.gameObject.GetComponent<EnemyEagle>().SetDeadStatus();
                }
                if (c.CompareTag("EnemyOpossum"))
                {
                    c.gameObject.GetComponent<EnemyOpossum>().SetDeadStatus();
                }
                //player踩踏后有反弹力
                rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                rb.AddForce(new Vector2(0.0f, 400f));
            }
        }
    }

    //player身体接触则死亡
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

    //重新加载关卡
    void Restart()
    {
        SceneManager.LoadScene("Level-1");
    }

    //载入下一场景
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Door_1"))
        {
            SceneManager.LoadScene("PassedScene");
        }
    }
}
