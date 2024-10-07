using UnityEngine;

public class AudioDefination : MonoBehaviour
{
    // 挂载在物体上, 决定广播哪个audioclip
    public PlayAudioEventSO playAudioEvent;
    public AudioClip audioClip;
    public bool playOnEnable;

    private void OnEnable()
    {
        if (playOnEnable)
        {
            PlayAudioClip();
        }
    }

    public void PlayAudioClip()
    {
        // 不同于在OnHealthChange事件上启动，直接在方法中引发SO事件
        playAudioEvent.RaiseEvent(audioClip);
    }
}
