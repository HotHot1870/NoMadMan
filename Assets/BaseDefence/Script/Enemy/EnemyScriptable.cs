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
    public float MaxHp = 100;
    public float Damage = 33;
    public float AttackDelay = 1.5f;
    public float MoveSpeed = 5;
    public float DangerValue;
    public float GooOnKill=10;
    public float ExplodeDamageMod=1;
}
