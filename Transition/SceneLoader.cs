using System;
using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour, ISaveable
{
    public Transform playerTrans;
    public Vector3 firstPosition;
    public Vector3 menuPosition;

    [Header("Event Listening")]
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO newGameEventSO;
    public VoidEventSO backToMenuEventSO;

    [Header("Broadcasting")]
    public VoidEventSO afterSceneLoadedEvent;
    public FadeEventSO fadeEvent;
    public SceneLoadEventSO unloadedSceneEvent;

    [Header("Scenes")]
    public GameSceneSO firstLoadScene;
    public GameSceneSO menuScene;
    private GameSceneSO currentLoadedScene;
    private GameSceneSO sceneToLoad;

    public Vector3 positionToGo;
    private bool fadeScreen;
    private bool isLoading;

    public float fadeDuration;  // 渐隐渐出时间

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequest;
        newGameEventSO.OnEventRaised += NewGame;
        backToMenuEventSO.OnEventRaised += OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.RegiseterSaveData();
    }

    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequest;
        newGameEventSO.OnEventRaised -= NewGame;
        backToMenuEventSO.OnEventRaised -= OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.UnRegiseterSaveData();
    }

    private void OnBackToMenuEvent()
    {
        sceneToLoad = menuScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, menuPosition, true);
    }

    private void Start()
    {
        loadEventSO.RaiseLoadRequestEvent(menuScene, menuPosition, true);
    }

    private void NewGame()
    {
        sceneToLoad = firstLoadScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, firstPosition, true);
    }

    /// <summary>
    /// 场景加载事件请求
    /// </summary>
    /// <param name="locationToLoad">加载的场景引用</param>
    /// <param name="posToGo">玩家传送的位置</param>
    /// <param name="fadeScreen">是否渐入渐出</param>
    private void OnLoadRequest(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        if (isLoading)
        {
            return;
        }

        isLoading = true;
        sceneToLoad = locationToLoad;
        positionToGo = posToGo;
        this.fadeScreen = fadeScreen;

        if (currentLoadedScene != null)
        {
            StartCoroutine(UnLoadPreviousScene());
        }
        else
        {
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

        // 场景卸载时启动UI显示
        unloadedSceneEvent.RaiseLoadRequestEvent(sceneToLoad, positionToGo, true);

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
    /// 注册到loadingOption.Completed，场景加载之后调用
    /// </summary>
    /// <param name="handle"></param>
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> handle)
    {
        currentLoadedScene = sceneToLoad;

        playerTrans.position = positionToGo;

        playerTrans.gameObject.SetActive(true);
        if (fadeScreen)
        {
            fadeEvent.FadeOut(fadeDuration);
        }

        isLoading = false;

        if (currentLoadedScene.sceneType != SceneType.Menu)
            // 场景加载后事件(菜单加载完成后人物不能移动)
            afterSceneLoadedEvent.RaiseEvent();
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data)
    {
        data.SaveGameScene(currentLoadedScene);
    }

    public void LoadData(Data data)
    {
        Debug.Log("LoadSceneData " + data.sceneToSave);
        var playerID = playerTrans.GetComponent<DataDefination>().ID;
        if (data.characterPosDict.ContainsKey(playerID))
        {
            positionToGo = data.characterPosDict[playerID].ToVector3();
            sceneToLoad = data.GetSavedScene();

            OnLoadRequest(sceneToLoad, positionToGo, true);
        }
    }
}

