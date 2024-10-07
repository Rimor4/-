using UnityEngine;

public class TeleportPoint : MonoBehaviour, IInteractable
{
    public SceneLoadEventSO loadEventSO;
    public Vector3 positionToGO;
    public GameSceneSO sceneToGO;

    public void TriggerAction()
    {
        // 类似AudioDefination
        loadEventSO.RaiseLoadRequestEvent(sceneToGO, positionToGO, true);
    }
}
