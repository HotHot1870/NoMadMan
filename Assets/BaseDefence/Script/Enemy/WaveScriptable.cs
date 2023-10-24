using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Waves", menuName = "Scriptable/Waves", order = 2)]
public class WaveScriptable : ScriptableObject
{
    public int Id;
    [Header("Normal")]
    public int NormalWavesCount = 2;
    public float NormalWavesStrength = 10f;
    public List<int> NormalWaveEnemy = new List<int>();
    [Header("Final")]
    public float FinalWaveStrength = 50f;
    public List<int> FinalWaveEnemy = new List<int>();


}
