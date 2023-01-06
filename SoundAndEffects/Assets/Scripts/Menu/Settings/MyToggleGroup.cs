using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyToggleGroup : ToggleGroup
{

    protected override void Awake()
    {
        base.Awake();
        Debug.LogError($"{this} : Awake()");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Debug.LogError($"{this} : OnEnable()");
    }
}
