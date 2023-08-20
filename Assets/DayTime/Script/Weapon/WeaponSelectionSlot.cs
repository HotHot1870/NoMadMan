using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionSlot : MonoBehaviour
{
    [SerializeField] private Image m_Border;
    public Image m_WeaponImage;
    public Button m_Btn;

    public void BorderColorToggle(bool isActive){
        m_Border.color = isActive?Color.green:Color.white;   
    }

    public void SetBorderRed(){
        // cannot have the same gun on different slot
        m_Border.color = Color.red;
    }
}
