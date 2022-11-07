// <copyright file="ReferenceResolvers.cs">
//  Copyright (c) 2022 Denis Sivtsov.
//  Licensed under the Apache License, Version 2.0
// </copyright>
//
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OdinSerializer;

public class UniversalScriptableObjectStringReferenceResolver<T> : IExternalStringReferenceResolver where T : ScriptableObject
{
    private static Dictionary<string, T> dictObjectByStr;
    private static readonly object lockObject = new object();
    private static bool _init = false;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pathResourceAssets">By default will try to load the entire whole resource folder</param>
    public UniversalScriptableObjectStringReferenceResolver(string pathResourceAssets="")
    {
        lock (lockObject)
        {
            if (_init)
                return;
            else
                _init = true;
        }
        dictObjectByStr = Resources.LoadAll<T>(pathResourceAssets).ToDictionary((SO) => SO.name, (SO) => SO);
#if UNITY_EDITOR
        //Debug.Log($".ctor UniversalScriptableObjectStringReferenceResolver : {typeof(T)} dictionary filled by SO, count={dictObjectByStr.Count}"); 
#endif
    }

    private static T ResolveStringReference(string name)
    {
        if (dictObjectByStr != null)
        {
            if (name != null)
            {
                if (dictObjectByStr.TryGetValue(name, out T SO))
                {
                    return SO;
                }
                else
                    return null;
                    //Debug.LogWarning($"ResolveStringReference() : Can find the {typeof(T)} object with {name} name");
            }
            throw new NotImplementedException($"ResolveStringReference() : string reference==null");
        }
        throw new NotImplementedException($"ResolveStringReference() : dictionary==null");
        //else
        //    Debug.LogError($"ResolveStringReference() : dictionary==null");
        //return null;
    }

    // Multiple string reference resolvers can be chained together.
    public IExternalStringReferenceResolver NextResolver { get; set; }

    public bool CanReference(object value, out string id)
    {
#if UNITY_EDITOR
        //Debug.Log($"{this} : CanReference(Type[{value.GetType().Name}])");
        //Debug.Log($"value is T[{value is T}]"); 
#endif
        if (value is T objSO)
        {
            id = objSO.name;
            //Debug.Log($"id = [{id}]");
            return true;
        }
        id = null;
        return false;
    }

    public bool TryResolveReference(string id, out object value)
    {
#if UNITY_EDITOR
        //Debug.Log($"{this} : TryResolveReference(id[{id}])"); 
#endif
        value = ResolveStringReference(id);
        //if NextResolver == null it means that this resolver was a last in chain
        if (value == null && NextResolver == null)
        {
            Debug.LogError($"ResolveStringReference() : Can find the {typeof(T)} object with {id} name and it was a last resolver in chain");
        }
        return value != null;
    }
}