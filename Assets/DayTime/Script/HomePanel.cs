using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MainGameNameSpace;
using LootResultPanelNameSpace;
using System;

public class HomePanel : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [Header("BaseContent")]
    [SerializeField] private GameObject m_BaseContentPanel;
    [SerializeField] private TMP_Text m_WallHpText;
    [SerializeField] private Image m_WallHpBar;
    [SerializeField] private Button m_RepairWallBtn;
    [SerializeField] private ResourcesRecord m_WallRepairNeeded = new ResourcesRecord();
    [SerializeField] private float m_WallRepairAmount = 50f;
    [Header("Popup")]
    [SerializeField] private GameObject m_PopupPanel;
    [SerializeField] private TMP_Text m_PopupTitle;
    [SerializeField] private LootResultResourcesText m_ResourceTexts;
    [SerializeField] private Image m_FillImage;
    [SerializeField] private TMP_Text m_FillImageText;
    [SerializeField] private Button m_IncreaseBtn;
    [SerializeField] private TMP_Text m_DoCountText;
    [SerializeField] private Button m_DecreaseBtn;
    [SerializeField] private Button m_PopupConfirmBtn;
    [SerializeField] private Button m_PopupCancelBtn;
    private ResourcesRecord m_Cost = new ResourcesRecord();
    private float m_CurValue;
    private float m_ValueChangesPerCount;
    private float m_MaxValue;
    private int m_DoCount = 0;



    private void Start() {
        TurnOffHomePanel();
        DayTimeManager.GetInstance().m_ChangeFromHome += TurnOffHomePanel;
        DayTimeManager.GetInstance().m_ChangeToHome += SetBasePanel;

        m_PopupCancelBtn.onClick.AddListener(()=>{
            m_BaseContentPanel.SetActive(true);
            m_PopupPanel.SetActive(false);
            SetBasePanel();
        });
        
        m_RepairWallBtn.onClick.AddListener(()=>{
            SetPopupPanel("Repair Wall",m_WallRepairNeeded,
                MainGameManager.GetInstance().GetWallCurHp(),
                m_WallRepairAmount,
                MainGameManager.GetInstance().GetWallMaxHp(),
                OnClickWallRepair
                );
        });

        m_IncreaseBtn.onClick.AddListener(()=>{
            // check is enough resources AND value is not fill
            if(MainGameManager.GetInstance().GetOwnedResources().IsEnough(m_Cost.Multiply(m_DoCount)) && 
                (m_CurValue + m_DoCount * m_ValueChangesPerCount) / m_MaxValue < 1 ){
                m_DoCount++;
                ResetPopupPanelByDoCountChanges();
            }
        });

        m_DecreaseBtn.onClick.AddListener(()=>{
            if(m_DoCount > 0){
                m_DoCount--;
                ResetPopupPanelByDoCountChanges();
            }
        });
    }


    private void TurnOffHomePanel(){
        m_Self.SetActive(false);
    }

    #region Base Content
        
    private void SetBasePanel(){
        float wallCurHp = MainGameManager.GetInstance().GetWallCurHp();
        float wallMaxHp = MainGameManager.GetInstance().GetWallMaxHp();
        m_WallHpText.text = $"{(int)wallCurHp} / {(int)wallMaxHp}";
        m_WallHpBar.fillAmount = wallCurHp / wallMaxHp;

        m_Self.SetActive(true);

        m_BaseContentPanel.SetActive(true);
        m_PopupPanel.SetActive(false);        

    }

    private void OnClickWallRepair(){
        if( MainGameManager.GetInstance().GetWallCurHp() >= MainGameManager.GetInstance().GetWallMaxHp() ){
            return;
        }
        // Check If enough resource
        if(MainGameManager.GetInstance().GetOwnedResources().IsEnough(m_WallRepairNeeded)){
            // consume resource
            MainGameManager.GetInstance().GetOwnedResources().Change(m_WallRepairNeeded.GetReverse().Multiply(m_DoCount));

            // Gain wall hp 
            MainGameManager.GetInstance().ChangeWallHp(m_WallRepairAmount*m_DoCount);

        }

    }
    #endregion

    #region Popup
    private void SetPopupPanel(string title, ResourcesRecord cost, float curValue,float valueChangesPerCount, float maxValue, Action onConfirm){
        m_PopupPanel.SetActive(true);
        m_PopupTitle.text = title;
        m_Cost = cost;
        m_CurValue = curValue;
        m_ValueChangesPerCount = valueChangesPerCount;
        m_MaxValue = maxValue;
        m_DoCount = 0;
        ResetPopupPanelByDoCountChanges();

        m_PopupConfirmBtn.onClick.RemoveAllListeners();
        m_PopupConfirmBtn.onClick.AddListener(()=>{
            onConfirm?.Invoke();
            m_BaseContentPanel.SetActive(true);
            m_PopupPanel.SetActive(false);
            SetBasePanel();
            });
        
        
    }


    private void ResetPopupPanelByDoCountChanges(){
        // do if doCount change
        m_DoCountText.text = $"{m_DoCount}";

        m_FillImage.fillAmount = (m_CurValue + m_ValueChangesPerCount * m_DoCount ) / m_MaxValue;
        string symbol = m_ValueChangesPerCount<0?"":"+";
        m_FillImageText.text = $"{(int)m_CurValue} ({symbol}{ (int) (m_ValueChangesPerCount * m_DoCount)}) / {(int)m_MaxValue}"; 
        
        var ownedResource = MainGameManager.GetInstance().GetOwnedResources();
        var cost = m_Cost.GetReverse();
        symbol = cost.Raw<0?"":"+";
        m_ResourceTexts.Raw.text = $"{ownedResource.Raw} ({symbol}{cost.Raw* m_DoCount})";
        symbol = cost.Scrap<0?"":"+";
        m_ResourceTexts.Scrap.text = $"{ownedResource.Scrap} ({symbol}{cost.Scrap* m_DoCount})";
        symbol = cost.Chem<0?"":"+";
        m_ResourceTexts.Chem.text = $"{ownedResource.Chem} ({symbol}{cost.Chem* m_DoCount})";
        symbol = cost.Electronic<0?"":"+";
        m_ResourceTexts.Electronic.text = $"{ownedResource.Electronic} ({symbol}{cost.Electronic* m_DoCount})";
        symbol = cost.Bot<0?"":"+";
        m_ResourceTexts.Bot.text = $"{ownedResource.Bot} ({symbol}{cost.Bot* m_DoCount})";
        symbol = cost.Heat<0?"":"+";
        m_ResourceTexts.Heat.text = $"{ownedResource.Heat} ({symbol}{cost.Heat* m_DoCount})";

    }
    
    #endregion
}
