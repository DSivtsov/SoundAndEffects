
using System;
using System.Collections;
using UnityEngine;

public class CountFrame : MonoBehaviour
{
    public static int currentNumUpdateFrame { get; private set; } = 1;

    public static int currentNumFixedUpdateFrame { get; private set; } = 1;

#if UNITY_EDITOR
    private void Awake()
    {
        StartCoroutine(CountUpdateFrame());
        StartCoroutine(CountFixedUpdateFrame());
    }
    private IEnumerator CountUpdateFrame()
    {
        do
        {
            yield return new WaitForEndOfFrame();
            currentNumUpdateFrame++;
        } while (true);
    }

    private IEnumerator CountFixedUpdateFrame()
    {
        do
        {
            yield return new WaitForFixedUpdate();
            currentNumFixedUpdateFrame++;
        } while (true);
    }
#endif
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLogUpdate(string str) => Debug.Log($"[U#{currentNumUpdateFrame}] " + str);

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLogUpdate(MonoBehaviour _this, string str) => Debug.Log($"[U#{currentNumUpdateFrame}] {_this} [{_this.gameObject.scene.name}] " + str);

    /// <summary>
    /// Will make output periodical only 
    /// </summary>
    /// <param name="_this"></param>
    /// <param name="str"></param>
    /// <param name="UpdateFramePeriod">data output period by frame count</param>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLogUpdate(MonoBehaviour _this, string str, int UpdateFramePeriod)
    {
        if (UpdateFramePeriod > 0 && currentNumUpdateFrame % UpdateFramePeriod == 0)
            DebugLogUpdate(_this, str);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLogFixedUpdate(string str) => Debug.Log($"[FU#{currentNumFixedUpdateFrame}] " + str);

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLogFixedUpdate(MonoBehaviour _this, string str) => Debug.Log($"[FU#{currentNumFixedUpdateFrame}] {_this} [{_this.gameObject.scene.name}] " + str);

    /// <summary>
    /// Will make output periodical only 
    /// </summary>
    /// <param name="_this"></param>
    /// <param name="str"></param>
    /// <param name="UpdateFixedFramePeriod">data output period</param>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLogFixedUpdate(MonoBehaviour _this, string str, int UpdateFixedFramePeriod)
    {
        if (UpdateFixedFramePeriod > 0 && currentNumFixedUpdateFrame % UpdateFixedFramePeriod == 0)
            DebugLogUpdate(_this, str);
    }
} 

