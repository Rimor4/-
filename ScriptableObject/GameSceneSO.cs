using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Game Scene/GameSceneSO")]
public class GameSceneSO : ScriptableObject
{
    // 引用场景资源
    public AssetReference sceneReference;
    public SceneType sceneType;
}