using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

enum SceneName
{
    //Put only Scenes possible to Load don't put the "Loader" Scene
    //Loader = 0,
    Menus = 1,
    Game = 2
}

public class LoaderManager : MonoBehaviour
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
    [Header("Demo Option")]
    [SerializeField] private float pauseBeforeStart = 3f;

    public bool AllScenesLoaded { get; private set; }
    private System.Random random  = new System.Random();
    private AsyncOperation[] asyncOperations;
    private int numberOperations;
    private string[] arrAphorism;
    private void Awake()
    {
        imageLoadPicture.texture = arrLoadPictures[random.Next(0, arrLoadPictures.Length)];
        sliderLoad.value = 0;
        arrAphorism = AphorismText.GetArrAphorismTex();
        textAphorism.text = arrAphorism[random.Next(0, arrAphorism.Length)];

        numberOperations = loadOrder.Length;
        asyncOperations = new AsyncOperation[numberOperations];

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
    /// Loading progress indicator never reaches 100%, because the first Scene will start early and overlap it before other Scenes will be fully loaded
    /// </summary>
    /// <returns></returns>
    IEnumerator StartFirstLoadedScene()
    {
        float totalProgress;
        bool totalIsDone;
        do
        {
            totalProgress = 0;
            totalIsDone = true;
            for (int i = 0; i < numberOperations; i++)
            {
                AsyncOperation currentOperation = asyncOperations[i];
                totalProgress += currentOperation.progress;
                totalIsDone &= currentOperation.isDone;
                Debug.Log($"[{i}] currentOperation={currentOperation.progress} totalIsDone={totalIsDone}");
            }
            Debug.Log($"sum={totalProgress} {Mathf.Clamp01(totalProgress / (.9f * numberOperations))}");
            totalProgress = Mathf.Clamp01(totalProgress / (.9f * numberOperations));
            sliderLoad.value = totalProgress;
            yield return null;
        } while (!totalIsDone);
        AllScenesLoadedActivated();
        //AllScenesLoaded = true;
    }

    /// <summary>
    /// Loading progress indicator always reaches 100%, only after that will activated the first Scene and other Scenes after 
    /// </summary>
    /// <returns></returns>
    IEnumerator StartAfterLoadedAllScenes()
    {
        //Unity Bug https://issuetracker.unity3d.com/issues/loadsceneasync-allowsceneactivation-flag-is-ignored-in-awake
        yield return null;
        //for (int i = 0; i < numberOperations; i++)
        //{
        //    asyncOperations[i] = SceneManager.LoadSceneAsync((int)loadOrder[i], LoadSceneMode.Additive);
        //    asyncOperations[i].allowSceneActivation = false;
        //}
        //Don't Activate Scenes after load
        StartAsyncLoad(false);
        float totalProgress;
        do
        {
            totalProgress = 0;
            for (int i = 0; i < numberOperations; i++)
            {
                AsyncOperation currentOperation = asyncOperations[i];
                totalProgress += currentOperation.progress;
                //Debug.Log($"[{i}] currentOperation={currentOperation.progress} currentOperation={currentOperation.isDone}");
            }
            //Debug.Log($"sum={totalProgress} {Mathf.Clamp01(totalProgress / (.9f * numberOperations))}");
            totalProgress = Mathf.Clamp01(totalProgress / (.9f * numberOperations));
            sliderLoad.value = totalProgress;
            yield return null;
        } while (totalProgress != 1);
#if UNITY_EDITOR
        //Demo Only
        yield return new WaitForSeconds(pauseBeforeStart);
#endif
        //All scenes are loaded but not Acivated
        ActivateFirstScene();
        if (activateOtherAfterFirst)
        {
            ActivateOtherScene(); 
        }
        AllScenesLoadedActivated();
        //AllScenesLoaded = true;
        //loaderCameraObj.SetActive(false);
        //loaderCanvas.SetActive(false);
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
        AllScenesLoaded = true;
        GameMainManager.Instance.AllScenesLoaded();
    }
}