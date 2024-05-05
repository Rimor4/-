using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;
    private PhysicsCheck physicsCheck;
    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private PlayerAnimation playerAnimation;
    private Character character;
    public Vector2 inputDirection;

    [Header("Basic parameters")]
    public float speed;
    public float jumpForce;
    public float wallJumpForce;
    public float hurtForce;
    public float slideDistance;
    public float slideSpeed;
    public float slidePowerCost;

    private float runSpeed;
    private float walkSpeed => speed / 2.5f;     // Lambda表达式, 当尝试访问walkSpeed时, 计算该表达式 
    private Vector2 originSize;
    private Vector2 originOffset;

    [Header("Physic Material")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    [Header("State")]
    public bool isCrouch;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;
    public bool wallJump;
    public bool isSlide;

    private void Awake()
    {
        inputControl = new PlayerInputControl();
        physicsCheck = GetComponent<PhysicsCheck>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        character = GetComponent<Character>();

        // 碰撞体初始参数
        originSize = coll.size;
        originOffset = coll.offset;

        // 跳跃:    按下Jump对应的按键时调用Jump()方法;
        inputControl.Gameplay.Jump.started += Jump;
        #region 强制走路
        runSpeed = speed;
        inputControl.Gameplay.WalkButton.performed += ctx =>
        {
            if (physicsCheck.isGround)
            {
                speed = walkSpeed;
            }
        };
        inputControl.Gameplay.WalkButton.canceled += ctx =>
        {
            if (physicsCheck.isGround)
            {
                speed = runSpeed;
            }
        };
        #endregion
        // 攻击
        inputControl.Gameplay.Attack.started += PlayerAttack;
        inputControl.Gameplay.Slide.started += Slide;
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void Update()
    {
        // 键盘输入方向  
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        CheckState();
        if (!isHurt && !isAttack)
        {
            Move();
        }
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    private void Move()
    {
        if (!isCrouch && !wallJump)
        {
            // 蹬墙跳
            rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
        }

        int faceDir = (int)transform.localScale.x;

        if (inputDirection.x > 0)
        {
            faceDir = 1;
            physicsCheck.bottomOffset = new Vector2(-0.08f, 0.1f);
        }
        else if (inputDirection.x < 0)
        {
            faceDir = -1;
            physicsCheck.bottomOffset = new Vector2(0.08f, 0.1f);
        }

        // 人物翻转
        transform.localScale = new Vector3(faceDir, 1, 1);

        // 人物下蹲
        isCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;
        if (isCrouch)
        {
            // 修改碰撞体大小和位移
            coll.size = new Vector2(0.74f, 1.66f);
            coll.offset = new Vector2(-0.09f, 0.84f);
            // 停止人物移动
            rb.velocity = Vector2.zero;
        }
        else
        {
            // 还原原来碰撞体参数
            coll.size = originSize;
            coll.offset = originOffset;
        }
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

            // 打断滑铲协程
            isSlide = false;
            StopAllCoroutines();
        }
        else if (physicsCheck.onWall)
        {
            rb.AddForce(new Vector2(-inputDirection.x, 2.5f) * wallJumpForce, ForceMode2D.Impulse);
            wallJump = true;
        }
    }

    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        playerAnimation.PlayerAttack();
        isAttack = true;
    }

    private void Slide(InputAction.CallbackContext obj)
    {
        if (!isSlide && physicsCheck.isGround && character.currentPower >= slidePowerCost)
        {
            isSlide = true;

            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x, transform.position.y);

            StartCoroutine(TriggerSlide(targetPos));

            GetComponent<Character>().OnSlide(slidePowerCost);  
        }
    }

    private IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {
            // 不需要暂停，直接返回null
            yield return null;

            if (!physicsCheck.isGround)
            {
                break;
            }
            // 碰墙判断
            else if (physicsCheck.touchLeftWall && transform.localScale.x < 0f || physicsCheck.touchRightWall && transform.localScale.x > 0f)
            {
                break;
            }
            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed, transform.position.y));
        } while (Mathf.Abs(target.x - transform.position.x) > 0.1f);

        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public void CheckState()
    {
        // 切换人物擦墙材质
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;

        // 切换onWall时速度
        if (physicsCheck.onWall)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);

        // 检测取消蹬墙跳
        if (wallJump && rb.velocity.y < 0f)
        {
            wallJump = false;
        }

        // 死亡后不受伤，滑铲时无敌
        if (isDead || isSlide) {
            gameObject.layer = LayerMask.NameToLayer("Enemy"); 
        } else {
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }

    #region UnityEvent
    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;

        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    public void PlayerDead()
    {
        isDead = true;
        inputControl.Gameplay.Disable();
    }
    #endregion
}
