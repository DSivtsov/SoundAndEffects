using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace GMTools.Manager
{
    public interface IStore
    {
        public string[] Save();
        public void Load(string[] streamArr);
    }

    public static class ObjectPool
    {
        public static Dictionary<string, IStore> storeObjectsPool = new Dictionary<string, IStore>(StringComparer.Ordinal);

        public static void AddObject(string guid, IStore obj)
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
    /// <summary>
    /// WARNING Not support the Changing the order of records in the arrays (in objects derived from Store class),
    /// which store the sources of information, after Data from this sources was saved to file.
    /// Not Garanted the possibility of Data restoring in this case
    /// </summary>
    public class StoreGame : MonoBehaviour
    {
        const string nameFile = "TopList.txt";
        public void QuickSave()
        {
            Debug.Log($"StoreGame : QuickSave({ObjectPool.storeObjectsPool.Count} objects)");
            using (StreamWriter sw = new StreamWriter(nameFile, false, Encoding.UTF8, 1024))
            {
                foreach (var item in ObjectPool.storeObjectsPool)
                {
                    string[] jsonArray = item.Value.Save();
                    //if (jsonArray == null || jsonArray.Length == 0)
                    if (jsonArray == null)
                    {
                        //Debug.Log("QuickSave() : item.Value.Save() ==  null || jsonArray.Length == 0");
                        Debug.Log("QuickSave() : item.Value.Save() ==  null");
                        continue; 
                    }
                    //This means TopList is empty
                    if (jsonArray.Length == 0)
                        continue;
                    sw.WriteLine(item.Key);
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
                    switch (currentMode)
                    {
                        case TypeDataRead.GUID:
                            guid = str;
                            currentMode = TypeDataRead.NumElements;
                            break;
                        case TypeDataRead.NumElements:
                            if (Byte.TryParse(str, out numElements))
                            {
                                if (numElements != 0)
                                {
                                    jsonArray = new string[numElements];
                                    idx = 0;
                                    currentMode = TypeDataRead.Elements; 
                                }
                                else
                                {
                                    Debug.LogWarning($"StoreGame : QuickLoad() - Detected numElements == 0. Restore {guid} skipped");
                                    currentMode = TypeDataRead.GUID;
                                }
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
                                try
                                {
                                    ObjectPool.storeObjectsPool[guid].Load(jsonArray);
                                }
                                catch (KeyNotFoundException e)
                                {
                                    Debug.LogError($"StoreGame : QuickLoad() [{guid}] {e.Message}");
                                    Debug.LogWarning($"StoreGame : QuickLoad() restoring for [{guid}] skipped");
                                    //throw;
                                }
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
