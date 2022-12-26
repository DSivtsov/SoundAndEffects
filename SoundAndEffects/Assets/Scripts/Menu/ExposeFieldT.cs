using System;
using System.Collections.Generic;
using UnityEngine;

public class FlagGameSettingChanged
{
    private GameSettingChangedBit _gameSettingChanged = GameSettingChangedBit.NoChanges;
    public event Action<bool> ExistChangesAfterFlagUpdated;

    public void SetBitGameSettingChanged(bool set, GameSettingChangedBit bit)
    {
        if (set)
            _gameSettingChanged |= bit;
        else
            _gameSettingChanged &= ~bit;
        UpdateFlagExistChanges();
    }
    public void SetNoChanges()
    {
        _gameSettingChanged = GameSettingChangedBit.NoChanges;
        UpdateFlagExistChanges();
    }

    private void UpdateFlagExistChanges()
    {
        ExistChangesAfterFlagUpdated.Invoke(_gameSettingChanged != GameSettingChangedBit.NoChanges);
        //ShowChanges();
    }

    public bool IsThisBitSet(GameSettingChangedBit bit) => _gameSettingChanged.HasFlag(bit);

    void ShowChanges() => Debug.LogError($"changes={Convert.ToString((int)_gameSettingChanged, 2).PadLeft(8, '0')}");
}


public class ExposeFieldBase
{
    public static event Action UpdateInitValueExposeFields;

    public static void UpdateInitValue() => UpdateInitValueExposeFields?.Invoke();
}

public class ExposeField<T> : ExposeFieldBase
{
    public Func<T> GetCurrentValue { get; private set; }
    public Action<T> SetNewValue { get; private set; }

    private T initValue;

    public ExposeField(Func<T> get, Action<T> set, FlagGameSettingChanged flagGameSettingChanged, GameSettingChangedBit bitChanged)
    {
        GetCurrentValue = get;
        SetNewValue = (newValue) =>
        {
            flagGameSettingChanged.SetBitGameSettingChanged(!EqualityComparer<T>.Default.Equals(newValue, initValue), bitChanged);
            set(newValue);
        };
        UpdateInitValueExposeFields += () => StoreNewInitValue();
        StoreNewInitValue();
    }

    public void StoreNewInitValue() => initValue = GetCurrentValue();
}
