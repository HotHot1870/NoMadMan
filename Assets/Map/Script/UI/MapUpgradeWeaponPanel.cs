using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MapUpgradeWeaponPanel : MonoBehaviour
{
    [SerializeField] private Button m_BackBtn; 
    [SerializeField] private Image m_GunDisplayImage;
    [SerializeField] private GameObject m_UpgradeStatRowPrefab;
    [SerializeField] private RectTransform m_Content;
    [SerializeField] private List<WeaponUpgradeRowController> m_AllRow =
        new List<WeaponUpgradeRowController>();



    private GunScriptable m_GunScriptable;

    private void Start(){
        m_BackBtn.onClick.AddListener(()=>
            this.gameObject.SetActive(false)
        );
        this.gameObject.SetActive(false);
    }

    public void Init(GunScriptable gunScriptable){
        m_GunScriptable = gunScriptable;
        var upgradeScriptable = m_GunScriptable.UpgradeScriptable;
        
        // clear all Row
        for (int i = 0; i < m_Content.childCount; i++)
        {
            Destroy(m_Content.GetChild(i).gameObject);
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(m_Content);
#endif
        foreach (var item in upgradeScriptable.UpgradeDetails)
        {
            // spawn row
            var row = Instantiate(m_UpgradeStatRowPrefab,m_Content);
            var rowController = row.GetComponent<WeaponUpgradeRowController>();
            WeaponUpgradeDetail weaponUpgradeDetail =item;
            var gunScriptableCopy = gunScriptable;
            m_AllRow.Add(rowController);
            row.GetComponent<WeaponUpgradeRowController>().Init(item,gunScriptable,ResetAllRow);
        }
        
        m_GunDisplayImage.sprite = gunScriptable.DisplayImage;
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(m_Content);
#endif
    }
    private void ResetAllRow(){
        Init(m_GunScriptable);
    }

}
