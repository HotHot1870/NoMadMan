using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapWeaponListGrid : MonoBehaviour
{
    [SerializeField] private Image m_Border;
    [SerializeField] private Button m_Btn;
    [SerializeField] private Image m_WeaponDisplayImage;
    private GunScriptable m_GunScriptable;

    private int m_WeaponSlotIndex;

    public void Init(GunScriptable gunScriptable, int weaponSlotIndex){
        m_GunScriptable = gunScriptable;
        m_WeaponDisplayImage.sprite = gunScriptable.DisplayImage;
        m_WeaponSlotIndex = weaponSlotIndex;

        m_Btn.onClick.RemoveAllListeners();
        m_Btn.onClick.AddListener(OnClickBtn);
    }

    public GunScriptable GetGunScriptable(){
        return m_GunScriptable;
    }

    private void OnClickBtn(){
        // Change Weapon
        MapManager.GetInstance().GetMapUIController().OnClickWeaponListSlot(m_GunScriptable, m_WeaponSlotIndex, this);
    }

}
