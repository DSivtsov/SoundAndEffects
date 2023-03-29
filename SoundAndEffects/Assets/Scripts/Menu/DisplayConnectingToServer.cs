using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
/*
 * Service class to manager Displaying of status connection to Server at MainMenu Canvas and at TopList Canvas
 */
[Serializable]
public class ModeOperationArray
{
    [Serializable]
    public class ModeOperationGroup
    {
        public TextMeshProUGUI mode;
        public TextMeshProUGUI modeStatus;
        public TextMeshProUGUI operation;
        public TextMeshProUGUI operationStatus;
    }

    [SerializeField] private ModeOperationGroup[] groups;
    [SerializeField] private CanvasName[] canvasNames;

    private Dictionary<CanvasName, int> dictGroups;

    public void InitModeOperationArray()
    {
        dictGroups = new Dictionary<CanvasName, int>(canvasNames.Length);
        for (int i = 0; i < canvasNames.Length; i++)
        {
            dictGroups.Add(canvasNames[i], i);
            SetActiveOperationStatus(groups[i], activeStatus: false);
        }
    }

    public void SetActiveOperationStatus(ModeOperationGroup group, bool activeStatus)
    {
        group.operation.gameObject.SetActive(activeStatus);
        group.operationStatus.gameObject.SetActive(activeStatus);
    }

    public ModeOperationGroup this[CanvasName name]
    {
        get => groups[dictGroups[name]];
    }

    public void SetModeStatus(string mode, Color modeColor, string modeStatus, Color modeStatusColor)
    {
        for (int i = 0; i < canvasNames.Length; i++)
        {
            ModeOperationGroup group = groups[i];
            group.mode.text = mode;
            group.mode.color = modeColor;
            group.modeStatus.text = modeStatus;
            group.modeStatus.color = modeStatusColor; 
        }
    }
}

public enum ServerOperationType
{
    Connecting = 1,
    Loading = 2,
    Saving = 3,
    Disconnecting = 4,
}


public class DisplayConnectingToServer : MonoBehaviour
{
    [SerializeField] private ModeOperationArray _moArr;
    [SerializeField] private GameObject _btnTryRecconect;

    private const string DeltaStr = ".";
    private const string ModeOnline = "Online mode :";
    private const string ModeOffline = "Offline mode :";
    private const string ModeStatusActive = "Active";
    private const string ModeStatusNotActive = "Not Active";

    private const int LenghtOneCycle = 10;
    private const float timePeriod = .15f;
    private Coroutine _coroutineAnimateProcessConnecting;
    private CanvasName _currentCanvasName;
    private ServerOperationType _currentOperationType;

    private void Awake()
    {
        _moArr.InitModeOperationArray();
        _currentCanvasName = CanvasName.MainMenu;
    }

    public void StartAnimateProcessConnecting(CanvasName name, ServerOperationType operationType)
    {
        _currentCanvasName = name;
        _currentOperationType = operationType;
        _coroutineAnimateProcessConnecting = StartCoroutine(CoroutineAnimateProcessConnecting());
    }

    public IEnumerator CoroutineAnimateProcessConnecting()
    {
        _moArr.SetActiveOperationStatus(_moArr[_currentCanvasName], activeStatus: true);
        _moArr[_currentCanvasName].operation.text = _currentOperationType.ToString();
        TextMeshProUGUI operationStatus = _moArr[_currentCanvasName].operationStatus;
        int _count = 0;
        StringBuilder strProcess = new StringBuilder();
        do
        {
            if (_count++ % LenghtOneCycle == 0)
                strProcess.Clear();
            strProcess.Append(DeltaStr);
            operationStatus.text = strProcess.ToString();
            yield return new WaitForSeconds(timePeriod);
        } while (true);
    }

    //In SwitchToOffline successResult = true always
    public void DisplayResultAfterSwitchToOffline(float startTime) => DisplayResultAfterFinishOperation(PlayMode.Offline, startTime);
    public void DisplayResultOperationInOnLine(float startTime, bool successResult)
        => DisplayResultAfterFinishOperation(PlayMode.Online, startTime, successResult);

    private void DisplayResultAfterFinishOperation(PlayMode currentPlayMode, float startTime, bool successResult = true)
    {
        StopCoroutine(_coroutineAnimateProcessConnecting);
        _moArr.SetActiveOperationStatus(_moArr[_currentCanvasName], activeStatus: false);

        if (currentPlayMode == PlayMode.Online)
        {
            CountFrame.DebugLogUpdate(this, $"Time response from Server={Time.time - startTime} Operation[{_currentOperationType}] Success[{successResult}]");
            if (successResult)
            {
                _moArr.SetModeStatus(ModeOnline, Color.green, ModeStatusActive, Color.green);
                ActivateButtonTryReconnect(activate: false);
            }
            else
            {
                _moArr.SetModeStatus(ModeOnline, Color.yellow, ModeStatusNotActive, Color.red);
                ActivateButtonTryReconnect();
            } 
        }
        else
        {
            CountFrame.DebugLogUpdate(this, $"Time response from Server={Time.time - startTime} Operation[{ServerOperationType.Disconnecting}]");
            DisplayOfflineModeActive();
            ActivateButtonTryReconnect(activate: false);
        }
    }

    public void DisplayOfflineModeActive() => _moArr.SetModeStatus(ModeOffline, Color.yellow, ModeStatusActive, Color.green);

    public void DisplayOnineModeNotActive() => _moArr.SetModeStatus(ModeOnline, Color.yellow, ModeStatusNotActive, Color.green);
    private void ActivateButtonTryReconnect(bool activate = true) => _btnTryRecconect.SetActive(activate);
}
