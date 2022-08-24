using System;

public interface IUIElement<T>
{
    public void Init(Action<T> callback, T initialValue);
    public void SetValueWithoutNotify(T value);
    public T GetValue();
    //public event Action<T> onChangeValue;
    public bool Initialized { get; }
    public bool Dirty { get; }
}
