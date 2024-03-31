using System.Collections;
using System.Collections.Generic;
using ExtendedButtons;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MapUpgradeWeaponPanel : MonoBehaviour
{
    [SerializeField] private Button2D m_BackBtn; 
    [SerializeField] private Image m_GunDisplayImage;
    [SerializeField] private Image m_GunShadowImage;
    [SerializeField] private GameObject m_UpgradeStatRowPrefab;
    [SerializeField] private RectTransform m_Content;
    [SerializeField] private List<WeaponUpgradeRowController> m_AllRow =
        new List<WeaponUpgradeRowController>();
        
    [SerializeField] private ScrollRect m_ScrollRect;



    private GunScriptable m_GunScriptable;

    private void Start(){

        
        MainGameManager.GetInstance().AddOnClickBaseAction(m_BackBtn, m_BackBtn.GetComponent<RectTransform>());
        m_BackBtn.onClick.AddListener(()=>
            this.gameObject.SetActive(false)
        );
        this.gameObject.SetActive(false);
    }

    public void Init(GunScriptable gunScriptable, bool scrollToTop = true){
        m_GunScriptable = gunScriptable;
        var upgradeScriptable = m_GunScriptable.UpgradeScriptable;
        if(scrollToTop)
            m_ScrollRect.normalizedPosition = new Vector2(0, 1);
        
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
        m_GunShadowImage.sprite = gunScriptable.WhiteImage;
        m_GunDisplayImage.sprite = gunScriptable.DisplayImage;
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(m_Content);
#endif
    }
    private void ResetAllRow(){
        Init(m_GunScriptable,false);
    }

}
