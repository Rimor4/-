using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemaControl : MonoBehaviour
{
    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource impulseSource;
    public VoidEventSO cameraShakeEvent;

    private void Awake() {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable() {
        // 监听并注册来自VoidEventSO广播的事件
        cameraShakeEvent.OnEventRaised += OnCameraShakeEvent;
    }
    private void OnDisable() {
        cameraShakeEvent.OnEventRaised -= OnCameraShakeEvent;
    }

    // 场景切换后更改
    private void Start() {
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
