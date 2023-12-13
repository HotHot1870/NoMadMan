
using UnityEngine;
using System.Linq;

[System.Serializable]
public enum GunScriptableStatEnum
{
    Damage,
    Pellet,
    ClipSize,
    FireRate,
    Accuracy,
    Handling,
    Recoil,
    ExplodeRadius,
    BulletType,
    IsSemiAuto
}

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


    public object GetStatValue(GunScriptableStatEnum statName){
        object ans = null;
        
        switch (statName)
        {
            case GunScriptableStatEnum.Damage:
                ans = GetUpgradedGunStat(statName, (object)GunStats.DamagePerPellet);
                break;
            case GunScriptableStatEnum.Pellet:
                ans = GetUpgradedGunStat(statName, (object)GunStats.PelletPerShot);
                break;
            case GunScriptableStatEnum.ClipSize:
                ans = GetUpgradedGunStat(statName, (object)GunStats.ClipSize);
                break;
            case GunScriptableStatEnum.FireRate:
                ans = GetUpgradedGunStat(statName, (object)GunStats.FireRate);
                break;
            case GunScriptableStatEnum.Accuracy:
                ans = GetUpgradedGunStat(statName, (object)GunStats.Accuracy);
                break;
            case GunScriptableStatEnum.Handling:
                ans = GetUpgradedGunStat(statName, (object)GunStats.Handling);
                break;
            case GunScriptableStatEnum.Recoil:
                ans = GetUpgradedGunStat(statName, (object)GunStats.Recoil);
                break;
            case GunScriptableStatEnum.ExplodeRadius:
                ans = GetUpgradedGunStat(statName, (object)ExplodeRadius);
                break;
            case GunScriptableStatEnum.BulletType:
                ans = GetUpgradedGunStat(statName, (object)GunStats.BulletType);
                break;
            case GunScriptableStatEnum.IsSemiAuto:
                string baseValue = GunStats.IsSemiAuto?"Yes":"No";
                ans = GetUpgradedGunStat(statName, (object)baseValue);
                break;
            default:
                break;
        }
        
        return ans;
    }

    public object GetUpgradedGunStat(GunScriptableStatEnum statName, object baseValue){
        var targetList = UpgradeScriptable.UpgradeDetails.Where(x=>x.UpgradeStat == statName).ToList();
        if(targetList.Count>0){
            string upgradeSaveKey = DisplayName+targetList[0].UpgradeStat.ToString() ;
            int upgradeCount = (int)MainGameManager.GetInstance().GetData<int>(upgradeSaveKey) ;
            if(upgradeCount-1 <0){
                return baseValue;
            }
            return targetList[0].CostAndValue[Mathf.Clamp(upgradeCount-1,0,targetList[0].CostAndValue.Count)].UpgradeValue;
        }
        return baseValue;

    }

    public float GetStatBaseValue(GunScriptableStatEnum statName){
        // TODO : use dictionary instead : working
        switch (statName)
        {
            case GunScriptableStatEnum.Damage:
                return GunStats.DamagePerPellet;
            case GunScriptableStatEnum.Pellet:
                return GunStats.PelletPerShot;
            case GunScriptableStatEnum.ClipSize:
                return GunStats.ClipSize;
            case GunScriptableStatEnum.FireRate:
                return GunStats.FireRate;
            case GunScriptableStatEnum.Accuracy:
                return GunStats.Accuracy;
            case GunScriptableStatEnum.Handling:
                return GunStats.Handling;
            case GunScriptableStatEnum.Recoil:
                return GunStats.Recoil;
            case GunScriptableStatEnum.ExplodeRadius:
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
    [Range (0.1f,100)]public float FireRate = 2;

    [Header("Max accuracy , 100 = always hit center")]
    [Range (0,100)]public float Accuracy = 50; 

    [Header("Min accuracy")]
    [Range (0,100)]public float Handling = 50; 
    
    [Header("Accuracy lose on shoot")]
    [Range (0,100)]public float Recoil = 50; 
}
