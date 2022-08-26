using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestToggles : MonoBehaviour
{
    public int numToggles;
    public Toggle[] arrTogle;
    public ToggleGroup toggleGroup;

    private int prevNumToggles;
    private void Awake()
    {
        prevNumToggles = numToggles;
        for (int i = 0; i < arrTogle.Length; i++)
        {
            Toggle toggle = arrTogle[i];
            toggle.onValueChanged.AddListener((state) => Debug.Log($"{toggle.name}[{state}]"));
        }
    }

    private void OnValidate()
    {
        if (prevNumToggles != numToggles)
        {
            Change();
            prevNumToggles = numToggles;
        }
    }

    // Update is called once per frame
    void Change()
    {
        arrTogle[numToggles-1].SetIsOnWithoutNotify(true);
        toggleGroup.NotifyToggleOn(arrTogle[numToggles-1]);
        Debug.Log($"Change Finished");
    }
}
