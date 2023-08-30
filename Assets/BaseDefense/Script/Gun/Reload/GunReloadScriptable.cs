using System.Collections;
using System.Collections.Generic;
using GunReloadScriptableNameSpace;
using UnityEngine;

namespace GunReloadScriptableNameSpace
{
    [System.Serializable]
    public class GunReloadDragFunction
    {
        [Header ("Start Drag")]
        public AudioClip StartDragSound;
        public string StartDragUnderText;
        public Vector2 StartDragPosition;
        public Sprite MainGunSpriteOnStart;

        [Header ("Draging")]
        public AudioClip CancelDragSound;
        public GameObject DragCursorPrefab;

        [Header ("End Drag")]
        public AudioClip EndDragSound;
        public string EndDragUnderText;
        public Vector2 EndDragPosition;
        public Sprite MainGunSpriteOnEnd;



        public GunReloadActionResult ResultAction;
    }
    [System.Serializable]
    public class GunReloadTapFunction
    {
        public AudioClip TapEndSound;
        public Vector2 Position;
        public string UnderText;
        public Sprite MainGunSpriteOnEnd;
        public GunReloadActionResult ResultAction;

    }

    [System.Serializable]
    public class GunReloadExtraImage
    {
        public Vector2 Position;
        public GameObject ImagePrefab;

    }

    [System.Serializable]
    public class GunReloadPhase
    {
        public List<GunReloadDragFunction> DragFunction = new List<GunReloadDragFunction>();
        public List<GunReloadTapFunction> TapFunction = new List<GunReloadTapFunction>();
        public List<GunReloadExtraImage> ExtraImages = new List<GunReloadExtraImage>();
        
    }


    [System.Flags]
    public enum GunReloadActionResult
    {   
        None ,
        CancelReload = 1 << 1 ,
        GainOneAmmo = 1 << 2 , // do nothing if ammo already reach maximum 
        FullAmmoReload = 1 << 3 ,
        ToNextPhase = 1 << 4 ,
        PhaseChangeByAmmo = 1 << 5 , // for shotgun
        SetClipAmmoToZero = 1 << 6 ,


    }
}

[CreateAssetMenu(fileName = "Gun", menuName = "Scriptable/Reload", order = 2)]
public class GunReloadScriptable : ScriptableObject
{
    public Sprite StartMainGunImage; // image tobe use at the start of reloading
    public Vector2 MainGunSize;
    public List<GunReloadPhase> ReloadPhases = new List<GunReloadPhase>();
}
