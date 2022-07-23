using UnityEngine;
using UnityEngine.UI;

namespace GMTools.Menu
{

    [RequireComponent(typeof(Button))]
    public class ButtonController : MonoBehaviour
    {
        [SerializeField] private ButtonType buttonType;
        private Button menuButton;

        private void Awake()
        {
            menuButton = GetComponent<Button>();
        }

        private void OnEnable()
        {
            menuButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            menuButton.onClick.RemoveListener(OnButtonClicked);
        }

        void OnButtonClicked()
        {
            buttonType.ButtonPressed();
        }
    }
}