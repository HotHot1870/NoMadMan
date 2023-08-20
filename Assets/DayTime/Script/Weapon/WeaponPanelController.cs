using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponPanelController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;

    [Header("Grid")]
    [SerializeField] private GameObject m_WeaponGridPanel;
    [SerializeField] private TMP_Text m_WeaponPageText;
    [SerializeField] private Button m_LastPageBtn;
    [SerializeField] private Button m_NextPageBtn;
    [SerializeField] private List<WeaponDetailGrid> m_AllWeaponGrid = new List<WeaponDetailGrid>();
    private int m_PageIndex = 0;




    [Header("Detail")]
    [SerializeField] private GameObject m_WeaponDetailPanel;
    [SerializeField] private Button m_QuitWeaponDetailBtn;
    [SerializeField] private Button m_SelectWeaponBtn;
    [SerializeField] private Image m_WeaponDetailImage;
    [SerializeField] private TMP_Text m_WeaponDisplayName;
    [SerializeField] private TMP_Text m_DamagePerPellet;
    [SerializeField] private TMP_Text m_PelletPerShot;
    [SerializeField] private TMP_Text m_ClipSize;
    [SerializeField] private TMP_Text m_SemiAuto;
    [SerializeField] private TMP_Text m_FireRate;
    [SerializeField] private TMP_Text m_Accuracy;
    [SerializeField] private TMP_Text m_RecoilControl;
    [SerializeField] private TMP_Text m_Stability;
    [SerializeField] private TMP_Text m_Handling;
    private GunScriptable m_CurrentDetailWeapon;


    [Header("Select Weapon")]
    [SerializeField] private GameObject m_WeaponSelectionPanel;
    [SerializeField] private Button m_QuitSelectWeaponBtn;
    [SerializeField] private Button m_ConfirmSelectWeaponBtn;
    [SerializeField] private Image m_WeaponToBeSelectedImage;
    [SerializeField] private List<WeaponSelectionSlot> m_AllWeaponSelectSlot = new List<WeaponSelectionSlot>();

    private int m_CurrentSelectedSlotIndex=-1;



    private void Start(){
        DayTimeManager.GetInstance().m_ChangeFromWeapon += ()=>{
            m_Self.SetActive(false);
            };

        DayTimeManager.GetInstance().m_ChangeToWeapon += ()=>{
            m_WeaponDetailPanel.SetActive(false);
            m_WeaponSelectionPanel.SetActive(false);
            m_WeaponGridPanel.SetActive(true);
            ClearInfo();
            m_Self.SetActive(true);
            };

        m_SelectWeaponBtn.onClick.AddListener(()=>{
            m_WeaponDetailPanel.SetActive(false);
            m_WeaponGridPanel.SetActive(false);
            m_WeaponSelectionPanel.SetActive(true);
            SetSelectionPanel();
        });

        m_QuitSelectWeaponBtn.onClick.AddListener(()=>{
            m_WeaponDetailPanel.SetActive(true);
            m_WeaponSelectionPanel.SetActive(false);
            m_WeaponGridPanel.SetActive(false);

        });

        m_ConfirmSelectWeaponBtn.onClick.AddListener(()=>{
            // cannot use same weapon in 2 different slot
            var CurSelectedWeaponList = MainGameManager.GetInstance().GetAllSelectedWeapon();
            if(CurSelectedWeaponList.Contains(m_CurrentDetailWeapon) ){
                // remove old one if 
                int targetIndex = CurSelectedWeaponList.IndexOf(m_CurrentDetailWeapon);
                MainGameManager.GetInstance().ChangeSelectedWeapon(targetIndex,null);
            }

            MainGameManager.GetInstance().ChangeSelectedWeapon(m_CurrentSelectedSlotIndex, m_CurrentDetailWeapon);
            m_QuitSelectWeaponBtn.onClick?.Invoke();
        });

        m_QuitWeaponDetailBtn.onClick.AddListener(()=>{
            m_WeaponDetailPanel.SetActive(false);
            m_WeaponGridPanel.SetActive(true);
        });

        ClearInfo();
        m_Self.SetActive(false);

        // Set Weapon select slot click function
        for (int i = 0; i < m_AllWeaponSelectSlot.Count; i++)
        {
            var slotIndex = i;
            m_AllWeaponSelectSlot[slotIndex].m_Btn.onClick.AddListener(()=>{
                if(m_CurrentSelectedSlotIndex>=0 && m_CurrentSelectedSlotIndex<m_AllWeaponSelectSlot.Count )
                    m_AllWeaponSelectSlot[m_CurrentSelectedSlotIndex].BorderColorToggle(false);
        
                // cannot use same weapon in 2 different slot
                var CurSelectedWeaponList = MainGameManager.GetInstance().GetAllSelectedWeapon();
                if(CurSelectedWeaponList.Contains(m_CurrentDetailWeapon) ){
                    // remove old one if 
                    int targetIndex = CurSelectedWeaponList.IndexOf(m_CurrentDetailWeapon);
                    m_AllWeaponSelectSlot[targetIndex].SetBorderRed();
                }


                m_CurrentSelectedSlotIndex = slotIndex;
                m_AllWeaponSelectSlot[slotIndex].BorderColorToggle(true);
        
            });
        }

        // cannot use same weapon in 2 different slot
        var CurSelectedWeaponList = MainGameManager.GetInstance().GetAllSelectedWeapon();
        if(CurSelectedWeaponList.Contains(m_CurrentDetailWeapon) ){
            // remove old one if 
            int targetIndex = CurSelectedWeaponList.IndexOf(m_CurrentDetailWeapon);
            m_AllWeaponSelectSlot[targetIndex].SetBorderRed();
        }


        var allWeaponOwnership = MainGameManager.GetInstance().GetAllWeaponOwnership();
        int index = 0;
        for (int i = m_PageIndex*12; index < m_AllWeaponGrid.Count; i++){
            if( i >= allWeaponOwnership.Count){
                // clear image and weapon name , but do not destory
                m_AllWeaponGrid[index].Init(null);
                index ++;
                continue;
            }
            // TODO : not own handling
            m_AllWeaponGrid[index].Init(allWeaponOwnership[i].Gun);
            index ++;
        }
    }


    private void SetSelectionPanel(){
        // set selected weapon
        m_WeaponToBeSelectedImage.sprite = m_CurrentDetailWeapon.DisplaySprite;
        var selectedWeapon = MainGameManager.GetInstance().GetAllSelectedWeapon();
        for (int i = 0; i < m_AllWeaponSelectSlot.Count; i++)
        {
            m_AllWeaponSelectSlot[i].BorderColorToggle(false);
            if(selectedWeapon.Count>i && selectedWeapon[i] != null){
                m_AllWeaponSelectSlot[i].m_WeaponImage.sprite = selectedWeapon[i].DisplaySprite;
            }else{
                m_AllWeaponSelectSlot[i].m_WeaponImage.sprite = null;
            }
        }
        
        // cannot use same weapon in 2 different slot
        var CurSelectedWeaponList = MainGameManager.GetInstance().GetAllSelectedWeapon();
        if(CurSelectedWeaponList.Contains(m_CurrentDetailWeapon) ){
            // remove old one if 
            int targetIndex = CurSelectedWeaponList.IndexOf(m_CurrentDetailWeapon);
            m_AllWeaponSelectSlot[targetIndex].SetBorderRed();
        }

    }

    public void SetInfo(GunScriptable gunScriptable){
        m_CurrentDetailWeapon = gunScriptable;
        m_WeaponGridPanel.SetActive(false);
        m_WeaponDetailPanel.SetActive(true);
        m_WeaponDetailImage.sprite = gunScriptable.DisplaySprite;
        m_WeaponDisplayName.text = gunScriptable.DisplayName;
        m_DamagePerPellet.text = $"Damage per pellet : {gunScriptable.Damage}";
        m_PelletPerShot.text = $"Pellet per shot: {gunScriptable.PelletPerShot}";
        m_SemiAuto.text = $"Semi-Auto : {gunScriptable.IsSemiAuto.ToString() }";
        m_ClipSize.text = $"Clip size : {gunScriptable.ClipSize}";
        m_FireRate.text = $"Fire Rate : {gunScriptable.FireRate}";
        m_Accuracy.text = $"Accuracy : {gunScriptable.Accuracy}";
        m_RecoilControl.text = $"Recoil Control : {gunScriptable.RecoilControl}";
        m_Stability.text = $"Stability : {gunScriptable.Stability}";
        m_Handling.text = $"Handling : {gunScriptable.Handling}";

    }

    private void ClearInfo(){
        m_WeaponDisplayName.text = "- - -";
        m_DamagePerPellet.text = $"Damage per pellet : - - -";
        m_PelletPerShot.text = $"Pellet per shot: - - -";
        m_ClipSize.text = $"Clip size : - - -";
        m_FireRate.text = $"Fire Rate : - - -";
        m_Accuracy.text = $"Accuracy : - - -";
        m_RecoilControl.text = $"Recoil Control : - - -";
        m_Stability.text = $"Stability : - - -";
        m_Handling.text = $"Handling : - - -";
    }
}
