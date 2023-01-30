using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DisplayConnectingToServer : MonoBehaviour
{
    [SerializeField] private GameObject _groupModeAndStatus;
    [SerializeField] private TextMeshProUGUI _currentMode;
    [SerializeField] private TextMeshProUGUI _currentModeStatus;

    private StringBuilder strProcess = new StringBuilder();
    private const string DeltaStr = ".";
    private const string ModeConnecting = "Connecting ";
    private const string ModeOnline = "Online mode :";
    private const string ModeOffline = "Offline mode :";
    private const string ModeStatusActive = "Active";
    private const string ModeStatusNotActive = "Not Active";

    private const int LenghtOneCycle = 10;
    private const float timePeriod = .15f;
    private int _count;

    private Coroutine _coroutineAnimateProcessConnecting;

    private void Awake() => _groupModeAndStatus.SetActive(false);

    public void StartAnimateProcessConnecting() => _coroutineAnimateProcessConnecting = StartCoroutine(CoroutineAnimateProcessConnecting());

    public IEnumerator CoroutineAnimateProcessConnecting()
    {
        _currentMode.text = ModeConnecting;
        _groupModeAndStatus.SetActive(true);
        do
        {
            _count++;
            if (_count % LenghtOneCycle == 0)
            {
                strProcess = new StringBuilder(DeltaStr);
                _currentModeStatus.text = strProcess.ToString();
            }
            else
            {
                strProcess.Append(DeltaStr);
                _currentModeStatus.text = strProcess.ToString();
            }
            yield return new WaitForSeconds(timePeriod);
        } while (true);
    }

    public void DisplayResult(bool resultOK, float startTime)
    {
        StopCoroutine(_coroutineAnimateProcessConnecting);

        CountFrame.DebugLogUpdate(this, $"Time response from Server={Time.time - startTime} resultOK[{resultOK}]");
        _currentMode.text = ModeOnline;
        if (resultOK)
        {
            _currentMode.color = Color.green;
            _currentModeStatus.text = ModeStatusActive;
            _currentModeStatus.color = Color.green;
        }
        else
        {
            _currentMode.color = Color.yellow;
            _currentModeStatus.text = ModeStatusNotActive;
            _currentModeStatus.color = Color.red;
        }
    }

    public void DisplayOfflineModeActive()
    {
        _groupModeAndStatus.SetActive(true);
        _currentMode.text = ModeOffline;
        _currentMode.color = Color.yellow;
        _currentModeStatus.text = ModeStatusActive;
        _currentModeStatus.color = Color.green;
    }

    public void SetResultsGoingToOffLineMode()
    {
        StopCoroutine(_coroutineAnimateProcessConnecting);
        DisplayOfflineModeActive();
    }
}
