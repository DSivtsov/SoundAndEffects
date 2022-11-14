using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System;

public enum SceneName
{
    //Put only Scenes possible to Load don't put the "Loader" Scene
    //Loader = 0,
    Menus = 1,
    Game = 2
}

public class LoaderScenes : MonoBehaviour
{
    [SerializeField] private Texture[] arrLoadPictures;
    [SerializeField] private RawImage imageLoadPicture;
    [SerializeField] private Slider sliderLoad;
    [SerializeField] private TextMeshProUGUI textAphorism;
    [SerializeField] private GameObject loaderCameraObj;
    [SerializeField] private GameObject loaderCanvas;
    [Tooltip("Load all after that start the first scene otherwise start the first scene after it will be loaded")]
    [SerializeField] private bool loadedAllAfterStartFirst = true;
    [Tooltip("Work only with \"Loaded All After Start First\"")]
    [SerializeField] private bool activateOtherAfterFirst = true;
    [Header("Scenes loading Order")]
    [SerializeField] private SceneName[] loadOrder;

    public bool AllScenesLoaded { get; private set; }
    private System.Random random  = new System.Random();
    private AsyncOperation[] asyncOperations;
    private int numberOperations;
    private void Awake()
    {
        imageLoadPicture.texture = arrLoadPictures[random.Next(0, arrLoadPictures.Length)];
        sliderLoad.value = 0;
        textAphorism.text = AphorismText.GetStrRandomAphorismText();
        numberOperations = loadOrder.Length;
        asyncOperations = new AsyncOperation[numberOperations];
    }

    public void LoadScenes()
    {
        if (loadedAllAfterStartFirst)
            LoadAllAfterStartFirst();
        else
            StartAfterFirstLoaded();
    }

    private void OnValidate()
    {
        if (loadedAllAfterStartFirst != true)
        {
            activateOtherAfterFirst = false;
        }
    }

    //Start the first scene after it will be loaded
    private void StartAfterFirstLoaded()
    {
        AllScenesLoaded = false;
        StartAsyncLoad();
        StartCoroutine(StartFirstLoadedScene());
    }

    //Load all scenes after that start the first scene
    private void LoadAllAfterStartFirst()
    {
        AllScenesLoaded = false;
        StartCoroutine(StartAfterLoadedAllScenes());
    }
    //Use only from StartAfterFirstLoaded()
    private void StartAsyncLoad(bool allowActivation = true)
    {
        for (int i = 0; i < numberOperations; i++)
        {
            asyncOperations[i] = SceneManager.LoadSceneAsync((int)loadOrder[i], LoadSceneMode.Additive);
            asyncOperations[i].allowSceneActivation = allowActivation;
        }
    }

    /// <summary>
    /// Loading progress indicator never reaches 100%, because the first Scene will start early and overlap it before other Scenes will be fully loaded.
    /// It made one "yield return null" after load
    /// </summary>
    /// <returns></returns>
    IEnumerator StartFirstLoadedScene()
    {
        //Debug.Log($"{this} [{CountFrame.currentNumFrame}] [{this.gameObject.scene.name}] StartFirstLoadedScene()");
        yield return null;
        float totalProgress;
        bool totalIsDone;
        do
        {
            //Debug.Log($"{this} [{CountFrame.currentNumFrame}] [{this.gameObject.scene.name}] StartFirstLoadedScene() : LoadCycle");
            totalProgress = 0;
            totalIsDone = true;
            for (int i = 0; i < numberOperations; i++)
            {
                AsyncOperation currentOperation = asyncOperations[i];
                totalProgress += currentOperation.progress;
                totalIsDone &= currentOperation.isDone;
                //Debug.Log($"[{i}] currentOperation={currentOperation.progress} totalIsDone={totalIsDone}");
            }
            //Debug.Log($"sum={totalProgress} {(Mathf.Clamp01(totalProgress / (numberOperations)) * 100):F2}%");
            totalProgress = Mathf.Clamp01(totalProgress / (numberOperations));
            sliderLoad.value = totalProgress;
            yield return null;
        } while (!totalIsDone);
        AllScenesLoadedActivated();
    }

    /// <summary>
    /// Loading progress indicator always reaches 100%, only after that will activated the first Scene and other Scenes after.
    /// It made one "yield return null" after load
    /// </summary>
    /// <returns></returns>
    IEnumerator StartAfterLoadedAllScenes()
    {
        //Debug.Log($"{this} [{CountFrame.currentNumFrame}] [{this.gameObject.scene.name}] StartAfterLoadedAllScenes()");
        //Unity Bug https://issuetracker.unity3d.com/issues/loadsceneasync-allowsceneactivation-flag-is-ignored-in-awake
        yield return null;
        //Don't Activate Scenes after load
        StartAsyncLoad(false);
        float totalProgress;
        do
        {
            //Debug.Log($"{this} [{CountFrame.currentNumFrame}] [{this.gameObject.scene.name}] StartAfterLoadedAllScenes() : LoadCycle");
            totalProgress = 0;
            for (int i = 0; i < numberOperations; i++)
            {
                AsyncOperation currentOperation = asyncOperations[i];
                totalProgress += currentOperation.progress;
                //Debug.Log($"[{i}] currentOperation={currentOperation.progress} currentOperation={currentOperation.isDone}");
            }
            //Debug.Log($"sum={totalProgress} {(Mathf.Clamp01(totalProgress / (.9f * numberOperations)) * 100):F2}%");
            totalProgress = Mathf.Clamp01(totalProgress / (.9f * numberOperations));
            sliderLoad.value = totalProgress;
            yield return null;
        } while (totalProgress != 1);
        //All scenes are loaded but not Acivated
        ActivateFirstScene();
        if (activateOtherAfterFirst)
        {
            ActivateOtherScene(); 
        }
        AllScenesLoadedActivated();
    }

    private void ActivateFirstScene()
    {
        asyncOperations[0].allowSceneActivation = true;
    }

    private void ActivateOtherScene()
    {
        for (int i = 1; i < numberOperations; i++)
            asyncOperations[i].allowSceneActivation = true;
    }

    private void AllScenesLoadedActivated()
    {
        //CountFrame.DebugLogUpdate(this, $"AllScenesLoadedActivated()");
        if (loadedAllAfterStartFirst)
            StartCoroutine(WaitAfterLoad());
        else
            LoadingFinished();
    }

    //Wait in Case of use the loadedAllAfterStartFirst
    private IEnumerator WaitAfterLoad()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        LoadingFinished();
    }

    private void LoadingFinished()
    {
        AllScenesLoaded = true;
        MainManager.Instance.AllScenesLoaded();
    }

    public IEnumerator UnLoadScenes()
    {
        CountFrame.DebugLogUpdate(this, $"UnLoadScenes()");
        bool waitUnloadScene = false;
        List<AsyncOperation> asyncUnLoadOperations = new List<AsyncOperation>(asyncOperations.Length);
        
        for (int i = 0; i < numberOperations; i++)
        {
            int sceneBuildIdx = (int)loadOrder[i];
            //Debug.Log($"[{i}] [{loadOrder[i]}] loaded={SceneManager.GetSceneByBuildIndex(sceneBuildIdx).isLoaded}");
            if (SceneManager.GetSceneByBuildIndex(sceneBuildIdx).isLoaded)
            {
                waitUnloadScene = true;
                asyncUnLoadOperations.Add(SceneManager.UnloadSceneAsync(sceneBuildIdx, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects));
            }
        }
        //Debug.Log($"asyncUnLoadOperations.Count={asyncUnLoadOperations.Count}");
        if (waitUnloadScene)
        {
            bool finish;
            do
            {
                yield return null;
                finish = true;
                for (int i = 0; i < asyncUnLoadOperations.Count; i++)
                {
                    finish &= asyncUnLoadOperations[i].isDone;
                }
            } while (!finish);
        }
    }
}