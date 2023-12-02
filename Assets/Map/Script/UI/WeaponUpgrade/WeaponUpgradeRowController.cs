using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ExtendedButtons;

public class WeaponUpgradeRowController : MonoBehaviour
{
    [SerializeField] private Button2D m_UpgradeBtn;
    [SerializeField] private TMP_Text m_StatName;  
    [SerializeField] private TMP_Text m_Cost;  
    [SerializeField] private GameObject m_UpgradeStatSmallBoxPrefab;
    private WeaponUpgradeScriptable m_UpgradeScriptable;
    private WeaponUpgradeDetail m_UpgradeDetail;

    public void Init(WeaponUpgradeScriptable upgradeScriptable, WeaponUpgradeDetail upgradeDetail){
        m_UpgradeDetail = upgradeDetail;
        m_UpgradeScriptable = upgradeScriptable;
        foreach (var item in m_UpgradeDetail.CostAndValue)
        {
            // TODO : Upgrade Block value 
        }
    }
}
