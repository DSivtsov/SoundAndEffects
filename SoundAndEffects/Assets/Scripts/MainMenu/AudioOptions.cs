using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioOptions
{
    public class VolumeCtrl
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
    private Type _enumType = typeof(MixerAudio);
    private AudioMixer _mixerMain;
    private VolumeCtrl[] _mixerVolSliders;

    public AudioOptions(string[] transformName, string[] paramName, Transform audioGroupOptions, AudioMixer mixerMain)
    {
        _numberElements = transformName.Length;
        if (_numberElements != paramName.Length)
            throw new Exception($"AudioOptions.ctor the number of parameters are not correlated each other transformName[{_numberElements}] != paramName[{paramName.Length}]");
        if (_numberElements != Enum.GetValues(_enumType).Length)
            throw new Exception($"AudioOptions.ctor the number of parameters not correlated with {_enumType.Name} Enum");
        _mixerMain = mixerMain;
        _mixerVolSliders = new VolumeCtrl[_numberElements];
        for (int i = 0; i < _numberElements; i++)
        {
            _mixerVolSliders[i] = new VolumeCtrl(audioGroupOptions.Find(transformName[i]).GetComponent<Slider>(), paramName[i]);
            //_mixerVolSliders[i].sliderVol = audioGroupOptions.Find(transformName[i]).GetComponent<Slider>();
            //_mixerVolSliders[i].paramName = paramName[i];
        }
        Debug.Log($"AudioOptions.ctor : Created");
    }

    private VolumeCtrl this[MixerAudio index]
    {
        get => _mixerVolSliders[(int)index];
        set => _mixerVolSliders[(int)index] = value;
    }

    public void InitVolumeControls()
    {
        for (int i = 0; i < _numberElements; i++)
        {
            _mixerMain.GetFloat(_mixerVolSliders[i].paramName, out float volume);
            _mixerVolSliders[i].sliderVol.value = volume;
            int idxVolume = i;
            //_mixerVolSliders[i].sliderVol.onValueChanged.AddListener((float newValue) =>
            //{
            //    Debug.Log($"Volume [{(MixerAudio)index}] new val={newValue}");
            //});
            _mixerVolSliders[i].sliderVol.onValueChanged.AddListener((float newValue) => VolumeChange(newValue, idxVolume));
        }
    }

    public void VolumeChange(float newValue, int idxVolume)
    {
        Debug.Log($"Volume [{(MixerAudio)idxVolume}] new val={newValue}");
        _mixerMain.SetFloat(_mixerVolSliders[idxVolume].paramName, newValue);
    }
}
