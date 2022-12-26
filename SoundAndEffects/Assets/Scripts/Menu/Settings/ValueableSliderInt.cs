using System;
using UnityEngine;

namespace GMTools.Menu.Elements
{
    public class ValueableSliderInt : MonoBehaviour, IElement<int>
    {
        private UnityEngine.UI.Slider _slider;
        public bool SliderIsInit { get; private set; } = false;

        private void Awake()
        {
            InitElement();
        }

        public event Action<int> onNewValue;

        public void InitElement()
        {
            if (!SliderIsInit)
            {
                _slider = GetComponent<UnityEngine.UI.Slider>();
                _slider.wholeNumbers = true;
                _slider.onValueChanged.AddListener((floatValue) => onNewValue.Invoke((int)floatValue));
                //_slider.onValueChanged.AddListener((floatValue) => onNewValue.Invoke((int)Mathf.Round(Mathf.Log10(floatValue) * 20)));
                SliderIsInit = true;
            }
        }

        public void SetValue(int value)
        {
            if (SliderIsInit)
            {
                _slider.value = value;
            }
            else
                Debug.LogError($"{this} : Attemp SetValue but Slider is not inited");
        }
    } 
}