using System.Collections.Generic;
using UnityEngine;
using OdinSerializer;
using System.IO;
using OdinSerializer.Utilities;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;
public static class OdinCallsExample
{
    private const DataFormat FormatData = DataFormat.JSON;

    #region StoreByOdinClassicTwoSteps
    public static void SaveTwoSteps<T>(T data, string filePath, ref List<Object> unityReferences)
    {
        byte[] bytes = SerializationUtility.SerializeValue(data, FormatData, out unityReferences);
        File.WriteAllBytes(filePath, bytes);
    }

    public static T LoadTwoSteps<T>(string filePath, List<Object> unityReferences)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        return SerializationUtility.DeserializeValue<T>(bytes, FormatData, unityReferences);
    }
    #endregion

    #region StoreByOdinNewOneStepSystemObject
    public static void Save<T>(T data, string filePath, ref List<Object> unityReferences)
    {
        SerializationContext serContext = new SerializationContext()
        {
            //StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<ComplexitySO>(),
            StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<LevelSO>(),
        };
        byte[] bytes = SerializationUtility.SerializeValue(data, FormatData, serContext);
        File.WriteAllBytes(filePath, bytes);
    }

    public static T Load<T>(string filePath, List<Object> unityReferences)
    {
        DeserializationContext desContext = new DeserializationContext()
        {
            //StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<ComplexitySO>(),
            StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<LevelSO>(),
        };
        byte[] bytes = File.ReadAllBytes(filePath);
        return SerializationUtility.DeserializeValue<T>(bytes, FormatData, desContext);
    }
    #endregion

    #region StoreByOdinNewOneStepUnityObject
    public static void SaveUnityObject(Object data, string filePath, ref List<Object> unityReferences)
    {
        SerializationContext serContext = new SerializationContext()
        {
            //StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<LevelSO>(),
            StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<NewGameSettingsSO>()
            {
                NextResolver = new UniversalScriptableObjectStringReferenceResolver<LevelSO>()
            },
        };
        byte[] bytes = default;
        //UnitySerializationUtility.SerializeUnityObject(data, ref bytes, ref unityReferences, FormatData, serializeUnityFields: true, context: serContext);
        UnitySerializationUtilityMod.SerializeUnityObject(data, ref bytes, FormatData, serializeUnityFields: true, context: serContext);
        File.WriteAllBytes(filePath, bytes);
    }

    public static void LoadUnityObject(Object data, string filePath, ref List<Object> unityReferences)
    {
        DeserializationContext desContext = new DeserializationContext()
        {
            StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<LevelSO>(),
        };
        byte[] bytes = File.ReadAllBytes(filePath);
        //UnitySerializationUtility.DeserializeUnityObject(data, ref bytes, ref unityReferences, FormatData, desContext);
        UnitySerializationUtilityMod.DeserializeUnityObject(data, ref bytes, FormatData, desContext);
    }
    #endregion

    #region StoreByOdinUniversalPlainClass
    public static void SaveUnityObjectPlainClass<T>(T data, string filePath, ref List<Object> unityReferences) where T : UnityEngine.Object
    {
        byte[] bytes = default;

        SerializationContext myContext = new SerializationContext();
        myContext.Config.SerializationPolicy = myUnitySerializationPolicy;
        UnitySerializationUtility.SerializeUnityObject(data, ref bytes, ref unityReferences, FormatData, serializeUnityFields: true, context: myContext);
        File.WriteAllBytes(filePath, bytes);
    }

    public static void LoadUnityObjectPlainClass<T>(T data, string filePath, ref List<Object> unityReferences) where T : UnityEngine.Object
    {
        DeserializationContext myContext = new DeserializationContext();
        myContext.Config.SerializationPolicy = myUnitySerializationPolicy;

        byte[] bytes = File.ReadAllBytes(filePath);

        UnitySerializationUtility.DeserializeUnityObject(data, ref bytes, ref unityReferences, FormatData, context: myContext);
    }
    // In Unity 2017.1's .NET 4.6 profile, Tuples implement System.ITuple. In Unity 2017.2 and up, tuples implement System.ITupleInternal instead for some reason.
    private static Type tupleInterface = typeof(string).Assembly.GetType("System.ITuple") ?? typeof(string).Assembly.GetType("System.ITupleInternal");
    /// <summary>
    /// The member which will return the false it will be excluded from serialization
    /// </summary>
    private static ISerializationPolicy myUnitySerializationPolicy = new CustomSerializationPolicy("OdinSerializerPolicies.Unity", true, (member) =>
    {
        /*
         * Use the NonOdinSerializedAttribute
         */
        if (member.IsDefined<NonOdinSerializedAttribute>(true))
        {
            Debug.Log($"ISerializationPolicy : member={member.Name} will exclude from serialization");
            return false;
        }

        // As of Odin 3.0, we now allow non-auto properties and virtual properties.
        // However, properties still need a getter and a setter.
        if (member is PropertyInfo)
        {
            var propInfo = member as PropertyInfo;
            if (propInfo.GetGetMethod(true) == null || propInfo.GetSetMethod(true) == null) return false;
        }

        // If OdinSerializeAttribute is defined, NonSerializedAttribute is ignored.
        // This enables users to ignore Unity's infinite serialization depth warnings.
        if (member.IsDefined<NonSerializedAttribute>(true) && !member.IsDefined<OdinSerializeAttribute>())
        {
            return false;
        }

        if (member is FieldInfo && ((member as FieldInfo).IsPublic || (member.DeclaringType.IsNestedPrivate && member.DeclaringType.IsDefined<CompilerGeneratedAttribute>())
        || (tupleInterface != null && tupleInterface.IsAssignableFrom(member.DeclaringType))))
        {
            return true;
        }

        return member.IsDefined<SerializeField>(false) || member.IsDefined<OdinSerializeAttribute>(false) || (UnitySerializationUtility.SerializeReferenceAttributeType != null
        && member.IsDefined(UnitySerializationUtility.SerializeReferenceAttributeType, false));
    });

    #endregion

    #region StoreByOdinClassicRefResolversEditorOnly
#if UNITY_EDITOR
    public static void SaveGuiDRef(string filePath, List<Object> unityReferences)
    {
        var serContext = new SerializationContext()
        {
            GuidReferenceResolver = new ScriptableObjectGuidReferenceResolver(),
        };
        byte[] bytes = Serialize(unityReferences[0], serContext);
        File.WriteAllBytes(filePath, bytes);
    }

    public static void LoadGuiDRef(string filePath, List<Object> unityReferences, bool clearUnityReferences = true)
    {
        if (clearUnityReferences)
        {
            unityReferences.Clear();
        }
        byte[] bytes = File.ReadAllBytes(filePath);
        var desContext = new DeserializationContext()
        {
            GuidReferenceResolver = new ScriptableObjectGuidReferenceResolver(),
        };
        unityReferences.Add((Object)Deserialize(bytes, desContext));
        //return (Object)Deserialize(bytes, context);
    }
#endif
    #endregion

    #region StoreUnityRefByUniversalRefResolvers
    /*
     * They Store the Unity Refs which they get from List<Object> unityReferences 
     */
    private static string FileName((string fileBase, string fileExt) fileName, int num) => fileName.fileBase + num + fileName.fileExt;

    public static void SaveStrRef((string fileBase, string fileExt) fileName, List<Object> unityReferences)
    {
        //SerializationContext serContext = new SerializationContext()
        //{
        //    StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver(),
        //};

        SerializationContext serContext = new SerializationContext()
        {
            StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<ComplexitySO>(),
        };

        for (int i = 0; i < unityReferences.Count; i++)
        {
            byte[] bytes = Serialize(unityReferences[i], serContext);
            File.WriteAllBytes(FileName(fileName, i), bytes);
        }
    }

    public static void LoadStrRef((string fileBase, string fileExt) fileName, List<Object> unityReferences, bool clearUnityReferences = true)
    {
        //var desContext = new DeserializationContext()
        //{
        //    StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver(),
        //};
        DeserializationContext desContext = new DeserializationContext()
        {
            StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<ComplexitySO>(),
        };
        if (clearUnityReferences)
        {
            unityReferences.Clear();
        }
        int i = 0;
        string currentFileName = FileName(fileName, i);
        while (File.Exists(currentFileName))
        {
            byte[] bytes = File.ReadAllBytes(currentFileName);
            unityReferences.Add((Object)Deserialize(bytes, desContext));
            currentFileName = FileName(fileName, ++i);
        }
    }
    #endregion

    #region CommonForOdinRefResolvers
    static byte[] Serialize(object obj, SerializationContext context)
    {
        return SerializationUtility.SerializeValue(obj, FormatData, context);
    }

    static object Deserialize(byte[] bytes, DeserializationContext context)
    {
        return SerializationUtility.DeserializeValue<object>(bytes, FormatData, context);
    } 
    #endregion
}