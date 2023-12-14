using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MapChooseWeaponSlot : MonoBehaviour
{
    [SerializeField] private Image m_Border;
    [SerializeField] private Button m_Btn;
    [SerializeField] private Image m_WeaponDisplayImage;
    protected int m_WeaponSlotIndex = -1;

    public void Init(int slotIndex, Sprite weaponImage = null){
        m_WeaponSlotIndex = slotIndex;
        if(weaponImage != null){
            m_WeaponDisplayImage.sprite = weaponImage;
        }
        m_Btn.onClick.RemoveAllListeners();
        m_Btn.onClick.AddListener(OnClickWeaponSlot);

    }

    public void SetBorderColor(Color color){
        m_Border.color = color;
    }

    

    public virtual void OnClickWeaponSlot(){
        // show weapon list
        MapManager.GetInstance().GetMapUIController().ShowChangeWeaponInSlotPanel(m_WeaponSlotIndex);
    }
}
