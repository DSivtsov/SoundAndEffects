public abstract class StoreObjects : MonoBehaviour, IStoreObjects - base for other classes

all files must attached to coresponded GameObjects:

StoreObjectsClass.cs - to Object which contain links to GameObjects with Data fields

StoreObjectsTransform.cs - to Objects which must contain the position of it

StoreObjectsCharacterController.cs - to Objects which contain the CharacterController, additionally the main script must relialize the ILoadData interface

StoreGame to GameObjects which was manager the process of calling the QuickLoad() & QuickSave() methods. In current state it's attached to GameManeger
 