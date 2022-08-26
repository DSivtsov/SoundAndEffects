using UnityEngine;

[CreateAssetMenu(fileName = "LevelSO", menuName = "SoundAndEffects/LevelSO")]
public class LevelSO : ScriptableObject//, ISerializationCallbackReceiver
{
    //private string _levelSOName;
    public void OnAfterDeserialize()
    {
        //Debug.Log($"{this} : OnAfterDeserialize()");
    }

    public void OnBeforeSerialize()
    {
        //Debug.Log($"{this} : OnBeforeSerialize() = {this.name}");
    }
}