using UnityEngine;
using UnityEngine.UI;

namespace GMTools.Menu
{
    [RequireComponent(typeof(Button))]
    public class CanvasSwitcher : MonoBehaviour
    {
        [SerializeField] private CanvasName desiredCanvasType;
        //private CanvasManager canvasManager;
        //private Button menuButton;

        private void Awake()
        {
            //menuButton = GetComponent<Button>();
            //canvasManager = CanvasManager.Instance;
            GetComponent<Button>().onClick.AddListener(() => CanvasManager.Instance.SwitchCanvas(desiredCanvasType));
        }
        //private void OnEnable() => menuButton.onClick.AddListener(OnButtonClicked);
        //private void OnDisable() => menuButton.onClick.RemoveListener(OnButtonClicked);
        //void OnButtonClicked() => canvasManager.SwitchCanvas(desiredCanvasType);
    } 
}