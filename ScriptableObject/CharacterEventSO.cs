using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Composites;

[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    // 广播Character发出的事件请求
    public UnityAction<Character> OnEventRaised;

    public void RaiseEvent(Character character) {
        OnEventRaised?.Invoke(character);
    }
}
