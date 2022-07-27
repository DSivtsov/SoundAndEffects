using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectionName
{
    Walking,
    Died
}

[System.Serializable]
public class JukeBoxCollection
{
    public CollectionName collectionName;
    public JukeBoxSO jukeBoxSO;
}

public class PlayJukeBoxCollection : PlayJukeBox
{
    [SerializeField] private  JukeBoxCollection[] jukeBoxCollections;

    public void SwitchCollection(CollectionName newCollectionName, bool turnOnMusicAfterSwitch = true)
    {
        TurnOn(false);
        for (int i = 0; i < jukeBoxCollections.Length; i++)
        {
            if (jukeBoxCollections[i].collectionName == newCollectionName)
            {
                _jukeBox = jukeBoxCollections[i].jukeBoxSO;
            }
        }
        TurnOn(turnOnMusicAfterSwitch);
    }
}
