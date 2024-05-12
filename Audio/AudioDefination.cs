using UnityEngine;

public class AudioDefination : MonoBehaviour
{
    // 挂载在物体上, 决定广播哪个audioclip
    public PlayAudioEventSO playAudioEvent;
    public AudioClip audioClip;
    public bool playOnEnable;

    private void OnEnable() {
        if (playOnEnable) {
            // BGM需要立即播放
            PlayAudioClip();
        }
    }

    public void PlayAudioClip() {
        // 不需要 "+="
        playAudioEvent.RaiseEvent(audioClip);    
    }
}
