using System;
using System.Collections;
using System.Collections.Generic;
using ExtendedButtons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MapChooseWeaponSlot : MonoBehaviour
{
    [SerializeField] private Image m_Border;
    [SerializeField] private Button2D m_Btn;
    [SerializeField] private Image m_WeaponShadowImage;
    [SerializeField] private Image m_WeaponDisplayImage;
    protected int m_WeaponSlotIndex = -1;

    public void Init(int slotIndex, Sprite shadowImage = null, Sprite weaponImage = null){
        m_WeaponSlotIndex = slotIndex;
        if(weaponImage != null)
            m_WeaponDisplayImage.sprite = weaponImage;
        
        if(shadowImage != null)
            m_WeaponShadowImage.sprite = shadowImage;

        m_Btn.onClick.RemoveAllListeners();
        m_Btn.onDown.RemoveAllListeners();
        m_Btn.onUp.RemoveAllListeners();
        MainGameManager.GetInstance().AddOnClickBaseAction(m_Btn,m_Btn.GetComponent<RectTransform>());
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
