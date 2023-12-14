
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class MapWeaponListGrid : MonoBehaviour
{
    [SerializeField] private Image m_Border;
    [SerializeField] private Button m_Btn;
    [SerializeField] private Image m_WeaponDisplayImage;
    private GunScriptable m_GunScriptable = null;
    private bool m_IsWeaponLocked;
    private int m_WeaponSlotIndex;

    public void Init(GunScriptable gunScriptable, int weaponSlotIndex, bool isUnlocked){
            
        m_Btn.onClick.RemoveAllListeners();
        m_Btn.onClick.AddListener(OnClickBtnWithUnlockWeapon);
        m_IsWeaponLocked = !isUnlocked;
        m_WeaponDisplayImage.color =isUnlocked?Color.white: Color.black;
        
        

        m_WeaponSlotIndex = weaponSlotIndex;
        if(gunScriptable == null){
            // no selected weapon

            return;
        }
        m_GunScriptable = gunScriptable;
        m_WeaponDisplayImage.sprite = gunScriptable.DisplayImage;
        
    }


    private void OnClickBtnWithUnlockWeapon(){
        // Change Weapon detail
        MapManager.GetInstance().GetMapUIController().OnClickWeaponListSlot(m_IsWeaponLocked,m_GunScriptable, m_WeaponSlotIndex, this);
    }

}
