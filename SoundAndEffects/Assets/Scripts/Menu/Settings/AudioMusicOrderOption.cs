using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using GMTools.Menu;
using TMPro;
using GMTools;

public class AudioMusicOrderOption
{
    private TMP_Dropdown _dropdownMusicOrder;
    private SectionAudioOptionsController _audioMixerController;
    private Type enumType = typeof(SequenceType);
    private PlayJukeBox[] _arrPlayJukeBoxes;
    /// <summary>
    /// Initial sequence type will be get from first was found the JukeBox SO and it will used for all JukeBox SO at ResetDefault
    /// </summary>
    private SequenceType _initialSequenceType;

    public AudioMusicOrderOption(string paramName, Transform audioGroupOptions, SectionAudioOptionsController audioMixerController)
    {
        _dropdownMusicOrder = audioGroupOptions.Find(paramName).GetComponent<TMP_Dropdown>();
        _audioMixerController = audioMixerController;
    }

    public void InitMusicOrderOption()
    {
        _dropdownMusicOrder.ClearOptions();
        _dropdownMusicOrder.AddOptions(new List<string>(Enum.GetNames(enumType)));
    }

    /// <summary>
    /// Will be search for all MonoBehaviours with PlayJukeBox Base class  in all loasded Scenes
    /// </summary>
    public void FillArrPlayJukeBox()
    {
        _arrPlayJukeBoxes = UnityEngine.Object.FindObjectsOfType<PlayJukeBox>(includeInactive: true);
        if (_arrPlayJukeBoxes.Length != 0)
        {
            _initialSequenceType = _arrPlayJukeBoxes[0].GetInitialSeqenceType();
            //Set initial value to dropdown and then AddListener for onValueChanged event 
            //_dropdownMusicOrder.value = (int)_initialSequenceType;
            //_dropdownMusicOrder.onValueChanged.AddListener(MusicOrderValueChanged);
            Debug.LogWarning($"FillArrPlayJukeBox() : _arrPlayJukeBoxes.Length={_arrPlayJukeBoxes.Length} _initialSequenceType={_initialSequenceType}");
        }
        else
            throw new Exception($"FillArrPlayJukeBox() : AudioMusicOrderOption can be Inited");
    }

    //private void MusicOrderValueChanged(int value)
    //{
    //    MusicOrderSetValue((SequenceType)value);
    //    //_audioMixerController.AudioOptionsChanged(true);
    //}

    private void MusicOrderSetValue(SequenceType sequenceType)
    {
        for (int i = 0; i < _arrPlayJukeBoxes.Length; i++)
        {
            _arrPlayJukeBoxes[i].SwitchJukeBoxSequenceType(sequenceType);
        }
    }
    //Not checked Inited because it can be called w/o _inited = true
    //public void ResetDefault()
    //{
    //     MusicOrderSetValue(_initialSequenceType);
    //    _dropdownMusicOrder.value = (int)_initialSequenceType;
    //}
}

