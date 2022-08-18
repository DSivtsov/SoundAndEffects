using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConnectingToServer : MonoBehaviour
{
    [SerializeField] private GameObject groupConnectingToServer;
    [SerializeField] private TextMeshProUGUI currentStatus;
    [SerializeField] private TextMeshProUGUI currentProcess;

    private StringBuilder strProcess = new StringBuilder();
    private const string DeltaStr = ".";
    private const string InitialMsg = "Connecting to Server";
    private const string ModeOnline = "Online - Connected :";
    private const string ModeOffline = "Offline - NotConnected";
    private const string FinMsg = "Connected :";
    private const string ConnectedOK = "OK";
    private const string ConnectedError = "Error";

    private const int LenghtOneCycle = 10;
    private const float timePeriod = .15f;
    private const float maximumTimePeriod = 10f;

    private bool _connecting;
    private int _count;
    private float _startTime;

    public bool Connecting => _connecting;

    public IEnumerator CoroutineProcessConnecting(float currentMaximumPeriod = maximumTimePeriod)
    {
        _connecting = true;
        _count = 0;
        currentStatus.text = InitialMsg;
        _startTime = Time.time;
        groupConnectingToServer.SetActive(true);
        do
        {
            _count++;
            if (_count % LenghtOneCycle == 0)
            {
                strProcess = new StringBuilder(DeltaStr);
                currentProcess.text = strProcess.ToString();
            }
            else
            {
                strProcess.Append(DeltaStr);
                currentProcess.text = strProcess.ToString();
            }
            yield return new WaitForSeconds(timePeriod);
            if (Time.time -  _startTime > currentMaximumPeriod)
            {
                Result(resultOK: false);
                CountFrame.DebugLogUpdate(this, $"CoroutineProcessConnecting is canceled connection time is more than {currentMaximumPeriod}");
                yield break;
            }
        } while (_connecting);
    }

    public void Result(bool resultOK)
    {
        Debug.Log($"Loading data from Server={Time.time - _startTime}");
        _connecting = false;
        currentStatus.text = FinMsg;
        if (resultOK)
        {
            currentStatus.text = ModeOnline;
            currentProcess.text = ConnectedOK;
        }
        else
        {
            currentStatus.text = ModeOnline;
            currentProcess.text = ConnectedError;
            currentProcess.color = Color.red;
        }
    }

    private void OfflineMode()
    {
        currentStatus.text = ModeOffline;
        currentProcess.text = "";
    }
}
