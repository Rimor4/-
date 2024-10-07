using UnityEngine;
using UnityEngine.Events;

// 为编辑器中创建物体的菜单中添加此SO
[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    // 作为中介把Character组件中的信息发出去
    // 广播Character发出的事件请求
    public UnityAction<Character> OnEventRaised;

    public void RaiseEvent(Character character)
    {
        OnEventRaised?.Invoke(character);
    }
}
