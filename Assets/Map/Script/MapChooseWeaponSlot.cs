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
    private int m_Index = -1;

    private void Start() {
        m_Btn.onClick.AddListener(OnClickWeaponSlot);
    }
    public void Init(int index, Sprite weaponImage){
        m_Index = index;
        m_WeaponDisplayImage.sprite = weaponImage;
    }

    public void SetBorderColor(Color color){
        m_Border.color = color;
    }

    

    private void OnClickWeaponSlot(){
        // TODO : show weapon list
        MapManager.GetInstance().GetMapUIController().ShowWeaponListPanel(m_Index);
    }
}
