using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace GMTools.Menu.Elements
{
    public enum YesNoToggleEnum : byte
    {
        No = 0,
        Yes = 1
    }
    /* 
     * Limitation:
     *      the coresponded Enum Type must have "two values"
     *      the script must be in parent GameObject which contains the two child Toggle GameObject
     */

    /// <summary>
    /// Script for prefab of toggle group UIElements based on coresponded Enum Type (Extension UnityEngine.UI.ToggleGroup)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ExecuteInEditMode]
    public class ToggleGroupBoolEnum<T> : MonoBehaviour, IElement<bool> where T : Enum
    {
        [SerializeField] private T _true;
        [ReadOnly, SerializeField] private T _false;
        [SerializeField] private bool _trueFirstToggle = true;
        private const int TwoTogglesInGroup = 2;
        public bool ToggleGroupIsInit { get; private set; } = false;
        #region VariablesforValidate
        private T _oldValueTrue;
        private bool oldValueTrueFirstToggle; 
        #endregion
        private Toggle[] foudedToggles = new Toggle[TwoTogglesInGroup];
        private Array _enumValues;
        private Dictionary<bool, T> _dictBoolEnums = new Dictionary<bool, T>(2);
        private Dictionary<bool, Toggle> _dictBoolToggles = new Dictionary<bool, Toggle>(2);

        //At OnEnable() the m_Toggles may not be initiated use Coroutine or Start() or don't use the internal m_Toggles
        private void Awake()
        {
            //Start run once and after all OnEnable(), also the code doesn't linked with "m_Toggles.OnEnable"
            InitElement();
        }

        public void InitElement()
        {
            if (!ToggleGroupIsInit)
            {
                InitToggles();
                InitEnum();
                InitDictBoolValues();
                UpdateTrueFalseToggle();
                ToggleGroupIsInit = true;
            }
        }

        public event Action<bool> onNewValue;

        public void SetValue(bool value)
        {
            if (ToggleGroupIsInit)
            {
                _dictBoolToggles[value].SetIsOnWithoutNotify(true);
            }
            else
                Debug.LogError($"{this} : Attemp SetValue but ToggleGroupIsInit is not inited");
        }

        private void InitDictBoolValues()
        {
            _dictBoolEnums.Clear();
            T value0 = (T)_enumValues.GetValue(0);
            T value1 = (T)_enumValues.GetValue(1);
            if (EqualityComparer<T>.Default.Equals(value0, _true))
                SetTrueFalseEnum(value0, value1);
            else
                SetTrueFalseEnum(value1, value0);
            _oldValueTrue = _true;
            _false = _dictBoolEnums[false];
            void SetTrueFalseEnum(T trueValues, T falseValue)
            {
                _dictBoolEnums.Add(true, trueValues);
                _dictBoolEnums.Add(false, falseValue);
            }
        }

        private void InitEnum()
        {
            _enumValues = Enum.GetValues(typeof(T));
            if (_enumValues.Length != TwoTogglesInGroup)
            {
                throw new NotImplementedException($"enumValues.Length [{_enumValues.Length}] != {TwoTogglesInGroup} not implemented");
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            //base.OnValidate();
            if (ToggleGroupIsInit && oldValueTrueFirstToggle != _trueFirstToggle)
            {
                UpdateTrueFalseToggle();
            }
            if (ToggleGroupIsInit && !EqualityComparer<T>.Default.Equals(_oldValueTrue, _true))
            {
                InitDictBoolValues();
                UpdateTrueFalseToggle();
            }
        } 
#endif

        private void UpdateTrueFalseToggle()
        {
            SetTrueFalseToggle();
            oldValueTrueFirstToggle = _trueFirstToggle;
        }

        private void InitToggles()
        {
            int countToggles = FillArrToggles();
            if (countToggles != TwoTogglesInGroup)
            {
                throw new NotImplementedException($"countToggles [{countToggles}] != {TwoTogglesInGroup} not implemented");
            }
        }

        private void SetTrueFalseToggle()
        {
            int idx;
            int step;
            if (_trueFirstToggle)
            {
                idx = 0;
                step = 1;
            }
            else
            {
                idx = 1;
                step = -1;
            }
            _dictBoolToggles.Clear();
            foreach (bool value in new[] { true, false })
            {
                Toggle currentToggle = foudedToggles[idx];
                _dictBoolToggles.Add(value, currentToggle);
                bool fixValue = value;
                currentToggle.onValueChanged.AddListener((toggleIsOn) =>
                {
                    if (toggleIsOn) onNewValue?.Invoke(fixValue);
                });
                Text labelToggle = currentToggle.GetComponentInChildren<Text>();
                if (labelToggle)
                    labelToggle.text = _dictBoolEnums[value].ToString();
                else
                    throw new NotImplementedException($"absent the label object for [{currentToggle.name}] Toggle ");
                idx += step;
            }
        }

        /// <summary>
        /// Find the Toggle and add them to myArrToggles
        /// </summary>
        /// <returns>number founded toggles</returns>
        private int FillArrToggles()
        {
            foudedToggles = transform.GetComponentsInChildren<Toggle>();
            if (foudedToggles == null)
                throw new NotImplementedException($"{this} : Can't find demdanded Toggles at initialization");
            return foudedToggles.Length;
        }


    }
}
