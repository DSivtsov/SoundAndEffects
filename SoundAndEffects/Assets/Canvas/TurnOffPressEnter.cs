using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activate initial Screen "textPreesEnter"
/// </summary>
public class TurnOffPressEnter : MonoBehaviour
{
    public GameObject textPreesEnter;

    public void Active(bool value)
    {
        textPreesEnter.SetActive(value);
    }
}
