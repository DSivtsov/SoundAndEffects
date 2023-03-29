using GMTools.Menu.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueableSliderIntVolume : ValueableSliderInt
{
    [SerializeField] private MixerVolume _mixer;

    private AudioController _audioContoller;
    private bool _sliderIntVolumeIsInited = false;

    protected override void Awake()
    {
        base.Awake();
        onNewValue += (int newVolume) => SetMixerVolume(newVolume);
        InitElement();
    }

    public override void InitElement()
    {
        if (!_sliderIntVolumeIsInited)
        {
            base.InitElement();
            _audioContoller = AudioController.Instance;
            _sliderIntVolumeIsInited = true;
        }
    }

    public override void SetValue(int volume)
    {
        base.SetValue(volume);
        SetMixerVolume(volume);
    }

    private void SetMixerVolume(int newVolume) => _audioContoller.SetMixerVolume(_mixer, newVolume);
}
