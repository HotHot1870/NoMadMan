using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Waves", menuName = "Scriptable/Waves", order = 2)]
public class WavesScriptable : ScriptableObject
{
    [Header("Normal")]
    public int NormalWavesCount = 2;
    public float NormalWavesDangerValue = 10f;
    public List<EnemyScriptable> NormalWaveEnemy = new List<EnemyScriptable>();
    [Header("Final")]
    public float FinalWavesDangerValue = 10f;
    public List<EnemyScriptable> FinalWaveEnemy = new List<EnemyScriptable>();


}
