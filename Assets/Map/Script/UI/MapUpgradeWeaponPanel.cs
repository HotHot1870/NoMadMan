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
    [SerializeField] private CanvasGroup m_BottomFade;
    [SerializeField] private RectTransform m_Content;

    private float m_ContentHeight = 0;
    // including spawn
    private float m_RowTotalheight = 0;


    private GunScriptable m_GunScriptable;

    private void Start(){
        this.gameObject.SetActive(false);
    }

    public void Init(GunScriptable gunScriptable){
        m_GunScriptable = gunScriptable;
        var upgradeScriptable = m_GunScriptable.UpgradeScriptable;
        EditorUtility.SetDirty(m_Content);
        foreach (var item in upgradeScriptable.UpgradeDetails)
        {
            // spawn row
            var row = Instantiate(m_UpgradeStatRowPrefab,m_Content);
            row.GetComponent<WeaponUpgradeRowController>().Init(upgradeScriptable,item);
        }
        
        m_RowTotalheight = m_UpgradeStatRowPrefab.GetComponent<RectTransform>().sizeDelta.y*(m_GunScriptable.UpgradeScriptable.UpgradeDetails.Count*2-1 );
        EditorUtility.SetDirty(m_Content);
        m_BottomFade.alpha = m_Content.sizeDelta.y >= m_RowTotalheight ? 0f:1f;
    }

    public void OnScroll(){
        m_ContentHeight = m_Content.sizeDelta.y;
        // total height - content scroll amount - content size y
        m_BottomFade.alpha = (m_RowTotalheight - m_Content.position.y - m_ContentHeight);
        Debug.Log(m_RowTotalheight - m_Content.position.y - m_ContentHeight);
        Debug.Log(m_RowTotalheight+"    "+m_Content.position.y+"     "+ m_ContentHeight);
    }

}
