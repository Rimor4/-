using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour, IInteractable
{
    public SceneLoadEventSO loadEventSO;
    public Vector3 positionToGO;
    public GameSceneSO sceneToGO;

    public void TriggerAction()
    {
        // 呼叫者
        loadEventSO.RaiseLoadRequestEvent(sceneToGO, positionToGO, true);
    }
}
