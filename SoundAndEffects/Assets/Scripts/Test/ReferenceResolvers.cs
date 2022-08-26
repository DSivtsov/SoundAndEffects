//-----------------------------------------------------------------------
// <copyright file="ReferenceResolvers.cs" company="Goodman Inc">
// Copyright (c) 2022 Goodman Inc
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------
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
        Debug.Log($".ctor UniversalScriptableObjectStringReferenceResolver : {typeof(T)} dictionary filled by SO, count={dictObjectByStr.Count}");
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
                    Debug.LogError($"ResolveStringReference() : Can find the LevelSO with {name} name");
            }
        }
        else
            Debug.LogError($"ResolveStringReference() : dictionary==null");
        return null;
    }

    // Multiple string reference resolvers can be chained together.
    public IExternalStringReferenceResolver NextResolver { get; set; }

    public bool CanReference(object value, out string id)
    {
        Debug.Log($"{this} : CanReference(Type[{value.GetType().Name}])");
        //if (value is ScriptableObject objSO)
        Debug.Log($"value is T[{value is T}]");
        if (value is T objSO)
        {
            //id = ((T)objSO).name;
            id = objSO.name;
            Debug.Log($"id = [{id}]");
            return true;
        }
        id = null;
        return false;
    }

    public bool TryResolveReference(string id, out object value)
    {
        Debug.Log($"{this} : TryResolveReference(id[{id}])");
        value = ResolveStringReference(id);
        //Debug.Log($"value[{((T)value).name}]");
        return value != null;
    }
}

public class UniversalScriptableObjectStringReferenceResolver : IExternalStringReferenceResolver
{
    // Multiple string reference resolvers can be chained together.
    public IExternalStringReferenceResolver NextResolver { get; set; } 

    public bool CanReference(object value, out string id)
    {
        //Debug.Log($"{this} : CanReference(Type[{value.GetType().Name}])");
        //if (value is ScriptableObject objSO)
        if (value is NewGameSettingsSO objSO)
        {
            id = ((NewGameSettingsSO)objSO).GetStringReference();
            return true;
        }
        id = null;
        return false;
    }

    public bool TryResolveReference(string id, out object value)
    {
        value = ScriptableObject.CreateInstance<NewGameSettingsSO>();
        Debug.Log(value);
        ((NewGameSettingsSO)value).ResolveStringReference(id);
        //value = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(id));
        return value != null;
    }
}

#if UNITY_EDITOR
public class ScriptableObjectStringReferenceResolver : IExternalStringReferenceResolver
{
    // Multiple string reference resolvers can be chained together.
    public IExternalStringReferenceResolver NextResolver { get; set; }

    public bool CanReference(object value, out string id)
    {
        if (value is ScriptableObject)
        {
            id = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value as ScriptableObject));
            return true;
        }
        id = null;
        return false;
    }

    public bool TryResolveReference(string id, out object value)
    {
        value = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(id));
        return value != null;
    }
}

public class ScriptableObjectGuidReferenceResolver : IExternalGuidReferenceResolver
{
    // Multiple string reference resolvers can be chained together.
    public IExternalGuidReferenceResolver NextResolver { get; set; }

    public bool CanReference(object value, out Guid id)
    {
        if (value is ScriptableObject)
        {
            id = new Guid(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value as ScriptableObject)));
            return true;
        }
        id = default(Guid);
        return false;
    }

    public bool TryResolveReference(Guid id, out object value)
    {
        value = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(id.ToString("N")));
        return value != null;
    }
}
#endif