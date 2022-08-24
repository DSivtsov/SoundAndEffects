using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using GMTools.Menu;



public class AudioOptionsController : MonoBehaviour, ISectionControllerAction
{
    [SerializeField] private SettingsSectionManager _sectionManagerOptions;
    //[SerializeField] private SectionName _sectionName;
    //[SerializeField] private Transform _audioGroupOptions;
    [SerializeField] private SectionObject _audioSection;
    [SerializeField] private AudioMixer _mixerMain;
    [SerializeField] private JukeBoxSO _jukeBoxSO;

    private AudioVolumeOptions _audioVolumeOptions;
    private AudioMusicOrderOption _audioMusicOrderOption;
    private MainMenusSceneManager _mainMenusSceneManager;

    public bool IsAudioOptionsChanged { get; private set; }

    private void Awake()
    {
        _audioVolumeOptions = new AudioVolumeOptions(new string[] { "MasterVolume/Slider", "MusicVolume/Slider", "EffectsVolume/Slider" },
                                        new string[] { "VolMaster", "VolMusic", "VolEffects" }, _audioSection.TransformSection, _mixerMain, this);
        _audioMusicOrderOption = new AudioMusicOrderOption("MusicOrder", _audioSection.TransformSection, this);
        IsAudioOptionsChanged = false;
        _mainMenusSceneManager = FindObjectOfType<MainMenusSceneManager>();
    }

    private void Start()
    {
        _audioVolumeOptions.InitVolumeControls();
        _sectionManagerOptions.LinkToSectionActions(_audioSection.NameSection, this);
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
            yield return null;
        } while (!_mainMenusSceneManager.GetStatusLoadingScenes());
        _audioMusicOrderOption.FillArrPlayJukeBox();
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
    public void ResetSectionValuesToDefault()
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

    public void LoadSectionValues()
    {
        //throw new NotImplementedException();
        Debug.LogWarning($"{this} : LoadSectionValues()");
    }
}
