using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LootResultPanelNameSpace;
using MainGameNameSpace;

namespace LootResultPanelNameSpace
{
    [System.Serializable]
    public class LootResultResourcesText
    {
        public TMP_Text Raw;
        public TMP_Text Scrap;
        public TMP_Text Chem;
        public TMP_Text Electronic;
        public TMP_Text Bot;
        public TMP_Text Heat;
    }

}

public class LootResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Image m_DetailImage;
    [SerializeField] private TMP_Text m_LocationName;
    [SerializeField] private TMP_Text m_Safeness;
    [SerializeField] private TMP_Text m_SendBot;
    [SerializeField] private TMP_Text m_ResultText;
    [SerializeField] private LootResultResourcesText m_Gain;
    [SerializeField] private Button m_NextBtn;

    private void Start() {
        m_NextBtn.onClick.AddListener(()=>{
            DayTimeManager.GetInstance().OnClickResultPanelNextBtn();
        });

        m_Self.SetActive(false);
    }

    public void TurnOff(){
        m_Self.SetActive(false);
    }

    public ResourcesRecord Init(LootLocationScriptable scriptable , int botCount){
        m_Self.SetActive(true);
        m_DetailImage.sprite = scriptable.DetailImage;
       m_LocationName.text = $"Location : {scriptable.DisplayName }";
       float extraSafeness = scriptable.SaftnessGainPreExtraBot * Mathf.Max(0, (botCount-1) );
       float totalSafeness = scriptable.BaseSafeness + extraSafeness;            
       
       float toHomeDistance =Mathf.Sqrt(scriptable.Position.x * scriptable.Position.x + scriptable.Position.y * scriptable.Position.y);
        float heatRadius = DayTimeManager.GetInstance().GetHeatRadius();
        if(toHomeDistance >= heatRadius ){
            // outside safe zoon
            totalSafeness -=  (toHomeDistance - heatRadius)*0.25f ;
        }
       m_Safeness.text = $"Safeness : {(int)totalSafeness}%";
       m_SendBot.text = $"Send Bot : {botCount}";

        float safenessNeeded = Random.Range(0f,100f);
        bool isSuccess = safenessNeeded<=totalSafeness;
        string targetResultText = isSuccess?"Return with loots":"No one return";

       m_ResultText.text = $"Result : {targetResultText}";
       m_ResultText.color = isSuccess?Color.green:Color.red;

       ResourcesRecord ans = new ResourcesRecord();
       
       if(isSuccess){
            // gain resource on success 
            float seedRandom = Random.Range(0f,1f);
            ans.Raw = Mathf.Lerp(scriptable.MinRawMaterial,scriptable.MaxRawMaterial,seedRandom );
            m_Gain.Raw.text = $"+{(int)ans.Raw}";

            seedRandom = Random.Range(0f,1f);
            ans.Scrap = Mathf.Lerp(scriptable.MinScrapMaterial,scriptable.MaxScrapMaterial,seedRandom );
            m_Gain.Scrap.text = $"+{(int)ans.Scrap}";

            seedRandom = Random.Range(0f,1f);
            ans.Chem = Mathf.Lerp(scriptable.MinChemMaterial,scriptable.MaxChemMaterial,seedRandom );
            m_Gain.Chem.text = $"+{(int)ans.Chem}";

            seedRandom = Random.Range(0f,1f);
            ans.Electronic = Mathf.Lerp(scriptable.MinElectronicMaterial,scriptable.MaxElectronicMaterial,seedRandom );
            m_Gain.Electronic.text = $"+{(int)ans.Electronic}";

            ans.Bot = 0;
            m_Gain.Bot.text = "+0";

            ans.Heat = scriptable.HeatGainOnLoot;
            string heatText = scriptable.HeatGainOnLoot>=0 ? "+":"";
            m_Gain.Heat.text = heatText + $"{scriptable.HeatGainOnLoot}";

       }else{
            // lose bot on fail
            ans.Raw = 0;
            ans.Scrap = 0;
            ans.Chem = 0;
            ans.Electronic = 0;
            ans.Bot = botCount*-1;
            ans.Heat = 0;

            m_Gain.Raw.text =  "+0";
            m_Gain.Scrap.text =  "+0";
            m_Gain.Chem.text =  "+0";
            m_Gain.Electronic.text =  "+0";
            m_Gain.Bot.text = $"-{botCount}";
            m_Gain.Heat.text = $"+0";
       }

       return ans;
    }
}
