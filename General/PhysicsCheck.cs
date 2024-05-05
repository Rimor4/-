using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D coll;
    private PlayerController playerController;
    private Rigidbody2D rb;

    [Header("Test parameter")]
    public bool isPlayer;
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public float checkRadius;
    public LayerMask groundLayer;
    
    [Header("State")]
    public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;
    public bool onWall;

    private void Awake() {
        coll = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        isGround = true;

        if (isPlayer)
            playerController = GetComponent<PlayerController>();
    }

    private void Update() {
        Check();
    }

    private void Check() {
        // 检测地面
        // onWall 时滞后地面监测
        isGround = Physics2D.OverlapCircle(new Vector2(bottomOffset.x * transform.localScale.x, onWall ? bottomOffset.y : 0) + (Vector2)transform.position, checkRadius, groundLayer);

        // 墙体判断
        touchLeftWall = Physics2D.OverlapCircle(leftOffset + (Vector2)transform.position, checkRadius, groundLayer);
        touchRightWall = Physics2D.OverlapCircle(rightOffset + (Vector2)transform.position, checkRadius, groundLayer);

        // 在墙壁上
        if (isPlayer)
            onWall = (touchLeftWall && playerController.inputDirection.x < 0f || touchRightWall && playerController.inputDirection.x > 0f) && rb.velocity.y < 0f;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRadius);
    }
}
