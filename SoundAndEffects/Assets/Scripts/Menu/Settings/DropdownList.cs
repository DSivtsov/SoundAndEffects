using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;
using System.Linq;

namespace GMTools.Menu
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
    public class DropdownList<T> : MonoBehaviour, IElement<T> where T : ScriptableObject
    {
        private T[] _arrObjects;
        private TMP_Dropdown _dropdownOption;
        private static Dictionary<int, T> dictObjectByInt;
        private static Dictionary<string, int> dictIntByStr;
        public bool DropdownListIsInit { get; private set; } = false;

        string pathResourceAssets = "";

        private void Awake()
        {
            _dropdownOption = GetComponent<TMP_Dropdown>();
            //_arrObjects = Resources.LoadAll<T>(pathResourceAssets);
            _arrObjects = LoadArrObjects();

            dictObjectByInt = _arrObjects.Select((SO,idx) => new { SO, idx }).ToDictionary((x)=>x.idx, (x) => x.SO);
            dictIntByStr = dictObjectByInt.ToDictionary((keyPair) => keyPair.Value.name, (keyPair) => keyPair.Key);
        }

        private void OnEnable()
        {
            List<string> dropdownOptions = dictIntByStr.Keys.ToList();
            InitDropdownList(dropdownOptions);
        }

        public event Action<T> onNewValue = delegate { };

        private void InitDropdownList(List<string> dropdownOptions)
        {
            if (!DropdownListIsInit)
            {
                Debug.LogError($"{this} : InitDropdownList()");
                _dropdownOption.ClearOptions();
                _dropdownOption.AddOptions(dropdownOptions);
                _dropdownOption.onValueChanged.AddListener((selectedIdx) => { onNewValue.Invoke(dictObjectByInt[selectedIdx]); });
                //_dropdownOption.value = _initialValue;
                DropdownListIsInit = true;
            }
        }

        public void SetValue(T value)
        {
            if (DropdownListIsInit)
            {
                //_dropdownOption.value = dictIntByStr[value.name];
                _dropdownOption.SetValueWithoutNotify(dictIntByStr[value.name]);
            }
            else
                Debug.LogError($"{this} : Attemp SetValue but ToggleGroupIsInit is not inited");
        }

        T[] LoadArrObjects()
        {
            T[] arr= Resources.FindObjectsOfTypeAll<T>();
            if (arr.Length == 0)
            {
                arr = Resources.LoadAll<T>(pathResourceAssets);
            }
            //Debug.LogError($"{this} : CountComplexitySO() ComplexitySO[].Count={arr.Length} ");
            return arr;
        }

    }
}

