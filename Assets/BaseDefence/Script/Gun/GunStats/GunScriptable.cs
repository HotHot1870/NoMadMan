using System.Collections;
using System.Collections.Generic;
using GunReloadScriptableNameSpace;
using UnityEngine;
using MainGameNameSpace;


[CreateAssetMenu(fileName = "Gun", menuName = "Scriptable/Weapon", order = 1)]
public class GunScriptable : ScriptableObject
{
    [Header("For Weapon selection panel in DayTime scene")]
    public string DisplayName;
    [Range(0,1f)]public float ShakeAmount = 0.1f;
    public Sprite DisplayImage;
    public GameObject FPSPrefab;
    public Vector3 FPSPos;
    public Vector3 FPSRot;
    public Vector3 FPSScale = Vector3.one;
    public float Damage;
    public int PelletPerShot = 1;
    public AudioClip ShootSound;
    public float ClipSize;

    [Header("In switch weapon panel")]
    public Vector3 ReloadPos;
    public Vector3 ReloadRot;
    public Vector3 ReloadScale = Vector3.one;
    public AudioClip OutOfAmmoSound;
    [Header("True if holding down shoot btn will shoot continuously")]
    public bool IsSemiAuto = true;

    [Header("How many ammo could be shot out every 1 second")]
    [Range (1,25)]public float FireRate = 2;

    [Header("Max accuracy , 100 = always hit center")]
    [Range (0,100)]public float Accuracy = 50; 

    [Header("Min accuracy")]
    [Range (0,100)]public float RecoilControl = 50; 
    
    [Header("Accuracy lose on shoot")]
    [Range (0,100)]public float Recoil = 50; 


    
    [Space(20)]
    [Header("Reload")]
    public GunReloadScriptable ReloadScriptable;
    
}
