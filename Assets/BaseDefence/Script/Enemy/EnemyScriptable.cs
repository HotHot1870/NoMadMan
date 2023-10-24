using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable/Enemy/Walker", order = 3)]
public class EnemyScriptable : ScriptableObject
{
    public int Id;
    public GameObject Prefab;
    public string DisplayName;
    public Sprite DisplayImage;
    [Range(0.1f,500)] public float MaxHp = 100;
    [Range(0.1f,100)] public float Damage = 33;
    [Range(0.1f,5f)] public float AttackDelay = 1.5f;
    [Range(1, 25)] public float MoveSpeed = 5;
    [Range(1f, 100)]public float DangerValue;
    [Range(0f, 1000)]public float GooOnKill=10;
}
