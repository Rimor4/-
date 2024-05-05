using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class Sign : MonoBehaviour
{
    private PlayerInputControl playerInput;
    private Animator anim;
    public Transform playerTrans;
    public GameObject signSprite;
    private bool canPress;

    private void Awake() {
        anim = signSprite.GetComponent<Animator>();

        playerInput = new PlayerInputControl();
        playerInput.Enable();
    }

    private void OnEnable() {
        InputSystem.onActionChange += OnActionChange;
    }

    private void Update() {
        signSprite.GetComponent<SpriteRenderer>().enabled  = canPress;
        signSprite.transform.localScale = playerTrans.localScale; 
    }

    /// <summary>
    /// 不同控制器设备输入
    /// </summary>
    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        // // 当前激活的输入设备的名字
        // Debug.Log(((InputAction)obj).activeControl.device);
        var device = ((InputAction)obj).activeControl.device;

        switch (device) {
            case Keyboard:
                anim.Play("keyboard");
                break;
            case DualShockGamepad:
                anim.Play("ps");
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Interactive")) {
            canPress = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        canPress = false; 
    }
}
