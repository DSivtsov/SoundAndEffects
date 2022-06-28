using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool
{
    Dictionary<int,Rigidbody> arrElements;
    Stack<int> arrIdxFreeElements;
    Func<Rigidbody> funcCreateElement;

    public Pool(Func<Rigidbody> func)
    {
        this.arrElements = new Dictionary<int, Rigidbody>();
        this.arrIdxFreeElements = new Stack<int>();
        this.funcCreateElement = func;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>not active Rigidbody</returns>
    public Rigidbody GetElement()
    {
        if (arrIdxFreeElements.Count > 0)
        {
            int keyElement = arrIdxFreeElements.Pop();
            //Debug.Log($"GetElement({keyElement})");
            return arrElements[keyElement]; 
        }
        else
        {
            Rigidbody newElement = funcCreateElement();
            //Debug.Log($"GetInstanceID({newElement.gameObject.GetInstanceID()})");
            newElement.gameObject.SetActive(false);
            //arrElements.Add(newElement);
            arrElements.Add(newElement.GetInstanceID(), newElement);
            return newElement; 
        }
    }

    public void ReturnElement(Rigidbody returnedElement)
    {
        int keyElement = returnedElement.GetInstanceID();
        //Doesn't check the validity of idxElement value
        if (arrElements.TryGetValue(keyElement, out Rigidbody value))
        {
            value.gameObject.SetActive(false);
            arrIdxFreeElements.Push(keyElement);
            //Debug.Log($"ReturnElement(<{keyElement},{value})");
        }
        else
            Debug.LogError($"Can't find {returnedElement} in Pool");
    }
}

public interface IPool
{
    Rigidbody GetRigidbody(Rigidbody Element);
}