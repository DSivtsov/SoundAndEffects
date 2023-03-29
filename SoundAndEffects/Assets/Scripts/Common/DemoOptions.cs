using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoOptions : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] bool _activateDemoOptions = false;
    [Header("Demo Options")]
    [Tooltip("If value not zerro, the CharacterHealth will be override by it, works only in Editor")]
    [SerializeField] private int _characterHealth;
    void Awake()
    {
        if (_activateDemoOptions)
        {
            Debug.LogWarning($"{this} : Will use Demo options");
            MainManager.Instance.OverrideCharacterHealth(overrideCharacterHealth: _characterHealth);
        }
    }
#endif
}
