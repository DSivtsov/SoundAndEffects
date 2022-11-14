interface IUpdateElementValue
{
    public void UpdateElementValue();
}

public class LinkFieldToElement<T> : IUpdateElementValue
{
    ExposeField<T> fieldTolink;
    IElement<T> element;

    public LinkFieldToElement(ExposeField<T> fieldTolink, IElement<T> element)
    {
        this.fieldTolink = fieldTolink;
        this.element = element;
        element.onNewValue += fieldTolink.SetNewValue;
    }

    public void UpdateElementValue() => element.SetValue(fieldTolink.GetCurrentValue());
}