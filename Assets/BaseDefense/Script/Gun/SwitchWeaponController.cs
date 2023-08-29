using System.Collections;
using System.Collections.Generic;
using BaseDefenseNameSpace;
using UnityEngine;

public class SwitchWeaponController : MonoBehaviour
{

    [Header("Switch Weapon")]
    [SerializeField] private List<WeaponToBeSwitch> m_AllWeaponSlot = new List<WeaponToBeSwitch>();
    private int m_CurrentWeaponSlotIndex = 0;

    private void Start() {
        var allSelectedWeapon = MainGameManager.GetInstance().GetAllSelectedWeapon();

        for (int i = 0; i < allSelectedWeapon.Count; i++)
        {
            int index = i;
            if (allSelectedWeapon[i] != null)
            {
                m_AllWeaponSlot[index].Init(
                    allSelectedWeapon[i],
                    index
                ); 
                //m_GunsClipAmmo.Add(index, 0);
            }else{
                m_AllWeaponSlot[index].Init(
                    null,
                    index
                ); 
            }
            index++;

            // TODO : check slot owned in main game manager 
            if (index >= m_AllWeaponSlot.Count)
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
                if(m_AllWeaponSlot[i].IsGunDaraEmpty() != null){
                    m_AllWeaponSlot[i].OnClickWeapon();
                    m_CurrentWeaponSlotIndex = i;
                    break;
                }
            }
        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1) )
        {
            BaseDefenseManager.GetInstance().ChangeGameStage(BaseDefenseStage.SwitchWeapon);
            Debug.Log("SwitchWeapon");
        }
        if (Input.GetMouseButtonDown(0) && BaseDefenseManager.GetInstance().GameStage == BaseDefenseStage.SwitchWeapon )
        {
            //Debug.Log("");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // hit SwitchWeapon
            if (Physics.Raycast(ray, out hit, 100, 1<<11))
            {
                if(hit.transform.TryGetComponent<WeaponToBeSwitch>(out var weaponData)){
                    Debug.Log(hit.transform.position);
                }
            }
        }
    }
}
