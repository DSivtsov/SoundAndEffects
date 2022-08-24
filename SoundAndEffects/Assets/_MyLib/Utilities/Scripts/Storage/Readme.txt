Ver.3.0
Changes:
- Every objects using the class derived from Store must select the certain StoreGame object
- Every StoreGame object have own file (field nameFile) to store data from related "Store" objects
Therefore, it makes it possible to separate storages of different data by using different files.

for all "objects" which must be saved added special classes,
the base for these classes
- public abstract class Store : MonoBehaviour, IStore 

Every "special classes" cn be attached to any GameObjects, every class contain the coresponded array for specific stored Data:

contains the array with links to a GameObjects with MonoBehaviour scripts with Serialized fields, data of it can be stored on disk (data can be stored only from One MonoBehaviour script from specific GameObject).

- StoreTransform.cs
contains an array with references to GameObjects, their Transform data can be stored on disk

- StoreObjectsCharacterController.cs
contains an array with references to GameObjects, which contain the "CharacterController", additionally the "CharacterController" script must relialize the ILoadData interface (to give possibility find the "Right time & place" for Restore)

- StoreObjectT<T>.cs
It's a base class for any derived classes (they can be empty) where T is the plain C# class with Serialized fields, data of it can be stored on disk. Use the method T[] GetLoadedStoreObjects() and SetStoreObject(T[] arrObjects) to get or set the T array after load or before save data from/to disk.

- StoreGame
to GameObjects which was manager the process of calling the QuickLoad() & QuickSave() methods.

The "special classes" at Start() added self to "backup pool", which used by StoreGame.cs

The default Save format:
 new StreamWriter(nameFile, false, Encoding.UTF8, 1024)
 nameFile = "test.txt":
	- in Editor Play mode the default folder is the root of Project folder Unity.
	- in Build Play mode the default folder is the root of Game (where the exe file).
 all data serialized to JSON vis JsonUtility.ToJson()
 every objects write separetly in the one file

Every record have the next format:
227bccd6-6372-4a7d-9a8f-6ea8d040bcc3 -  Guid of the object
1 - number Data records of this object
{"_maxHealth":0,"_health":0,"_summaryDistance":0,"_summaryScore":3,"_userName":""} - the Data records
