using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Transform playerTrans;
    public Vector3 firstPosition;

    [Header("Event Listening")]
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO firstLoadScene;

    [Header("Broadcasting")]
    public VoidEventSO afterSceneLoadedEvent;
    public FadeEventSO fadeEvent;

    [SerializeField] private GameSceneSO currentLoadedScene;
    private GameSceneSO sceneToLoad;
    private Vector3 positionToGo;
    private bool fadeScreen;
    private bool isLoading;

    public float fadeDuration;  // 渐隐渐出时间

    // TODO:
    private void Start() {
        NewGame();
    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestRequest;
    }

    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestRequest;
    }

    private void NewGame() {
        sceneToLoad = firstLoadScene;
        OnLoadRequestRequest(sceneToLoad, firstPosition, true);
    }

    /// <summary>
    /// 场景加载事件请求
    /// </summary>
    /// <param name="locationToLoad"></param>
    /// <param name="posToGo"></param>
    /// <param name="fadeScreen"></param>
    private void OnLoadRequestRequest(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        if (isLoading) {
            return;
        }

        isLoading = true;
        this.sceneToLoad = locationToLoad;
        this.positionToGo = posToGo;
        this.fadeScreen = fadeScreen;

        if (currentLoadedScene != null)
        {
            StartCoroutine(UnLoadPreviousScene());
        } else {
            LoadNewScene();
        }
    }

    private IEnumerator UnLoadPreviousScene()
    {
        if (fadeScreen)
        {
            fadeEvent.FadeIn(fadeDuration);
        }

        yield return new WaitForSeconds(fadeDuration);  // 保证渐隐渐出结束后才卸载场景
        yield return currentLoadedScene.sceneReference.UnLoadScene();   // 确保场景完全卸载之前，协程会暂停执行

        // 关闭人物
        playerTrans.gameObject.SetActive(false);

        // 加载新场景   
        LoadNewScene();
    }

    private void LoadNewScene()
    {
        var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOption.Completed += OnLoadCompleted;
    }

    /// <summary>
    /// 场景加载之后
    /// </summary>
    /// <param name="handle"></param>
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> handle)
    {
        currentLoadedScene = sceneToLoad;

        playerTrans.position = positionToGo;
        
        playerTrans.gameObject.SetActive(true);
        if (fadeScreen) {
            fadeEvent.FadeOut(fadeDuration);
        }
        
        isLoading = false;

        // 场景加载后事件
        afterSceneLoadedEvent.RaiseEvent();
    }
}

