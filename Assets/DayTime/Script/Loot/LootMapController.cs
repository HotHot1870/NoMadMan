using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LootLocationControllerNameSpace;

public class LootMapController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;

    [SerializeField] private List<LootLocationScriptable> m_AllLocation = new List<LootLocationScriptable>();
    [SerializeField] private Transform m_LocationParent;
    [SerializeField] private GameObject m_LocaionPrefab;
    [SerializeField] private float m_HeatAreaSize = 600;
    [SerializeField] private LootDetailsPanel m_LootDetailsPanel;
    [SerializeField] private RectTransform m_HeatMask;
    [SerializeField] private TMP_Text m_Raw;
    [SerializeField] private TMP_Text m_Scrap;
    [SerializeField] private TMP_Text m_Chem;
    [SerializeField] private TMP_Text m_Electronic;
    [SerializeField] private TMP_Text m_Heat;


    private void Start() {
        DayTimeManager.GetInstance().m_ChangeFromLoot += ()=>{
            m_Self.SetActive(false);
        };

        DayTimeManager.GetInstance().m_ChangeToLoot += ()=>{
            m_Self.SetActive(true);
        };

        // Set Resource
        var ownedResources = MainGameManager.GetInstance().GetOwnedResources();
        m_Raw.text = $" : {(int)(ownedResources.Raw)}";
        m_Scrap.text = $" : {(int)(ownedResources.Scrap)}"; 
        m_Chem.text = $" : {(int)(ownedResources.Chem)}"; 
        m_Electronic.text = $" : {(int)(ownedResources.Electronic)}"; 
        m_Heat.text = $" : {(int)(ownedResources.Heat)}"; 

        // set heat area size
        m_HeatAreaSize = Mathf.Lerp(500f,3000f, MainGameManager.GetInstance().GetHeat() /1000f );
        m_HeatMask.sizeDelta = Vector2.one * m_HeatAreaSize;

        for (int i = 0; i < m_AllLocation.Count; i++)
        {
            var locationIcon = Instantiate(m_LocaionPrefab);
            locationIcon.name = m_AllLocation[i].DisplayName;

            // position
            locationIcon.transform.SetParent(m_LocationParent);

            if(locationIcon.TryGetComponent<LootLocationController>(out var lootLocationController)){

                // random id
                int id = Random.Range(0,999999);
                while (DayTimeManager.GetInstance().IsLocationIdUsed(id))
                {
                    id = Random.Range(0,999999);
                }
                DayTimeManager.GetInstance().StoreLootLocationController(id,lootLocationController);

                var scriptable = m_AllLocation[i];
                var tmp = new LootLocationControllerConfig{
                    Scriptable = scriptable,
                    OnClick = ()=>{m_LootDetailsPanel.SetLootDeail(lootLocationController,
                        DayTimeManager.GetInstance().GetLootLocationController(id).GetLootBotCount());},
                    LootBotCount = 0
                };
                lootLocationController.Init(tmp);

            }
            
        }
        // TODO : spawn heat bg 
        // TODO : Move by draging
    }

    public float GetHeatRadius(){
        return m_HeatAreaSize/2;
    }

    private void Update() {
        
    }

}
