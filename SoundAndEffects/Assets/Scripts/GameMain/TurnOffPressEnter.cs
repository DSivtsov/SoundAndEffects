using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TypeWaitMsg
{
    waitStart,
    waitContinue,
    waitEndGame
}

/// <summary>
/// Activate initial Screen "textPreesEnter"
/// </summary>
public class TurnOffPressEnter : MonoBehaviour
{
    [SerializeField] private GameObject textPreesEnter;

    private const string strStart = "Press Enter to Start";
    private const string strContinue = "Press Enter to Continue";
    private const string strEndGame = "Press Enter to Finish";
    private TextMeshProUGUI textMesh;


    private void Awake()
    {
        textMesh = textPreesEnter.GetComponent<TextMeshProUGUI>();
    }

    public void Active(bool activate, TypeWaitMsg typeMsg = TypeWaitMsg.waitStart)
    {
        if (activate)
        {
            switch (typeMsg)
            {
                case TypeWaitMsg.waitStart:
                    textMesh.text = strStart;
                    break;
                case TypeWaitMsg.waitContinue:
                    textMesh.text = strContinue;
                    break;
                case TypeWaitMsg.waitEndGame:
                    textMesh.text = strEndGame;
                    break;
            }
            textPreesEnter.SetActive(true); 
        }
        else
            textPreesEnter.SetActive(false);
    }
}
