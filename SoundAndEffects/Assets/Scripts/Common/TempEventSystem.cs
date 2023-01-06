
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
/// <summary>
/// Add EventSystem to current object if Scene can't find any one actine EventSystem, in Editor only
/// </summary>
public class TempEventSystem : MonoBehaviour
{
#if UNITY_EDITOR
    private void Awake()
    {
        EventSystem[] myEventSystems = FindObjectsOfType<EventSystem>(includeInactive: true);
        if (myEventSystems.Length < 1)
        {
            AddOneEventSystem();
        }
        else
        {
            for (int i = 0; i < myEventSystems.Length; i++)
            {
                if (myEventSystems[i].enabled)
                    return;
                else
                    Debug.Log($"[EventSystem] in {myEventSystems[i].gameObject.name} not enabled");
            }
            AddOneEventSystem();
        }

    }

    private void AddOneEventSystem()
    {
        EventSystem newEventSystems = gameObject.AddComponent<EventSystem>();
        Debug.LogWarning($"Temporary for [{gameObject.scene.name}] Scene was added [EventSystem] with [InputSystemUIInputModule]");
        InputSystemUIInputModule inputModule = gameObject.AddComponent<InputSystemUIInputModule>();
    }
#endif
}
