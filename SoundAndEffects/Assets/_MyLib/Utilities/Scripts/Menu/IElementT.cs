using System;
namespace GMTools.Menu.Elements
{
    public interface IElement<T>
    {
        public void InitElement();

        public event Action<T> onNewValue;
        public void SetValue(T value);
    }
}