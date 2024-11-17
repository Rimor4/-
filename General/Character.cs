using UnityEngine;
using UnityEngine.Events;

// 记录人物基本属性、计算伤害的类
public class Character : MonoBehaviour, ISaveable
{
    [Header("Event Listening")]
    public VoidEventSO newGameEvent;

    [Header("Attributes")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    [Header("Invulnerable after injury")]
    public float invulnerableDuration;
    [HideInInspector] public float invulnerableCounter;
    public bool invulnerable;

    public UnityEvent<Character> OnHealthChange;    // 启动UnityEvent来呼叫SO（中介）来向订阅者传递数据, 与向TeleportPoint那样直接在脚本中调用SO方法不同
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void NewGame()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }

    private void OnEnable()
    {
        newGameEvent.OnEventRaised += NewGame;
        ISaveable saveable = this;
        saveable.RegiseterSaveData();
    }

    private void OnDisable()
    {
        newGameEvent.OnEventRaised -= NewGame;
        ISaveable saveable = this;
        saveable.UnRegiseterSaveData();
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

        if (currentPower < maxPower)
        {
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 角色掉入水中, 死亡
        if (other.CompareTag("Water"))
        {
            if (currentHealth > 0)
            {
                currentHealth = 0;
                OnHealthChange?.Invoke(this);
                OnDie?.Invoke();
            }
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
            // 触发OnTakeDamage事件
            OnTakeDamage?.Invoke(attacker.transform);
        }
        else
        {
            currentHealth = 0;
            // 触发OnDie事件
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

    /// <summary>
    /// 消耗能量来滑铲
    /// </summary>
    /// <param name="cost"></param>
    public void OnSlide(float cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    /// <summary>
    /// 向'data'容器中添加人物的位置信息
    /// </summary>
    /// <param name="data"></param>
    public void GetSaveData(Data data)
    {
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            data.characterPosDict[GetDataID().ID] = transform.position;
            data.floatSavedData[GetDataID().ID + "health"] = this.currentHealth;
            data.floatSavedData[GetDataID().ID + "power"] = this.currentPower;
        }
        else
        {
            data.characterPosDict.Add(GetDataID().ID, transform.position);
            data.floatSavedData.Add(GetDataID().ID + "health", this.currentHealth);
            data.floatSavedData.Add(GetDataID().ID + "power", this.currentPower);
        }
    }

    /// <summary>
    /// 从'data'容器中读取人物的位置信息
    /// </summary>
    /// <param name="data"></param>
    public void LoadData(Data data)
    {
        Debug.Log(data.characterPosDict.Values);
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            transform.position = data.characterPosDict[GetDataID().ID];
            this.currentHealth = data.floatSavedData[GetDataID().ID + "health"];
            this.currentPower = data.floatSavedData[GetDataID().ID + "power"];

            OnHealthChange?.Invoke(this);
        }
    }
}
