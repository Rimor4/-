using UnityEngine;
using UnityEngine.Events;

// 有关伤害的类方法
public class Character : MonoBehaviour
{
    [Header("Attributes")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    [Header("受伤无敌")]
    public float invulnerableDuration;
    [HideInInspector] public float invulnerableCounter;
    public bool invulnerable;

    public UnityEvent<Character> OnHealthChange;    // 使用UnityEvent来呼叫SO, 而不是向TeleportPoint那样在脚本中调用SO方法
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;

    private void Start()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);       // 相当于SO.RaiseEvent(this);  
    }

    private void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }

        if (currentPower < maxPower) {
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        // 角色掉入水中, 死亡
        if (other.CompareTag("Water")) {
            currentHealth = 0;
            OnHealthChange?.Invoke(this);
            OnDie?.Invoke();
        }
    }

    public void TakeDamage(Attack attacker)
    {
        if (invulnerable)
            return;

        float afterAttackHealth = currentHealth - attacker.damage;
        if (afterAttackHealth > 0)
        {
            currentHealth = afterAttackHealth;
            TriggerInvulnerable();
            // 执行受伤
            OnTakeDamage?.Invoke(attacker.transform);
        }
        else
        {
            currentHealth = 0;
            // 触发死亡
            OnDie?.Invoke();
        }

        OnHealthChange?.Invoke(this);
    }

    private void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    public void OnSlide(float cost) {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }
}
