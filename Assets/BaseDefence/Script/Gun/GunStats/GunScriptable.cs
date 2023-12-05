using System.Collections;
using System.Collections.Generic;
using GunReloadScriptableNameSpace;
using UnityEngine;
using MainGameNameSpace;
using Unity.VisualScripting;
using System.Linq;


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


    public object GetStatValue(string statName){
        // TODO : get enum instead of string
        object ans = null;
        
        switch (statName)
        {
            case "Damage":
                ans = GetUpgradedGunStat(statName, (object)GunStats.DamagePerPellet);
                break;
            case "Pellet":
                ans = GetUpgradedGunStat(statName, (object)GunStats.PelletPerShot);
                break;
            case "ClipSize":
                ans = GetUpgradedGunStat(statName, (object)GunStats.ClipSize);
                break;
            case "FireRate":
                ans = GetUpgradedGunStat(statName, (object)GunStats.FireRate);
                break;
            case "Accuracy":
                ans = GetUpgradedGunStat(statName, (object)GunStats.Accuracy);
                break;
            case "Handling":
                ans = GetUpgradedGunStat(statName, (object)GunStats.Handling);
                break;
            case "Recoil":
                ans = GetUpgradedGunStat(statName, (object)GunStats.Recoil);
                break;
            case "ExplodeRadius":
                ans = GetUpgradedGunStat(statName, (object)ExplodeRadius);
                break;
            case "BulletType":
                 // TODO : bullet type
                ans = (object)GunStats.BulletType;
                break;
            case "IsSemiAuto":
                ans = (object)"False";
                break;
            default:
                break;
        }
        
        return ans;
    }

    public object GetUpgradedGunStat(string typeName, object baseValue){
        var targetList = UpgradeScriptable.UpgradeDetails.Where(x=>x.UpgradeStat == typeName).ToList();
        if(targetList.Count>0){
            string upgradeSaveKey = DisplayName+targetList[0].UpgradeStat.ToString() ;
            int upgradeCount = (int)MainGameManager.GetInstance().GetData<int>(upgradeSaveKey) -1;
            if(upgradeCount <=0){
                return baseValue;
            }
            return targetList[0].CostAndValue[upgradeCount].UpgradeValue;
        }
        return baseValue;

    }

    public float GetStatBaseValue(string statName){
        // TODO : use dictionary instead
        switch (statName)
        {
            case "Damage":
                return GunStats.DamagePerPellet;
            case "Pellet":
                return GunStats.PelletPerShot;
            case "ClipSize":
                return GunStats.ClipSize;
            case "FireRate":
                return GunStats.FireRate;
            case "Accuracy":
                return GunStats.Accuracy;
            case "Handling":
                return GunStats.Handling;
            case "Recoil":
                return GunStats.Recoil;
            case "ExplodeRadius":
                return ExplodeRadius;
            default:
            return 0;
        }
    }
    
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
