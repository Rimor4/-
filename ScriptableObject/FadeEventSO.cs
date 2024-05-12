using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/FadeEventSO")]
// 直接在此SO中定义调用的两个OnEventRaised函数
public class FadeEventSO : ScriptableObject {
    public UnityAction<Color, float, bool> OnEventRaised;

    public void FadeIn(float duration) {
        RaiseEvent(Color.black, duration, true);
    }

    public void FadeOut(float duration) {
        RaiseEvent(Color.clear, duration, false);
    }

    public void RaiseEvent(Color target, float duration, bool fadeIn) {
        OnEventRaised?.Invoke(target, duration, fadeIn);
    }
}