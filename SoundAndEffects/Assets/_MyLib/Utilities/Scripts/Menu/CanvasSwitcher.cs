using UnityEngine;
using UnityEngine.UI;

namespace GMTools.Menu
{
    [RequireComponent(typeof(Button))]
    public class CanvasSwitcher : MonoBehaviour
    {
        public CanvasName desiredCanvasType;

        CanvasManager canvasManager;
        Button menuButton;

        private void Awake()
        {
            menuButton = GetComponent<Button>();
            //menuButton.onClick.AddListener(OnButtonClicked);
            canvasManager = CanvasManager.Instance;
        }

        private void OnEnable() => menuButton.onClick.AddListener(OnButtonClicked);

        private void OnDisable() => menuButton.onClick.RemoveListener(OnButtonClicked);

        void OnButtonClicked() => canvasManager.SwitchCanvas(desiredCanvasType);
    } 
}