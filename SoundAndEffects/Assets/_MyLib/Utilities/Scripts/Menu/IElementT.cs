using System;
namespace GMTools.Menu.Elements
{
    public interface IElement<T>
    {
        public void InitElement();
        /// <summary>
        /// Raised on update value of Element by UI
        /// </summary>
        public event Action<T> onNewValue;
        /// <summary>
        /// Update value Element got from external source through LinkFieldToElementBase:UpdateElementsValues(), SetValue doen't raise the onNewValue
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(T value);
    }
}