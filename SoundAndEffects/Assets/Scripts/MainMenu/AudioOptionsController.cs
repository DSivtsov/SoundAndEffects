using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using GMTools;

public enum MixerAudio
{
    Master,
    Music,
    Effects
}

public class AudioOptionsController : MonoBehaviour, IButtonAction
{
    [SerializeField] private SectionManagerOptions _sectionManagerOptions;
    [SerializeField] private SectionName _sectionName;
    [SerializeField] private Transform _audioGroupOptions;
    [SerializeField] private AudioMixer _mixerMain;
    [SerializeField] private JukeBoxSO _jukeBoxSO;
    //[Tooltip("Set the same initial SequenceType for all JukeBox")]
    //[SerializeField] private SequenceType _initialSequenceType;


    private TMP_Dropdown _dropdownMusicOrder;
    private AudioVolumeOptions _audioVolumeOptions;
    private AudioMusicOrderOption _audioMusicOrderOption;
    private MainMenusSceneManager _mainMenusSceneManager;

    public bool IsAudioOptionsChanged { get; private set; }

    private void Awake()
    {
        Debug.Log($"_audioGroupOptions{_audioGroupOptions}");
        _audioVolumeOptions = new AudioVolumeOptions(new string[] { "MasterVolume/Slider", "MusicVolume/Slider", "EffectsVolume/Slider" },
                                        new string[] { "VolMaster", "VolMusic", "VolEffects" }, _audioGroupOptions, _mixerMain, this);
        IsAudioOptionsChanged = false;

        _audioMusicOrderOption = new AudioMusicOrderOption("MusicOrder", _audioGroupOptions, this);
        _dropdownMusicOrder = _audioGroupOptions.Find("MusicOrder").GetComponent<TMP_Dropdown>();

        _mainMenusSceneManager = FindObjectOfType<MainMenusSceneManager>();
    }

    //private void Show(int t = 0)
    //{
    //    Debug.Log($"_dropdownMusicOrder.value = {_dropdownMusicOrder.value}");
    //    Debug.Log($"_dropdownMusicOrder.itemText = {_dropdownMusicOrder.itemText.text}");
    //    Debug.Log($"_dropdownMusicOrder.captionText = {_dropdownMusicOrder.captionText.text}");
    //    Debug.Log($"_dropdownMusicOrder.options[].text = {_dropdownMusicOrder.options[t].text}");
    //}

    ///         //Keep the current index of the Dropdown in a variable
    ///         m_DropdownValue = m_Dropdown.value;
    ///         //Change the message to say the name of the current Dropdown selection using the value
    ///         m_Message = m_Dropdown.options[m_DropdownValue].text;

    private void Start()
    {
        _audioVolumeOptions.InitVolumeControls();
        _sectionManagerOptions.LinkToButtonActions(_sectionName, this);
        StartCoroutine(InitMusicOrderOptionCoroutine());
    }
    /// <summary>
    /// InitMusicOrderOptionCoroutine() postpone the FillArrPlayJukeBox() till the all Scenes will loaded
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitMusicOrderOptionCoroutine()
    {
        _audioMusicOrderOption.InitMusicOrderOption();
        do
        {
            //Debug.Log($"GetStatusLoadingScenes={GameMainManager.Instance.GetStatusLoadingScenes()}");
            yield return null;
        } while (!_mainMenusSceneManager.GetStatusLoadingScenes());
        _audioMusicOrderOption.FillArrPlayJukeBox();
        Debug.Log($"GetStatusLoadingScenes={_mainMenusSceneManager.GetStatusLoadingScenes()}");
    }
    /// <summary>
    /// Seting values to default for VolumeOptions & MusicOrderOption are generating will set IsAudioOptionsChanged to true. This variable block this.
    /// </summary>
    private TransitionFinished rezultResetDeafult;
    [Flags]
    private enum TransitionFinished : byte
    {
        NotFinishedAny = 0b00,
        VolumeResetDefault = 0b01,
        MusicOrderResetDefault = 0b10,
        FinishedAll = 0b11,
    }

    public void AudioOptionsChanged(bool isChanged)
    {
        if (!IsAudioOptionsChanged && isChanged)
        {
            IsAudioOptionsChanged = true;
            _sectionManagerOptions.ActivateResetButton(true);
            return;
        }
        if (!isChanged && IsAudioOptionsChanged && rezultResetDeafult.HasFlag(TransitionFinished.FinishedAll))
        {
            IsAudioOptionsChanged = false;
            _sectionManagerOptions.ActivateResetButton(false);
            return;
        }
    }
    /// <summary>
    /// Actions on ResetDeafult pressing
    /// </summary>
    public void ResetDefault()
    {
        rezultResetDeafult = TransitionFinished.NotFinishedAny;
        StartCoroutine(ResetDefaultVolumeCoroutine());
        ResetDefaultMusicOrder();
    }

    private void ResetDefaultMusicOrder()
    {
        _audioMusicOrderOption.ResetDefault();
        rezultResetDeafult |= TransitionFinished.MusicOrderResetDefault;
        AudioOptionsChanged(false);
    }

    //Features of AudioMixer.ClearFloat() because it will only change values after one Update() cycle
    private IEnumerator ResetDefaultVolumeCoroutine()
    {
        _audioVolumeOptions.ResetMixerParamToDefaul();
        yield return null;
        _audioVolumeOptions.ResetVolumeSlidersToDefaul();
        rezultResetDeafult |= TransitionFinished.VolumeResetDefault;
        AudioOptionsChanged(false);
    }
}