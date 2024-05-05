using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDefination : MonoBehaviour
{
    // 挂载在物体上, 决定广播哪个audioclip
    public PlayAudioEventSO playAudioEvent;
    public AudioClip audioClip;
    public bool playOnEnable;

    private void OnEnable() {
        if (playOnEnable) {
            PlayAudioClip();
        }
    }

    private void PlayAudioClip() {
        // 不需要 "+="
        playAudioEvent.RaiseEvent(audioClip);    
    }
}
