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
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(m_Content);
#endif
        foreach (var item in upgradeScriptable.UpgradeDetails)
        {
            // spawn row
            var row = Instantiate(m_UpgradeStatRowPrefab,m_Content);
            row.GetComponent<WeaponUpgradeRowController>().Init(item,gunScriptable);
        }
        
        m_GunDisplayImage.sprite = gunScriptable.DisplayImage;
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(m_Content);
#endif
    }

}
