using System.Collections;
using System.Collections.Generic;
using BaseDefenceNameSpace;
using UnityEngine;

public class SwitchWeaponController : MonoBehaviour
{

    [SerializeField] private List<WeaponToBeSwitch> m_AllWeaponSlot = new List<WeaponToBeSwitch>();
    private int m_CurrentWeaponSlotIndex = 0;

    private void Start() {

        var allSelectedWeapon = MainGameManager.GetInstance().GetAllSelectedWeapon();

        // set selected weapon into Slot
        for (int i = 0; i < m_AllWeaponSlot.Count; i++)
        {
            int index = i;
            if (allSelectedWeapon[index] != null)
            {
                m_AllWeaponSlot[index].Init(
                    index,
                    allSelectedWeapon[index].DisplayImage
                ); 
                BaseDefenceManager.GetInstance().GetGunShootController().SetUpGun(index,allSelectedWeapon[index] );
            }else{
                // no selected weapon
                m_AllWeaponSlot[index].Init(
                    index,
                    allSelectedWeapon[index].DisplayImage
                ); 
            }
            index++;

            if (index >= allSelectedWeapon.Count)
                break;

        }
        if (allSelectedWeapon == null || allSelectedWeapon.Count <= 0)
        {
            Debug.Log("No Selected Weapon");
        }
        else
        {
            // select first usable gun
            for (int i = 0; i < m_AllWeaponSlot.Count; i++)
            {
                if(m_AllWeaponSlot[i].IsGunDataEmpty()){
                    m_AllWeaponSlot[i].OnClickWeaponSlot();
                    m_CurrentWeaponSlotIndex = i;
                    break;
                }
            }
        }
    }
}
