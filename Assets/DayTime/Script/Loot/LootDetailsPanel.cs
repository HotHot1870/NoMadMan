using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// show after click on loot location

public class LootDetailsPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Image m_DetailImage; 

    [SerializeField] private TMP_Text m_HeatGain; 
    [SerializeField] private TMP_Text m_LocationName; 
    [SerializeField] private TMP_Text m_Raw; 
    [SerializeField] private TMP_Text m_Scrap;
    [SerializeField] private TMP_Text m_Chem;
    [SerializeField] private TMP_Text m_Electronic;
    [SerializeField] private TMP_Text m_Saftness;
    [SerializeField] private TMP_Text m_BotCount;

    [SerializeField] private Button m_ConfirmBtn;
    [SerializeField] private Button m_CancelBtn;
    [SerializeField] private Button m_IncreaseBotBtn;
    [SerializeField] private Button m_DecreaseBotBtn;


    private int m_OriginalLootBotCount = 0; // used by cancel btn
    private LootLocationController m_TargetLootLocationController;

    private void Start() {
        m_IncreaseBotBtn.onClick.AddListener(()=>{
            // check if enough bot
            if( DayTimeManager.GetInstance().GetTotalBotUsed() >=
                  MainGameManager.GetInstance(). GetOwnedBotCount() ){
                return;
            }
            m_TargetLootLocationController.SetLootBotCount(m_TargetLootLocationController.GetLootBotCount()+1);
            UpdateSentBotCount();
        });

        m_DecreaseBotBtn.onClick.AddListener(()=>{
            // low cap 0
            if(m_TargetLootLocationController.GetLootBotCount()<=0){
                return;
            }
            m_TargetLootLocationController.SetLootBotCount(m_TargetLootLocationController.GetLootBotCount()-1);
            UpdateSentBotCount();
        });

        m_ConfirmBtn.onClick.AddListener(OnClickConfirm);

        m_CancelBtn.onClick.AddListener(()=>{
            m_TargetLootLocationController.SetLootBotCount(m_OriginalLootBotCount);
            DayTimeManager.GetInstance().SetTopBar(true);
            m_Self.SetActive(false);
        });

        m_Self.SetActive(false);
    }

    public void SetLootDeail(LootLocationController lootLocationController, int originalLootBotCount){
        DayTimeManager.GetInstance().SetTopBar(false);
        m_TargetLootLocationController = lootLocationController;

        m_OriginalLootBotCount = originalLootBotCount; 
        m_Self.SetActive(true);
        var scriptable = m_TargetLootLocationController.GetScriptable();
        string text = $"{scriptable.DisplayName}";
        m_LocationName.text = text;

        text = $"Heat gain on loot : {scriptable.HeatGainOnLoot }";
        m_HeatGain.text = text;

        text = $"{scriptable.MinRawMaterial}~{scriptable.MaxRawMaterial}";
        m_Raw.text = text;
        text = $"{scriptable.MinScrapMaterial}~{scriptable.MaxScrapMaterial}";
        m_Scrap.text = text;
        text = $"{scriptable.MinChemMaterial}~{scriptable.MaxChemMaterial}";
        m_Chem.text = text;
        text = $"{scriptable.MinElectronicMaterial}~{scriptable.MaxElectronicMaterial}";
        m_Electronic.text = text;

        m_DetailImage.sprite = scriptable.DetailImage;

        UpdateSentBotCount();
    }

    private void OnClickConfirm(){
        DayTimeManager.GetInstance().SetTopBar(true);
        m_Self.SetActive(false);
    }

    private void UpdateSentBotCount(){
        int sentLootBotCount = m_TargetLootLocationController.GetLootBotCount();
        string text = $"x {sentLootBotCount}";
        m_BotCount.text = text;

        if(sentLootBotCount <=0){
            text = $"Sent at least one bot to start looting ";
            m_Saftness.text = text;
        }else{
            var scriptable = m_TargetLootLocationController.GetScriptable();
            text = $"Safeness : {scriptable.BaseSafeness}%";



            // safe on mutil bot sent
            if(sentLootBotCount>1){
                string symbol = scriptable.SaftnessGainPreExtraBot>=0?"+":"";
                text += $" {symbol}{scriptable.SaftnessGainPreExtraBot *(sentLootBotCount-1) }%";
            }

            float toHomeDistance =Mathf.Sqrt(scriptable.Position.x * scriptable.Position.x + scriptable.Position.y * scriptable.Position.y);
            float heatRadius = DayTimeManager.GetInstance().GetHeatRadius();
            float heatDanger = 0;
            bool IsOutsideSafeZoon = toHomeDistance >= heatRadius;
            if( IsOutsideSafeZoon ){
                // outside safe zoon
                heatDanger = (toHomeDistance - heatRadius)*0.25f;
                text += $" -{(int)heatDanger}%";
            }

            if(sentLootBotCount>1 || IsOutsideSafeZoon){
                // show total safeness

                text += $" ({ (int) (scriptable.BaseSafeness+scriptable.SaftnessGainPreExtraBot *(sentLootBotCount-1) - heatDanger ) }%)";
            }

            m_Saftness.text = text;
        }

        DayTimeManager.GetInstance().SetUsedBotCountText( );
    }
}
