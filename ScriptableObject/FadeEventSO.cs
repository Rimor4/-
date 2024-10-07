using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/FadeEventSO")]
// 直接在此SO中定义调用的两个事件函数
public class FadeEventSO : ScriptableObject
{
    public UnityAction<Color, float> OnEventRaised;

    public void FadeIn(float duration)
    {
        RaiseEvent(Color.black, duration);
    }

    public void FadeOut(float duration)
    {
        RaiseEvent(Color.clear, duration);
    }

    public void RaiseEvent(Color target, float duration)
    {
        OnEventRaised?.Invoke(target, duration);
    }
}