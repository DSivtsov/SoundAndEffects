using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using GMTools.Menu;

//public enum MixerAudio
//{
//    Master,
//    Music,
//    Effects
//}

public class AudioVolumeOptions
{
    private class VolumeCtrl
    {
        public Slider sliderVol;
        public string paramName;

        public VolumeCtrl(Slider sliderVol, string paramName)
        {
            this.sliderVol = sliderVol;
            this.paramName = paramName;
        }
    }
    private int _numberElements;
    //private Type _enumType = typeof(MixerAudio);
    private AudioMixer _mixerMain;
    private VolumeCtrl[] _mixerVolSliders;
    private AudioOptionsController _audioMixerController;

    public AudioVolumeOptions(string[] transformName, string[] paramName, Transform audioGroupOptions, AudioMixer mixerMain, AudioOptionsController audioMixerController)
    {
        _numberElements = transformName.Length;
        if (_numberElements != paramName.Length)
            throw new Exception($"AudioOptions.ctor the number of parameters are not correlated each other transformName[{_numberElements}] != paramName[{paramName.Length}]");
        //if (_numberElements != Enum.GetValues(_enumType).Length)
        //    throw new Exception($"AudioOptions.ctor the number of parameters not correlated with {_enumType.Name} Enum");
        _mixerMain = mixerMain;
        _audioMixerController = audioMixerController;
        _mixerVolSliders = new VolumeCtrl[_numberElements];
        for (int i = 0; i < _numberElements; i++)
        {
            _mixerVolSliders[i] = new VolumeCtrl(audioGroupOptions.Find(transformName[i]).GetComponent<Slider>(), paramName[i]);
        }
    }

    public void InitVolumeControls()
    {
        for (int i = 0; i < _numberElements; i++)
        {
            _mixerMain.GetFloat(_mixerVolSliders[i].paramName, out float volume);
            _mixerVolSliders[i].sliderVol.value = volume;
            int idxVolume = i;
            _mixerVolSliders[i].sliderVol.onValueChanged.AddListener((float newValue) => VolumeChange(newValue, idxVolume));
        }
    }

    public void VolumeChange(float newValue, int idxVolume)
    {
        _mixerMain.SetFloat(_mixerVolSliders[idxVolume].paramName, newValue);
        _audioMixerController.AudioOptionsChanged(true);
    }

    public void ResetMixerParamToDefaul()
    {
        for (int i = 0; i < _numberElements; i++)
            _mixerMain.ClearFloat(_mixerVolSliders[i].paramName);
    }

    public void ResetVolumeSlidersToDefaul()
    {
        for (int i = 0; i < _numberElements; i++)
        {
            _mixerMain.GetFloat(_mixerVolSliders[i].paramName, out float volume);
            _mixerVolSliders[i].sliderVol.value = volume;
        }
    }
}
