using System;
using UnityEngine;

namespace GMTools.Menu.Elements
{
    public class ValueableSliderInt : MonoBehaviour, IElement<int>
    {
        private UnityEngine.UI.Slider _slider;
        private bool _sliderIntIsInited = false;

        protected virtual void Awake()
        {
            InitElement();
        }

        public event Action<int> onNewValue;

        public virtual void InitElement()
        {
            if (!_sliderIntIsInited)
            {
                _slider = GetComponent<UnityEngine.UI.Slider>();
                _slider.wholeNumbers = true;
                _slider.onValueChanged.AddListener((floatValue) => onNewValue.Invoke((int)floatValue));
                //_slider.onValueChanged.AddListener((floatValue) => onNewValue.Invoke((int)Mathf.Round(Mathf.Log10(floatValue) * 20)));
                _sliderIntIsInited = true;
            }
        }

        public virtual void SetValue(int value)
        {
            if (_sliderIntIsInited)
            {
                _slider.SetValueWithoutNotify(value);
            }
            else
                Debug.LogError($"{this} : Attemp SetValue but Slider is not inited");
        }
    } 
}