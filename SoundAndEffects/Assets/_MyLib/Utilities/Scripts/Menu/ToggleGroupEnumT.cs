using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;
using System.Collections;

namespace GMTools.Menu.Elements
{
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
    public class ToggleGroupEnum<T> : MonoBehaviour, IElement<T> where T : Enum
    {
        private const int TwoTogglesInGroup = 2;
        private Dictionary<T, Toggle> _dictToggle = new Dictionary<T, Toggle>();
        private Dictionary<T, T> _dictOpositeValues = new Dictionary<T, T>();
        public bool ToggleGroupIsInit { get; private set; } = false;

        private Toggle[] myArrToggles = new Toggle[TwoTogglesInGroup];

        //At OnEnable() the m_Toggles may not be initiated use Coroutine or Start() or don't use the internal m_Toggles
        protected void Awake()
        {
            //Start run once and after all OnEnable(), also the code doesn't linked with "m_Toggles.OnEnable"
            InitElement();
        }

        public void InitElement()
        {
            if (!ToggleGroupIsInit)
            {
                InitToggleGroup();
                ToggleGroupIsInit = true;
            }
        }
        
        public event Action<T> onNewValue = delegate { };

        public virtual void SetValue(T value)
        {
            if (ToggleGroupIsInit)
            {
                //Use the method of Toggle which doesn't know regarding about other toggles
                _dictToggle[value].SetIsOnWithoutNotify(true);
                _dictToggle[_dictOpositeValues[value]].SetIsOnWithoutNotify(false);
            }
            else
                Debug.LogError($"{this} : Attemp SetValue but ToggleGroupIsInit is not inited");
        }

        private void InitToggleGroup()
        {
            int countToggles = FillArrToggles();
            if (countToggles != TwoTogglesInGroup)
            {
                throw new NotImplementedException($"countToggles [{countToggles}] != {TwoTogglesInGroup} not implemented");
            }

            Array enumValues = Enum.GetValues(typeof(T));

            if (enumValues.Length != TwoTogglesInGroup)
            {
                throw new NotImplementedException($"enumValues.Length [{enumValues.Length}] != {TwoTogglesInGroup} not implemented");
            }
            IList listEnumValues = enumValues;
            Type enumType = typeof(T);
            for (int i = 0; i < TwoTogglesInGroup; i++)
            {
                T value = (T)Enum.ToObject(enumType, listEnumValues[i]);
                _dictOpositeValues.Add(value, (T)Enum.ToObject(enumType, listEnumValues[(i == 0) ? 1 : 0]));
                Toggle currentToggle = myArrToggles[i];
                //Enough to use the event from one Toggle because they will change synchronously
                currentToggle.onValueChanged.AddListener((toggleIsOn) => { if (toggleIsOn) onNewValue.Invoke(value); });
                _dictToggle.Add(value, currentToggle);
                    Text labelToggle = currentToggle.GetComponentInChildren<Text>();
                if (labelToggle)
                    labelToggle.text = Enum.GetName(enumType, listEnumValues[i]);
                else
                    throw new NotImplementedException($"absent the label object for [{value}] Toggle ");
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
                myArrToggles = transform.GetComponentsInChildren<Toggle>();
            }
            catch (Exception)
            {
                throw new NotImplementedException($"{this} : Can't find demdanded Toggles at initialization");
            }
            return myArrToggles.Length;
        }
    } 
}
