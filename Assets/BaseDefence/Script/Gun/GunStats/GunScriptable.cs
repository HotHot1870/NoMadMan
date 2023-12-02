using System.Collections;
using System.Collections.Generic;
using GunReloadScriptableNameSpace;
using UnityEngine;
using MainGameNameSpace;


[CreateAssetMenu(fileName = "Gun", menuName = "Scriptable/Weapon", order = 1)]
public class GunScriptable : ScriptableObject
{
    public int Id ;
    [Header("For Weapon selection panel in DayTime scene")]
    public string DisplayName;
    public float UnlockCost = 10;
    public Sprite DisplayImage;
    public GameObject FPSPrefab;
    public AudioClip ShootSound;

    public GunScriptableStats GunStats;


    
    [Space(20)]
    [Header("Reload")]
    public GunReloadScriptable ReloadScriptable;
    [Space(20)]
    [Header("Upgrade")]
    public WeaponUpgradeScriptable UpgradeScriptable;

    [Space(20)]
    [Header("Other")]
    public float ExplodeRadius;
    
}

[System.Serializable]
public class GunScriptableStats{
    public float DamagePerPellet;
    public int PelletPerShot = 1;
    public float ClipSize;
    public BulletType BulletType;

    [Header("True if holding down shoot btn will shoot continuously")]
    public bool IsSemiAuto = true;

    [Header("How many ammo could be shot out every 1 second")]
    [Range (0.1f,25)]public float FireRate = 2;

    [Header("Max accuracy , 100 = always hit center")]
    [Range (0,100)]public float Accuracy = 50; 

    [Header("Min accuracy")]
    [Range (0,100)]public float Handling = 50; 
    
    [Header("Accuracy lose on shoot")]
    [Range (0,100)]public float Recoil = 50; 
}
