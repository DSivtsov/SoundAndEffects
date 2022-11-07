using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoOptions : MonoBehaviour
{
    [SerializeField] bool _turnOffInBuild = true;
    [SerializeField] bool _activate = false;
    [Header("Demo Options")]
    [Tooltip("If value not zerro, the CharacterHealth will be override by it, works only in Editor")]
    [SerializeField] private int _characterHealth;
    // Start is called before the first frame update
    void Awake()
    {
#if !UNITY_EDITOR
        if (_turnOffInBuild)
           _activate = false;
#endif
        if (_activate)
        {
            Debug.LogWarning($"{this} : Demo options activated");
            MainManager.Instance.SetCharacterHelath(_characterHealth);
        }
    }
}
