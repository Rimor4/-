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
    private IInteractable targetItem;
    private bool canPress;

    private void Awake() {
        anim = signSprite.GetComponent<Animator>();

        playerInput = new PlayerInputControl();
        playerInput.Enable();
    }

    private void OnEnable() {
        InputSystem.onActionChange += OnActionChange;
        playerInput.Gameplay.Confirm.started += OnConfirm;
    }

    private void OnDisable() {
        canPress = false;        
    }

    private void Update() {
        signSprite.GetComponent<SpriteRenderer>().enabled  = canPress;
        signSprite.transform.localScale = playerTrans.localScale; 
    }

    private void OnConfirm(InputAction.CallbackContext context)
    {
        if (canPress) {
            targetItem.TriggerAction();
            GetComponent<AudioDefination>()?.PlayAudioClip();
        }
    }

    /// <summary>
    /// 不同控制器设备输入
    /// </summary>
    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        // // 当前激活的输入设备的名字
        // Debug.Log(((InputAction)obj).activeControl.device);
        if (obj is InputAction action) {
            var device = action.activeControl.device;

            switch (device) {
                case Keyboard:
                    anim.Play("keyboard");
                    break;
                case DualShockGamepad:
                    anim.Play("ps");
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Interactable")) {
            canPress = true;
            targetItem = other.GetComponent<IInteractable>();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        canPress = false; 
    }
}
