using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SetAudioClips
{
    public EnumSO enumClip;
    [SerializeField] public  AudioClip audioClip;
    [Range(0, 2f)] [SerializeField] public float pitch = 1f;
    [Range(0, 1f)] [SerializeField] public float volume = 1f;
}

[CreateAssetMenu(fileName = "SetSounds", menuName = "AudioEvent/SetSounds")]
public class SetAudioClipsSO : AudioEvent
{
    [SerializeField] private SetAudioClips[] audioClips;

    //private bool initClipDict;
    private int[] clipDict;
    private int this[PlayerState index]
    {
        get => clipDict[(int)index]; 
        set => clipDict[(int)index] = value;
    }

    //private void OnEnable()
    //{
    //    initClipDict = false;
    //}

    /// <summary>
    /// Initialization of Clip dictionay based on SO parameters and linksSetAudio
    /// </summary>
    /// <param name="linksSetAudio"></param>
    /// <returns>true if initialized</returns>
    public bool InitClipDict(LinkStateAndClip<PlayerState>[] linksSetAudio)
    {
        //Debug.Log(LinkStateAndClip<PlayerState>.NumberEnumValues);
        //The size of the array is set to the number of possible values of the Enum type being used.
        clipDict = new int[LinkStateAndClip<PlayerState>.NumberEnumValues];
        for (int i = 0; i < linksSetAudio.Length; i++)
        {
            if (!SearchClipForState(linksSetAudio[i]))
            {
                Debug.LogError($"[{this}] AudioEvent disabled because SO with Audio clips doesn't have the demanded clip for [{linksSetAudio[i].state}] state");
                return false; 
            }
        }
        //foreach (var item in linksSetAudio)
        //{
        //    Debug.Log($"[{item.state}]={this[item.state]}");
        //}
        return true;
    }

    /// <summary>
    /// Associate an entry from the audioClips array with the corresponding state
    /// </summary>
    /// <param name="linkSetAudio"></param>
    /// <returns>false if not found the corresponding clip</returns>
    private bool SearchClipForState(LinkStateAndClip<PlayerState> linkSetAudio)
    {
        PlayerState state = linkSetAudio.state;
        EnumSO enumState = linkSetAudio.enumAudio;
        for (int j = 0; j < audioClips.Length; j++)
        {
            if (audioClips[j].enumClip == enumState)
            {
                this[state] = j;
                return true;
            }
        }
        return false;
    }

    public void PlayClip(AudioSource audioSource, PlayerState state, float delay = 0)
    {
        PlayClip(audioSource, this[state], delay);
    }

    public void PlayClip(AudioSource audioSource, int idxClip, float delay = 0)
    {
        SetAudioClips item = audioClips[idxClip];
        audioSource.pitch = item.pitch;
        audioSource.volume = item.volume;
        audioSource.clip = item.audioClip;
        //audioSource.PlayDelayed(.25f);
        audioSource.PlayDelayed(delay);
    }

    public override bool ClipsArrayEmpty() => audioClips.Length == 0;

#if UNITY_EDITOR
    private int nextClipIdx = 0;

    public override void PlayOneClip(AudioSource audioSource)
    {
        if (ClipsArrayEmpty())
        {
            Debug.LogError($"[{this}] SO with Audio clips is Empty");
            return;
        }
        //Debug.Log($"nextClipIdx[{nextClipIdx}] enumClip[{audioClips[nextClipIdx].enumClip}] clipName[{audioClips[nextClipIdx].audioClip.name}]" +
        //    $" pitch={audioClips[nextClipIdx].pitch:f2} volume={audioClips[nextClipIdx].volume:f2}");
        PlayClip(audioSource, nextClipIdx);
        nextClipIdx = (nextClipIdx + 1) % audioClips.Length;
    } 
#endif
}
