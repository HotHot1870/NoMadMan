using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Location", menuName = "Scriptable/Location", order = 3)]
public class MapLocationScriptable : ScriptableObject
{
    public int Id;
    public string DisplayName;
    public int WaveId;
    public GameObject Prefab;
    public Vector3 Pos;
    public int RewardGunId;
    [Range(10,100000)]public float FortifyCost = 1000f;
 
}
