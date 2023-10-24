using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using BaseDefenceNameSpace;

[System.Serializable]
public class WeaponToBeSwitch : MapChooseWeaponSlot
{
/*
    public void Init(GunScriptable Gun, int SlotIndex){
        if(m_GunModel!=null){
            Destroy(m_GunModel.gameObject);
        }

        if(Gun==null)
            return;

        m_GunModel = Instantiate(Gun.FPSPrefab, m_Self).transform;
        m_GunModel.localPosition = Gun.GunSwitch.ReloadPos;
        m_GunModel.localEulerAngles = Gun.GunSwitch.ReloadRot;
        m_GunModel.localScale = Gun.GunSwitch.ReloadScale;

        m_Gun = Gun;
        m_SlotIndex = SlotIndex;
    }*/

    public override void OnClickWeaponSlot(){
        if(BaseDefenceManager.GetInstance().GameStage == BaseDefenceStage.SwitchWeapon &&
            m_Index != -1){
            BaseDefenceManager.GetInstance().SwitchSelectedWeapon(m_Index );
            BaseDefenceManager.GetInstance().DoneSwitchWeapon();
        }
    }

    public bool IsGunDataEmpty(){
        return m_Index == -1;
    }
}
