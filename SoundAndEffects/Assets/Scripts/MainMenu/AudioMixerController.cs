using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public enum MixerAudio
{
    Master,
    Music,
    Effects
}

public class AudioMixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixerMain;
    [SerializeField] private Transform _audioGroupOptions;
    //private Slider sliderVolMusic;
    private AudioOptions _audioOptions;

    private void Awake()
    {
        Debug.Log($"_audioGroupOptions{_audioGroupOptions}");
        _audioOptions = new AudioOptions(new string[] { "MasterVolume/Slider", "MusicVolume/Slider", "EffectsVolume/Slider" },
                                        new string[] { "VolMaster", "VolMusic", "VolEffects" }, _audioGroupOptions, _mixerMain);
        //sliderVolMusic = _audioGroupOptions.Find("MusicVolume/Slider").GetComponent<Slider>();
    }

    UnityEngine.Events.UnityAction<float> t;

    private void Start()
    {
        _audioOptions.InitVolumeControls();
        //sliderVolMusic.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        //sliderVolMusic.onValueChanged.AddListener((float t) => Debug.Log($"val={t}"));
        //t = ValueChangeCheck;
        //Debug.Log(_mixerMain.name);
        //_mixerMain.GetFloat("VolMusic", out float volMusic);
        //_mixerMain.GetFloat("VolMaster", out float volMaster);
        //StartCoroutine(RaiseMusic()); MainVolume
    }

    private void ValueChangeCheck(float r)
    {
        throw new NotImplementedException();
    }

    private IEnumerator RaiseMusic()
    {
        yield return new WaitForSeconds(5f);
        _mixerMain.SetFloat("VolMusic", 0);
    }
}
