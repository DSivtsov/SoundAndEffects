using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioEvent : ScriptableObject
{
    public abstract bool ClipsArrayEmpty();

#if UNITY_EDITOR
    public abstract void PlayOneClip(AudioSource audioSource); 
#endif
}
