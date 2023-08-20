using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable/Enemy/Walker", order = 3)]
public class EnemyScriptable : ScriptableObject
{
    public GameObject Prefab;
    [Range(0.1f,500)] public float MaxHp = 100;
    [Range(0.1f,100)] public float Damage = 33;
    [Range(1, 50)] public float MoveSpeed = 5;
    [Range(0f, 100)]public float TargetTotalHeatForSpawn;
    [Range(0.1f, 50)]public float HeatGainForSpawn;
}
