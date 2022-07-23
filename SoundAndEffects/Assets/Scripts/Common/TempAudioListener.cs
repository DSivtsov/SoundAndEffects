using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Add AudioListener to current object if Scene can't find any one actine Audiolisneter
/// </summary>
public class TempAudioListener : MonoBehaviour
{
    private void Awake()
    {
        //WARNING! This finds disabled listener components!! (although not inactive GOs with listeners on them)
        AudioListener[] myListeners = FindObjectsOfType<AudioListener>();

        if (myListeners.Length < 1)
        {
            AddOneActiveAudioListener();
        }
        else
        {
            for (int i = 0; i < myListeners.Length; i++)
            {
                if (myListeners[i].enabled)
                    return;
            }
            AddOneActiveAudioListener();
        }
    }

    private void AddOneActiveAudioListener()
    {
        gameObject.AddComponent<AudioListener>();
        Debug.LogWarning($"Temporary for [{gameObject.scene.name}] Scene was added [AudioListener]");
    }
}
