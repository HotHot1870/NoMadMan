using System.Collections;
using System.Collections.Generic;
using BaseDefenceNameSpace;
using UnityEditor.Rendering;
using UnityEngine;

public class SwitchWeaponController : MonoBehaviour
{

    [SerializeField] private List<WeaponToBeSwitch> m_AllWeaponSlot = new List<WeaponToBeSwitch>();
    private int m_CurrentWeaponSlotIndex = 0;

    private void Start() {

        List<GunScriptable> allSelectedWeapon = MainGameManager.GetInstance().GetAllSelectedWeapon();
        List<GunScriptable> allWeapon = MainGameManager.GetInstance().GetAllWeapon();

        // set selected weapon into Slot
        for (int i = 0; i < 4; i++)
        {
            int index = i;
            var targetGunId = (int)MainGameManager.GetInstance().GetData<int>("SelectedWeapon"+index.ToString(),"-1");
            GunScriptable targetGunScriptable = allWeapon.Find(x=>x.Id == targetGunId);
            if (targetGunScriptable != null)
            {
                m_AllWeaponSlot[index].Init(
                    index,
                    targetGunScriptable.DisplayImage
                ); 
                BaseDefenceManager.GetInstance().GetGunShootController().SetUpGun(index,targetGunScriptable );
            }else{
                // no selected weapon
                m_AllWeaponSlot[index].Init(
                    index,
                    null
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
