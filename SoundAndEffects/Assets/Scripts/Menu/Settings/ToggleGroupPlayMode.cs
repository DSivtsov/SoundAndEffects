using GMTools.Menu.Elements;
using UnityEngine;

public class ToggleGroupPlayMode : ToggleGroupEnum<PlayMode>
{
    [Header("FieldsOnlinePlaymode")]
    [SerializeField] private GameObject _titleOnlineOptions;
    [SerializeField] private GameObject _notCopyToGlobal;
    [SerializeField] private GameObject _defaultTopListGlobal;

    protected new void Awake()
    {
        base.Awake();
        onNewValue += (PlayMode mode) => ShowGroupFieldsBasedOnPlaymode(mode);
    }

    public override void SetValue(PlayMode value)
    {
        base.SetValue(value);
        ShowGroupFieldsBasedOnPlaymode(value);
    }

    private void ShowGroupFieldsBasedOnPlaymode(PlayMode currentMode)
    {
        bool playModeIsOnline = currentMode == PlayMode.Online;
        _notCopyToGlobal.SetActive(playModeIsOnline);
        _defaultTopListGlobal.SetActive(playModeIsOnline);
        _titleOnlineOptions.SetActive(playModeIsOnline);
    }
}