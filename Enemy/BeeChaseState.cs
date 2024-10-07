using UnityEngine;

public class BeeChaseState : BaseState
{
    private Attack attack;
    private Vector3 target;     // 追击玩家的位置
    private Vector3 moveDir;
    private bool isAttack;
    private float attackRateCounter = 0;
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        attack = currentEnemy.GetComponent<Attack>();
        currentEnemy.anim.SetBool("chase", true);
    }

    public override void LogicUpdate()
    {
        // 切换回巡逻状态
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
        }

        target = new Vector3(currentEnemy.attacker.position.x, currentEnemy.attacker.position.y + 1.5f, 0);

        // 判断攻击距离
        if (Mathf.Abs(target.x - currentEnemy.transform.position.x) < attack.attackRange && Mathf.Abs(target.y - currentEnemy.transform.position.y) < attack.attackRange)
        {
            //攻击
            isAttack = true;
            if (!currentEnemy.isHurt)
            {
                // 防止不会被玩家击退
                currentEnemy.rb.velocity = Vector2.zero;
            }

            // 计时器
            attackRateCounter -= Time.deltaTime;
            if (attackRateCounter <= 0)
            {
                attackRateCounter = attack.attackRate;
                currentEnemy.anim.SetTrigger("attack");
            }
        }
        else
        {
            // 超出攻击范围
            isAttack = false;
        }

        moveDir = (target - currentEnemy.transform.position).normalized;
        if (moveDir.x > 0)
        {
            currentEnemy.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (moveDir.x < 0)
        {
            currentEnemy.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public override void PhysicsUpdate()
    {
        // 移动
        if (!currentEnemy.isHurt && !currentEnemy.isDead && !isAttack)
        {
            currentEnemy.rb.velocity = currentEnemy.currentSpeed * Time.deltaTime * moveDir;
        }
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("chase", false);
    }
}
