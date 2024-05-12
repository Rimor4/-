using UnityEngine;

public class UIManager : MonoBehaviour
{
    public PlayerStatBar playerStatBar;

    [Header("Event Listening")]
    public CharacterEventSO healthEvent;        

    private void OnEnable() {
        // 监听来自CharacterEventSO广播的事件
        healthEvent.OnEventRaised += OnHealthEvent;
    }

    private void OnDisable() {
        healthEvent.OnEventRaised -= OnHealthEvent;
    }

    private void OnHealthEvent(Character character)
    {
        var percentage = character.currentHealth / character.maxHealth;
        playerStatBar.OnHealthChange(percentage);

        playerStatBar.OnPowerChange(character); 
    }
}
