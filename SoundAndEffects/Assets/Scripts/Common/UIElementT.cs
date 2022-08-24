using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GMTools.Menu;

public abstract class UIElement<T>
{
    protected UIElement()
    {
        Initialized = false;
        Dirty = false;
    }

    //protected event Action<T> onChangeValue;

    public virtual void Init(Action<T> callback)
    {
        Initialized = true;
        //onChangeValue += callback;
    }
    public abstract void SetValue(T value);
    public abstract T GetValue();
    
    public bool Initialized { get; protected set; }
    public bool Dirty { get; }
}
