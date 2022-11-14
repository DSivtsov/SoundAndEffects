using System;

public interface IElement<T>
{
    public event Action<T> onNewValue;
    public void SetValue(T value);
}