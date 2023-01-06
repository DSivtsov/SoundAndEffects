using System;
using GMTools.Menu.Elements;

public abstract class LinkFieldToElementBase
{
    public static event Action UpdateElementValue;

    /// <summary>
    /// Update Element values from GameSetting fields
    /// </summary>
    public static void UpdateElementsValues()
    {
        //if (UpdateElementValue != null)
        //{
        //    UnityEngine.Debug.Log($"GetInvocationList()?.Length={UpdateElementValue.GetInvocationList()?.Length}");
        //}
        UpdateElementValue?.Invoke();
    }

    public static void Link<T>(ExposeField<T> field, IElement<T> uiElement)
    {
        uiElement.InitElement();
        uiElement.onNewValue += field.SetNewValue;
        UpdateElementValue += () => uiElement.SetValue(field.GetCurrentValue());
    }
}