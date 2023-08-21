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
    [Header("How strong the shake")]
    [Range (1,25)]public float CameraShakeStrength = 2;
    [Header("How many time the camera bounce back and forth in one shake")]
    [Range (1,100)]public float CameraShakeAmount = 10;
    [Header("True if holding down shoot btn will shoot continuously")]
    public bool IsSemiAuto = true;

    [Header("How many ammo could be shot out every 1 second")]
    [Range (1,25)]public float FireRate = 2;

    [Header("Max accuracy , 100 = always hit center")]
    [Range (0,100)]public float Accuracy = 50; 

    [Header("Affect accuracy lose on shoot , the higher the recoil control , the less accruacy lose when shoot")]
    [Range (0,100)]public float RecoilControl = 50; 

    [Header("Affect accuracy lose on moving , the higher the stability , the less accruacy lose while moving")]
    [Range (0,100)]public float Stability = 50;  
    
    [Header("Acuracy gain overtime")]
    [Range (0.1f,100)]public float Handling = 50; 


    
    [Space(20)]
    [Header("Reload")]
    public GunReloadScriptable ReloadScriptable;
    

}
