using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Location", menuName = "Scriptable/Location", order = 3)]
public class MapLocationScriptable : ScriptableObject
{
    public string LocationName;
    public WavesScriptable WaveData;
    public GameObject Prefab;
    public Vector3 Pos;
    public GunScriptable Reward;
    [Range(10,100000)]public float FortifyCost = 1000f;
 
}
