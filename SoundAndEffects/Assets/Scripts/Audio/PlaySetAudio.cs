using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

[Serializable]
public class LinkStateAndClip
{
    public PlayerState playerState;
    public EnumSO enumAudio;
}
[Serializable]
public class LinkStateAndClip<T> where T : Enum
{
    public T state;
    public EnumSO enumAudio;

    static LinkStateAndClip()
    {
        NumberEnumValues = Enum.GetValues(typeof(T)).Length;
    }

    public static int NumberEnumValues { get; private set; }
}

public class PlaySetAudio : MonoBehaviour
{
    #region SerializedFields
    [SerializeField] private SetAudioClipsSO _audioSet;
    [SerializeField] private AudioMixerGroup _mixerGroup;
    //[SerializeField] private bool _playAtAwake;
    [Range(0, 255)] [SerializeField] private int _sourcePriority;
    public LinkStateAndClip<PlayerState>[] linksSetAudio;
    #endregion

    #region NonSerializedFields
    private AudioSource audioSource;
    private GameObject SoundtrackGameObject;
    public bool SFXState { get; private set; } = false;
    #endregion

    private void OnEnable()
    {
        //Debug.Log($"{this} OnEnable()");
        CheckInitSetSounds();
    }

    private void OnDisable()
    {
        Destroy(SoundtrackGameObject);
    }

    public void PlaySound(PlayerState currentState, float delay = 0)
    {
        if (SFXState)
        {
            _audioSet.PlayClip(audioSource, currentState, delay); 
        }
    }

    /// <summary>
    /// Check and inti the SO SetSounds and init AudioSources
    /// </summary>
    private void CheckInitSetSounds()
    {
        if (_audioSet.ClipsArrayEmpty())
        {
            Debug.LogError($"[{gameObject.name}] AudioEvent disabled because SO with Audio clips is Empty");
            SFXState = false;
        }
        else
        {
            SoundtrackGameObject = new GameObject($"Soundtrack");
            SoundtrackGameObject.transform.parent = gameObject.transform;
            audioSource = SoundtrackGameObject.AddComponent<AudioSource>();
            audioSource.priority = _sourcePriority;
            audioSource.outputAudioMixerGroup = _mixerGroup;
            if (_audioSet.InitClipDict(linksSetAudio))
                SFXState = true;
        }
    }

    /// <summary>
    /// Try turnOn or turnOff SetSounds.
    /// </summary>
    public void TurnOn(bool turnOn = true)
    {
        if (!SFXState)
        {
            if (turnOn)
            {
                CheckInitSetSounds();
                //if (MusicState)
                //    _jukeBox.PlayClipNextInit();
            }
        }
        else
        {
            if (!turnOn)
            {
                audioSource.Stop();
                SFXState = false;
            }
        }
    }
}
