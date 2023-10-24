using System.Collections;
using System.Collections.Generic;
using BaseDefenceNameSpace;
using UnityEngine;

public class SwitchWeaponController : MonoBehaviour
{

    [SerializeField] private List<WeaponToBeSwitch> m_AllWeaponSlot = new List<WeaponToBeSwitch>();
    private int m_CurrentWeaponSlotIndex = 0;

    private void Start() {
        BaseDefenceManager.GetInstance().m_SwitchWeaponUpdateAction += SwitchWeaponUpdate;

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
                m_AllWeaponSlot[index].Init(
                    index,
                    allSelectedWeapon[index].DisplayImage
                ); 
            }
            index++;

            // TODO : check slot owned in main game manager 
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

    private void SwitchWeaponUpdate() {/*
        if (Input.GetMouseButtonDown(0) && BaseDefenceManager.GetInstance().GameStage == BaseDefenceStage.SwitchWeapon )
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // hit SwitchWeapon
            if (Physics.Raycast(ray, out hit, 100, 1<<11))
            {
                if(hit.transform.TryGetComponent<WeaponToBeSwitch>(out var weaponData)){
                    // switch weapon 
                    weaponData.OnClickWeaponSlot();

                    // look up 
                    //BaseDefenceManager.GetInstance().LookUp();
                }
            }
        }*/
    }
}
