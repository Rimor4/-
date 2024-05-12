using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvas : MonoBehaviour
{
    [Header("Event Listening")]
    public FadeEventSO fadeEvent;
    public Image FadeImage;

    private void OnEnable() {
        fadeEvent.OnEventRaised += OnFadeEvent; 
    }

    private void OnDisable() {
        fadeEvent.OnEventRaised -= OnFadeEvent;
    }

    private void OnFadeEvent(Color target, float duration, bool fadeIn) {
        FadeImage.DOBlendableColor(target, duration);
    }
}
