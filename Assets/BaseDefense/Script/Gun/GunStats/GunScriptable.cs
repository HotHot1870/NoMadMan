using System.Collections;
using System.Collections.Generic;
using GunReloadScriptableNameSpace;
using UnityEngine;
using MainGameNameSpace;


[CreateAssetMenu(fileName = "Gun", menuName = "Scriptable/Weapon", order = 1)]
public class GunScriptable : ScriptableObject
{
    public string DisplayName;
    [Header("For Weapon selection panel in DayTime scene")]
    public Sprite DisplaySprite;
    public float Damage;
    public int PelletPerShot = 1;
    public AudioClip ShootSound;
    public float ClipSize;
    public Sprite FPSSprite;
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
