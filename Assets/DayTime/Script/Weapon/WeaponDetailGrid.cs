using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponDetailGrid : MonoBehaviour
{
    [SerializeField] private Image m_DisplayImage;
    [SerializeField] private TMP_Text m_WeaponName;
    [SerializeField] private Button m_Btn;

    public void Init(GunScriptable gun){
        // hide if no gun
        if(gun == null){
            m_DisplayImage.color = Color.clear;
            m_WeaponName.text = "";
            return;
        }

        m_DisplayImage.color = Color.white;
        m_WeaponName.text = gun.DisplayName;
        m_DisplayImage.sprite = gun.DisplaySprite;
        // TODO : size control
        //Debug.LogError(gun.DisplaySprite.border);
        //m_DisplayImage.GetComponent<RectTransform>().sizeDelta = gun.DisplaySprite.border ; 
        m_Btn.onClick.RemoveAllListeners();
        m_Btn.onClick.AddListener(()=>{
            DayTimeManager.GetInstance().SetWeaponDetailPanel(gun);
        });
    }
}
