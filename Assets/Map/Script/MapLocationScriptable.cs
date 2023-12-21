using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Location", menuName = "Scriptable/Location", order = 3)]
public class MapLocationScriptable : ScriptableObject
{
    public int Id;
    public string DisplayName;
    public List<int> LockBy = new List<int>();
    public int Level;
    public GameObject Prefab;
    public Vector3 Pos;
    public int StartDialogId;
    public int EndDialogId;
    
    [Header("Normal")]
    public int NormalWavesCount = 2;
    public float NormalWavesStrength = 10f;
    public List<int> NormalWaveEnemy = new List<int>();
    [Header("Final")]
    public float FinalWaveStrength = 50f;
    public List<int> FinalWaveEnemy = new List<int>();
 
}
