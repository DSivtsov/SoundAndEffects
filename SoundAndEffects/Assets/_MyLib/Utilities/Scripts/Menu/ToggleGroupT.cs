using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;
using System.Collections;

namespace GMTools.Menu
{
    [ExecuteInEditMode]
    public class ToggleGroup<T> : ToggleGroup where T : Enum
    {
        private const int _numTogglesInGroup = 2;
        private Dictionary<T, Toggle> _dictToggle = new Dictionary<T, Toggle>();
        public bool ElementIsInit { get; private set; } = false;

        protected override void Awake()
        {
            base.Awake();
            //ElementIsInit = false;
            //Debug.Log($"{this} : Awake() ElementIsInitialized[{ElementIsInit}]");
        }

        private Toggle[] myArrToggles = new Toggle[_numTogglesInGroup];
        private const string _baseNameToggle = "Toggle";

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!ElementIsInit)
                InitToggleGroup();
        }

        //Ay OnEnable() the m_Toggles may not be initiated use Coroutine or Start() or don't use the internal m_Toggles
        protected override void Start()
        {
            base.Start();
            // Redundant checks, because Start run once and after all OnEnable() and our code doesn't linked with OnEnable
            //if (!ElementIsInitialized)
            //    if (m_Toggles.Count == _numTogglesInGroup)
            //        InitToggleGroup();
        }

        public event Action<T> eventActivatedToggle = delegate { };

        private void InitToggleGroup()
        {
           if (!ElementIsInit)
            {
                int countToggles = FillArrToggles();
                if (countToggles != _numTogglesInGroup)
                {
                    throw new NotImplementedException($"countToggles [{countToggles}] != {_numTogglesInGroup} not implemented");
                }

                Array enumValues = Enum.GetValues(typeof(T));

                if (enumValues.Length != _numTogglesInGroup)
                {
                    throw new NotImplementedException($"enumValues.Length [{enumValues.Length}] != {_numTogglesInGroup} not implemented");
                }
                IList list = enumValues;
                Type enumType = typeof(T);
                bool noError = true;
                for (int i = 0; i < _numTogglesInGroup; i++)
                {
                    T value = (T)Enum.ToObject(enumType, list[i]);
                    Toggle currentToggle = myArrToggles[i];
                    //currentToggle.onValueChanged.AddListener((response) => Debug.Log($"{_baseNameToggle + value} : {response}"));
                    //currentToggle.onValueChanged.AddListener((response) => { if (response) Debug.Log($"{_baseNameToggle + value}"); });
                    currentToggle.onValueChanged.AddListener((toggleIsOn) => { if (toggleIsOn) eventActivatedToggle.Invoke(value); });
                    _dictToggle.Add(value, currentToggle);
                     Text labelToggle = currentToggle.GetComponentInChildren<Text>();
                    if (labelToggle)
                        labelToggle.text = Enum.GetName(enumType, list[i]);
                    else
                        noError = false;
                    //Debug.Log($"i={i} {value}");
                }
                if (noError)
                {
                    //Enough to use the event from one Toggle because they will change synchronously
                    //myArrToggles[0].onValueChanged.AddListener((response) => eventChange.Invoke(response));
                    ElementIsInit = true;
                }
                else
                {
                    throw new NotImplementedException($"noError = false not implemented");
                }
                Debug.Log($"{this} : ElementIsInitialized = true [{ElementIsInit}]");
            }
        }

        public T GetValue()
        {
            Toggle activeToggle = GetFirstActiveToggle();
            foreach (var item in _dictToggle)
            {
                if (item.Value == activeToggle)
                    return item.Key;
            }
            throw new NotImplementedException($"Not found the Toggle in Dictonary<T, Toggle> not implemented");
        }

        public void SetValueWithoutNotify(T key)
        {
            Debug.Log($"key = {(T)key}");
            //_dictToggle[key].isOn = true;
            _dictToggle[key].SetIsOnWithoutNotify(true);
        }

        private int FillArrToggles()
        {
            int countToggles = 0;
            try
            {
                while (countToggles < myArrToggles.Length)
                {
                    Toggle toggle = transform.Find(_baseNameToggle + countToggles).GetComponent<Toggle>();
                    if (toggle)
                    {
                        myArrToggles[countToggles] = toggle;
                        countToggles++;
                    }
                }
            }
            catch (Exception)
            {
                throw new NotImplementedException($"{this} : Can't find demdanded Toggles at initialization");
            }
            return countToggles;
        }
        /// <summary>
        /// The Underlaying type of enum may not be int
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="newInt"></param>
        /// <returns></returns>
        private bool ConvertAnyEnumUnderlayingTypeToInt32(object obj, out int newInt)
        {
            newInt = 0;
            bool result;
            try
            {
                newInt = Convert.ToInt32(obj);
                result = true;
            }
            catch (Exception)
            {
                Debug.LogError("Cann't convert");
                result = false;
            }
            return result;
        }
    } 
}