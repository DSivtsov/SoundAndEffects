using UnityEngine;
using TMPro;

/// <summary>
/// Activate initial Screen "textPreesEnter"
/// </summary>
public class GameMessages : MonoBehaviour
{
    [SerializeField] private GameObject textTop;
    [SerializeField] private GameObject textBottom;

    private const string strStart = "Press any Key to Start";
    private const string strContinue = "Press any Key to Continue";
    //private const string strEndGame = "Press any Key to Finish";
    private TextMeshProUGUI textMeshBottom;


    private void Awake()
    {
        textMeshBottom = textBottom.GetComponent<TextMeshProUGUI>();
        textBottom.SetActive(false);
        textTop.SetActive(false);
    }

    public void Active(bool activate, WaitType waitType)
    {
        switch (waitType)
        {
            case WaitType.waitStart:
                textMeshBottom.text = strStart;
                textBottom.SetActive(activate);
                break;
            case WaitType.waitContinueGame:
                textMeshBottom.text = strContinue;
                textBottom.SetActive(activate);
                break;
            case WaitType.waitEndGame:
                //textMeshBottom.text = strEndGame;
                textTop.SetActive(activate);
                break;
        }
    }
}
