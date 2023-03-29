using GMTools.Menu.Elements;
using GMTools;
using UnityEngine;

public class DropdownFieldSequenceOrder : DropdownListEnum<SequenceType>
{
    private AudioController _audioContoller;
    private bool _dropdownFieldSequenceOrderIsInited = false;

    protected override void Awake()
    {
        base.Awake();
        onNewValue += (SequenceType type) => SetSequenceType(type);
        InitElement();
    }

    public override void InitElement()
    {
        if (!_dropdownFieldSequenceOrderIsInited)
        {
            base.InitElement();
            _audioContoller = AudioController.Instance;
            _dropdownFieldSequenceOrderIsInited = true;
        }
    }

    public override void SetValue(SequenceType type)
    {
        base.SetValue(type);
        SetSequenceType(type);
    }

    private void SetSequenceType(SequenceType type)
    {
        if (_dropdownFieldSequenceOrderIsInited)
        {
            _audioContoller.SetSequenceType(type); 
        }
        else
            Debug.LogError($"{this} : Attemp SetValue but DropdownFieldSequenceOrder is not inited");
    }
}
