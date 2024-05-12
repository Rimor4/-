using System;
using Cinemachine;
using UnityEngine;

public class CinemaControl : MonoBehaviour
{
    [Header("Event Listening")]
    public VoidEventSO afterSceneLoadedEvent;

    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource impulseSource;
    public VoidEventSO cameraShakeEvent;

    private void Awake() {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable() {
        // 监听并注册来自VoidEventSO广播的事件
        cameraShakeEvent.OnEventRaised += OnCameraShakeEvent;
        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoadedEvent;
    }

    private void OnDisable() {
        cameraShakeEvent.OnEventRaised -= OnCameraShakeEvent;
        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        // 场景加载后获得新边界
        GetNewCameraBounds();    
    }

    private void GetNewCameraBounds() {
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if (obj == null) 
            return;
        
        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();

        confiner2D.InvalidateCache();
    }

    private void OnCameraShakeEvent()
    {
        // 有攻击生效时产生摄像机震动
        impulseSource.GenerateImpulse();
    }
}
