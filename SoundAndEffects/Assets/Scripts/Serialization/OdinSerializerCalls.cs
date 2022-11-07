// <copyright file="OdinSerializerCalls.cs">
//  Copyright (c) 2022 Denis Sivtsov.
//  Licensed under the Apache License, Version 2.0
// </copyright>

using OdinSerializer;
using System.IO;
using UnityEngine;
public static class OdinSerializerCalls
{
    private static readonly DataFormat[] arr = new DataFormat[] { DataFormat.JSON, DataFormat.Binary };
    private static DataFormat FormatData = arr[0];
    public static void SetFormat(int toolbarInt) => FormatData = arr[toolbarInt];

    #region StoreByOdinModUnityObject
    public static void SaveUnityObject(Object data, string filePath)
    {
        SerializationContext serContext = new SerializationContext()
        {
            StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<ComplexitySO>(),
        };
        byte[] bytes = default;
        UnitySerializationUtilityMod.SerializeUnityObject(data, ref bytes, FormatData, serializeUnityFields: true, context: serContext);
        File.WriteAllBytes(filePath, bytes);
    }

    public static void LoadUnityObject(Object data, string filePath)
    {
        DeserializationContext desContext = new DeserializationContext()
        {
            StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<ComplexitySO>(),
        };
        byte[] bytes = File.ReadAllBytes(filePath);
        UnitySerializationUtilityMod.DeserializeUnityObject(data, ref bytes, FormatData, desContext);
    }
    #endregion
}