using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace GMTools.Manager
{
    public interface IStoreObjects
    {
        public string[] Save();
        public void Load(string[] streamArr);
    }

    public static class ObjectPool
    {
        public static Dictionary<string, IStoreObjects> storeObjectsPool = new Dictionary<string, IStoreObjects>(StringComparer.Ordinal);

        public static void AddObject(string guid, IStoreObjects obj)
        {
            try
            {
                storeObjectsPool.Add(guid, obj);
            }
            catch (ArgumentException)
            {
                Debug.LogWarning($"Object ({obj}) exist in Dictionary");
            }
        }
    }

    public class StoreGame : MonoBehaviour
    {
        const string nameFile = "test.txt";
        public void QuickSave()
        {
            Debug.Log($"StoreGame : QuickSave({ObjectPool.storeObjectsPool.Count} objects)");
            using (StreamWriter sw = new StreamWriter(nameFile, false, Encoding.UTF8, 1024))
            {
                foreach (var item in ObjectPool.storeObjectsPool)
                {
                    sw.WriteLine(item.Key);
                    string[] jsonArray = item.Value.Save();
                    byte numValues = (byte)jsonArray.Length;
                    sw.WriteLine(numValues);
                    Debug.Log($"numValues={numValues}");
                    for (byte i = 0; i < numValues; i++)
                    {
                        sw.WriteLine(jsonArray[i]);
                    }
                }
            }
        }

        enum TypeDataRead
        {
            GUID,
            NumElements,
            Elements
        }

        public void QuickLoad()
        {
            TypeDataRead currentMode = TypeDataRead.GUID;
            using (StreamReader sr = new StreamReader(nameFile, Encoding.UTF8, false, 1024))
            {
                string str = sr.ReadLine();
                string guid = "";
                string[] jsonArray = new string[1];
                byte numElements = 0;
                byte idx = 0;
                while (str != null)
                {
                    Debug.Log(str);
                    switch (currentMode)
                    {
                        case TypeDataRead.GUID:
                            guid = str;
                            currentMode = TypeDataRead.NumElements;
                            break;
                        case TypeDataRead.NumElements:
                            if (Byte.TryParse(str, out numElements))
                            {
                                jsonArray = new string[numElements];
                                idx = 0;
                                currentMode = TypeDataRead.Elements;
                            }
                            else
                            {
                                Debug.LogError($"StoreGame : QuickLoad() - Format of the saved file ({nameFile}) is wrong. Restore interrupted");
                                sr.Close();
                                return;
                            }
                            break;
                        case TypeDataRead.Elements:
                            jsonArray[idx++] = str;
                            if (idx == numElements) 
                            {
                                var z = ObjectPool.storeObjectsPool[guid];
                                ObjectPool.storeObjectsPool[guid].Load(jsonArray);
                                currentMode = TypeDataRead.GUID; 
                            }
                            break;
                    }
                    str = sr.ReadLine();
                }
                if (currentMode != TypeDataRead.GUID)
                {
                    Debug.LogWarning($"StoreGame : QuickLoad() - EOF of the saved file ({nameFile}) is wrong. ");
                }

            }
        }
    }
}
