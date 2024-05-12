using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Event Listening")]
    public PlayAudioEventSO BGMEvent;
    public PlayAudioEventSO FXEvent;

    [Header("Component")]
    public AudioSource BGMSource;
    public AudioSource FXSource;

    private void OnEnable() {
        FXEvent.OnEventRaised += OnFXEvent;
        BGMEvent.OnEventRaised += OnBGMEvent;
    }

    private void OnDisable() {
        FXEvent.OnEventRaised -= OnFXEvent;    
        BGMEvent.OnEventRaised -= OnBGMEvent;
    }

    private void OnFXEvent(AudioClip clip) {
        FXSource.clip = clip;
        FXSource.Play();
    }

    private void OnBGMEvent(AudioClip clip) {
        BGMSource.clip = clip;
        BGMSource.Play();
    }
}
