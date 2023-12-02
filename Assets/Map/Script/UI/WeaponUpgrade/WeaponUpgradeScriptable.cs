using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Gun", menuName = "Scriptable/WeaponUpgrade", order = 1)]
public class WeaponUpgradeScriptable : ScriptableObject
{
    public int Id;
    public int WeaponId;
    public List<WeaponUpgradeDetail> UpgradeDetails = new List<WeaponUpgradeDetail>();

}

[System.Serializable]
public class WeaponUpgradeDetail
{
    public string UpgradeStat;
    public List<WeaponUpgradeCostAndValue> CostAndValue = new List<WeaponUpgradeCostAndValue>();

}


[System.Serializable]
public class WeaponUpgradeCostAndValue
{
    public float Cost;
    public string UpgradeValue;

}


