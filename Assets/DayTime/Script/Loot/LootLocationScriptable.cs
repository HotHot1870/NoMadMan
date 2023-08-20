using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootLocation", menuName = "Scriptable/LootLocation", order = 4)]
public class LootLocationScriptable : ScriptableObject
{
    public string DisplayName;
    [Header("Show in loot panel")]
    public Sprite Icon;
    public Vector2 Size = new Vector2(150f,150f);
    public Vector2 Position;
    [Header("Show in detail panel ( size 4 x 3)")]
    public Sprite DetailImage;
    [Range(-20f,20f)]public float HeatGainOnLoot;
    [Header("Total Safeness = BaseSafeness + SaftnessGainPreExtraBot * (Extra bot count ) \nExtra bot count = ( total bot in looing mission - 1 ) ( low cap at 0 )")]
    [Range(0f,100f)]public float BaseSafeness = 75;
    [Range(-50f,50f)]public float SaftnessGainPreExtraBot = 10;
    [Header("Material ")]
    [Range(0f,9000f)]public float MinRawMaterial;
    [Range(0f,9000f)]public float MaxRawMaterial;
    [Range(0f,9000f)]public float MinScrapMaterial;
    [Range(0f,9000f)]public float MaxScrapMaterial;
    [Range(0f,9000f)]public float MinChemMaterial;
    [Range(0f,9000f)]public float MaxChemMaterial;
    [Range(0f,9000f)]public float MinElectronicMaterial;
    [Range(0f,9000f)]public float MaxElectronicMaterial;


}
