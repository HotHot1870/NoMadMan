using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using BaseDefenceNameSpace;

[System.Serializable]
public class WeaponToBeSwitch : MapChooseWeaponSlot
{

    public override void OnClickWeaponSlot(){
        if(BaseDefenceManager.GetInstance().GameStage == BaseDefenceStage.SwitchWeapon &&
            m_WeaponSlotIndex != -1){
            BaseDefenceManager.GetInstance().SwitchSelectedWeapon(m_WeaponSlotIndex );
            BaseDefenceManager.GetInstance().DoneSwitchWeapon();
        }
    }

    public bool IsGunDataEmpty(){
        return m_WeaponSlotIndex == -1;
    }
}
