using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerType",menuName = "ScriptableObject/Create Spawner Type")]
public class SpawnerTypeSO : ScriptableObject
{
    public float DistanceBeforeMin;
    public float DistanceBeforeMax;
    public float DistanceAfter;
}
