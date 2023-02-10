using System;
using UnityEngine;
using System.IO;
using System.Text;

namespace GMTools.Manager
{
    [Flags]
    public enum IOError : byte
    {
        NoError = 0b0,
        FileNotFound = 0b1,
        WrongFormat = 0b10,
        NotInitilized = 0b11,
    }

    /// <summary>
    /// WARNING Not support the Changing the order of records in the arrays (in objects derived from Store class),
    /// which store the sources of information, after Data from this sources was saved to file.
    /// Not Garanted the possibility of Data restoring in this case
    /// </summary>
    public class StoreTopListController : MonoBehaviour
    {
        [SerializeField] private string nameFile = "TopList.txt";
        private IStoredObject _storedObject;
        private bool _storeObjectControllerLinked = false;

        public StoredObject GetStoredObject() => (StoredObject)_storedObject;

        private void Awake()
        {
            SetIStoreObject();
        }

        public void SetIStoreObject()
        {
            _storedObject = GetComponent<StoredObject>();
            if (_storedObject != null)
            {
                _storeObjectControllerLinked = true; 
            }
        }

        public string GetNameFile() => nameFile;

        public IOError Save()
        {
            if (!_storeObjectControllerLinked)
            {
                Debug.LogError($"{this} : Save() : Save skipped - _storeObjectControllerNotLinked ==  true");
                return IOError.NotInitilized;
            }
            using (StreamWriter sw = new StreamWriter(nameFile, false, Encoding.UTF8, 1024))
            {
                string[] jsonArray = _storedObject.ToJsonBeforeSave();
                if (jsonArray == null || jsonArray.Length == 0)
                {
                    Debug.LogError($"{this} : Save() : Save skipped - stored object is empty");
                    return IOError.WrongFormat;
                }
                byte numValues = (byte)jsonArray.Length;
                sw.WriteLine(numValues);
                for (byte i = 0; i < numValues; i++)
                {
                    sw.WriteLine(jsonArray[i]);
                }
            }
            return IOError.NoError;
        }

        enum TypeDataRead
        {
            NumElements,
            Elements
        }

        public IOError Load()
        {
            if (!_storeObjectControllerLinked)
            {
                Debug.LogError($"{this} : Load() : _storeObjectControllerNotLinked ==  true");
                return IOError.NotInitilized;
            }
            if (!File.Exists(nameFile))
            {
                Debug.LogWarning($"{this} : Load() : [{nameFile}] file which stores the local TopList, not found will be created new at first Save");
                return IOError.FileNotFound; 
            }
            TypeDataRead currentMode = TypeDataRead.NumElements;
            using (StreamReader sr = new StreamReader(nameFile, Encoding.UTF8, false, 1024))
            {
                string str = sr.ReadLine();
                string[] jsonArray = default(string[]);
                byte numElements = 0;
                byte idx = 0;
                while (str != null)
                {
                    switch (currentMode)
                    {
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
                                    Debug.LogError($"{this} : Load() : Detected numElements == 0");
                                    sr.Close();
                                    return IOError.WrongFormat;
                                }
                            }
                            else
                            {
                                Debug.LogError($"{this} : Load() : Format of the saved file ({nameFile}) is wrong");
                                sr.Close();
                                return IOError.WrongFormat;
                            }
                            break;
                        case TypeDataRead.Elements:
                            jsonArray[idx++] = str;
                            if (idx == numElements)
                            {
                                _storedObject.FromJsonAfterLoad(jsonArray);
                            }
                            break;
                    }
                    str = sr.ReadLine();
                }
                return IOError.NoError;
            }
        }

        public bool DeleteFile()
        {
            try
            {
                File.Delete(nameFile);
            }
            catch (Exception e)
            {
                Debug.LogError($"{this} : DeleteFile() : Can't delete file ({nameFile}) error [{e.Message}]");
                return false;
            }
            return true;
        }
    }
}
