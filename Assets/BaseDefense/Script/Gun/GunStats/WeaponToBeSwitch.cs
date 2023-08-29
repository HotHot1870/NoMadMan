using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using BaseDefenseNameSpace;

[System.Serializable]
public class WeaponToBeSwitch : MonoBehaviour
{
    [SerializeField] private Transform m_Self;
    private Transform m_GunModel=null;
    private GunScriptable m_Gun = null;
    private int m_SlotIndex = 0;

    public void Init(GunScriptable Gun, int SlotIndex){
        if(m_GunModel!=null){
            Destroy(m_GunModel.gameObject);
        }

        if(Gun==null)
            return;

        m_GunModel = Instantiate(Gun.FPSPrefab, m_Self).transform;
        m_GunModel.localPosition = Gun.ReloadPos;
        m_GunModel.localEulerAngles = Gun.ReloadRot;
        m_GunModel.localScale = Gun.ReloadScale;

        m_Gun = Gun;
        m_SlotIndex = SlotIndex;
    }

    public void OnClickWeapon(){
        if(BaseDefenseManager.GetInstance().GameStage == BaseDefenseStage.SwitchWeapon &&
            m_Gun != null){
            BaseDefenseManager.GetInstance().SwitchSelectedWeapon(m_Gun,m_SlotIndex );
        }
    }

    public bool IsGunDataEmpty(){
        return m_GunModel == null;
    }
}
