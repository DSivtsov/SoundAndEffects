using System;
using System.Collections.Generic;
using GMTools.Menu.Elements;

public abstract class LinkFieldToElementBase
{
    public static event Action UpdateElementValue;

    public static void UpdateElementsValues()
    {
        UpdateElementValue?.Invoke();
    }

    public static void Link<T>(ExposeField<T> field, IElement<T> uiElement)
    {
        uiElement.InitElement();
        uiElement.onNewValue += field.SetNewValue;
        UpdateElementValue += () => uiElement.SetValue(field.GetCurrentValue());
    }
}

//public class LinkFieldToElement<T> : LinkFieldToElementBase
//{
//    public LinkFieldToElement(ExposeField<T> field, IElement<T> uiElement)
//    {
//        uiElement.InitElement();
//        uiElement.onNewValue += field.SetNewValue;
//        UpdateElementValue += () => uiElement.SetValue(field.GetCurrentValue());
//    }
//}