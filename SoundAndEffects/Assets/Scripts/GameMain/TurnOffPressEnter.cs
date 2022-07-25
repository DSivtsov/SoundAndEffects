using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TypeMsg
{
    Start,
    Continue,
    EndGame
}

/// <summary>
/// Activate initial Screen "textPreesEnter"
/// </summary>
public class TurnOffPressEnter : MonoBehaviour
{
    public GameObject textPreesEnter;

    private const string strStart = "Press Enter to Start";
    private const string strContinue = "Press Enter to Continue";
    private const string strEndGame = "Press Enter to Finish";
    private TextMeshProUGUI textMesh;


    private void Awake()
    {
        textMesh = textPreesEnter.GetComponent<TextMeshProUGUI>();
    }

    public void Active(bool value, TypeMsg typeMsg = TypeMsg.Start)
    {
        if (value)
        {
            switch (typeMsg)
            {
                case TypeMsg.Start:
                    textMesh.text = strStart;
                    break;
                case TypeMsg.Continue:
                    textMesh.text = strContinue;
                    break;
                case TypeMsg.EndGame:
                    textMesh.text = strEndGame;
                    break;
            }
            textPreesEnter.SetActive(true); 
        }
        else
            textPreesEnter.SetActive(false);
    }
}
