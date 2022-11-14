using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;
using System.Collections;

namespace GMTools.Menu
{
    public enum YesNoToggleEnum : byte
    {
        No = 0,
        Yes = 1
    }
    /* 
     * Limitation:
     *      the coresponded Enum Type must have "two values", the first value will be selected as False
     *      the script must be in parent GameObject which contains the two child Toggle GameObject
     */

    /// <summary>
    /// Script for prefab of toggle group UIElements based on coresponded Enum Type (Extension UnityEngine.UI.ToggleGroup)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ExecuteInEditMode]
    public class ToggleGroupBoolEnum<T> : ToggleGroup, IElement<bool> where T : Enum
    {
        [SerializeField] private bool _trueFirstToggle = true;
        private const int TwoTogglesInGroup = 2;
        public bool ToggleGroupIsInit { get; private set; } = false;

        private bool oldValueTrueFirstToggle;
        private Toggle[] foudedToggles = new Toggle[TwoTogglesInGroup];

        protected override void OnEnable()
        {
            base.OnEnable();
            InitToggleGroup();
            oldValueTrueFirstToggle = _trueFirstToggle;
        }

        //At OnEnable() the m_Toggles may not be initiated use Coroutine or Start() or don't use the internal m_Toggles
        //protected override void Start()
        //{
        //    base.Start();
        //    //Start run once and after all OnEnable(), also the code doesn't linked with "m_Toggles.OnEnable"
        //    InitToggleGroup();
        //    oldValueTrueFirstToggle = _trueFirstToggle;
        //}

        protected override void OnValidate()
        {
            if (ToggleGroupIsInit && oldValueTrueFirstToggle != _trueFirstToggle)
            {
                ToggleGroupIsInit = false;
                InitToggleGroup();
                oldValueTrueFirstToggle = _trueFirstToggle;
            }
        }

        private void InitToggleGroup()
        {
            if (!ToggleGroupIsInit)
            {
                Debug.LogError($"{this} : InitToggleGroup()");
                int countToggles = FillArrToggles();
                if (countToggles != TwoTogglesInGroup)
                {
                    throw new NotImplementedException($"countToggles [{countToggles}] != {TwoTogglesInGroup} not implemented");
                }
                string[] enumNames = Enum.GetNames(typeof(T));
                if (enumNames.Length != TwoTogglesInGroup)
                {
                    throw new NotImplementedException($"enumValues.Length [{enumNames.Length}] != {TwoTogglesInGroup} not implemented");
                }
                int enumIdx;
                int maxIdx = TwoTogglesInGroup - 1;
                for (int i = 0; i < TwoTogglesInGroup; i++)
                {
                    Toggle currentToggle = foudedToggles[i];
                    if (_trueFirstToggle)
                        enumIdx = maxIdx - i;
                    else
                        enumIdx = i;
                    //Enough to use the event from one Toggle because they will change synchronously
                    bool rez = enumIdx == 1;
                    currentToggle.onValueChanged.AddListener((toggleIsOn) =>
                    {
                        if (toggleIsOn) onNewValue.Invoke(rez);
                    });
                    Text labelToggle = currentToggle.GetComponentInChildren<Text>();
                    if (labelToggle)
                        //labelToggle.text = Enum.GetName(enumType, list[enumValue]);
                        labelToggle.text = enumNames[enumIdx];
                    else
                        throw new NotImplementedException($"absent the label object for [{currentToggle.name}] Toggle ");
                }
                ToggleGroupIsInit = true;
            }
        }

        /// <summary>
        /// Find the Toggle and add them to myArrToggles
        /// </summary>
        /// <returns>number founded toggles</returns>
        private int FillArrToggles()
        {
            try
            {
                foudedToggles = transform.GetComponentsInChildren<Toggle>();
            }
            catch (Exception)
            {
                throw new NotImplementedException($"{this} : Can't find demdanded Toggles at initialization");
            }
            return foudedToggles.Length;
        }

        public event Action<bool> onNewValue;

        public void SetValue(bool value)
        {
            if (ToggleGroupIsInit)
            {
                foudedToggles[(_trueFirstToggle ? !value : value) ? 1 : 0].SetIsOnWithoutNotify(true); 
            }
            else
                Debug.LogError($"{this} : Attemp SetValue but ToggleGroupIsInit is not inited");
        }
    }
}
