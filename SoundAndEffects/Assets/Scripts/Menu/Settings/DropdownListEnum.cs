using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace GMTools.Menu.Elements
{
    /* 
     * Limitation:
     *      the coresponded Enum Type must have "two values"
     *      the script must be in parent GameObject which contains the two child Toggle GameObject
     */
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ExecuteInEditMode]
    public class DropdownListEnum<T> : MonoBehaviour, IElement<T> where T : Enum
    {
        private TMP_Dropdown _dropdownOption;
        public bool DropdownListIsInit { get; private set; } = false;
        private Type _enumType = typeof(T);
        private IList _listEnumValues;

        protected virtual void Awake()
        {
            InitElement();
        }

        public virtual void InitElement()
        {
            if (!DropdownListIsInit)
            {
                _dropdownOption = GetComponent<TMP_Dropdown>();
                _listEnumValues = Enum.GetValues(_enumType);
                InitDropdownList(new List<string>(Enum.GetNames(_enumType)));
                DropdownListIsInit = true;
            }
        }

        public event Action<T> onNewValue = delegate { };

        private void InitDropdownList(List<string> dropdownOptions)
        {
            _dropdownOption.ClearOptions();
            _dropdownOption.AddOptions(dropdownOptions);
            _dropdownOption.onValueChanged.AddListener((selectedIdx) => { onNewValue.Invoke(ConvertIdxToEnum(selectedIdx)); });
        }

        private T ConvertIdxToEnum(int selectedIdx) => (T) Enum.ToObject(_enumType, _listEnumValues[selectedIdx]);

        private int ConvertEnumtoIdx(T value) => Convert.ToInt32(value);

        public virtual void SetValue(T value)
        {
            if (DropdownListIsInit)
            {
                _dropdownOption.SetValueWithoutNotify(ConvertEnumtoIdx(value));
            }
            else
                Debug.LogError($"{this} : Attemp SetValue but DropdownListEnum is not inited");
        }
    }
}

