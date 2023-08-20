using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using DayTimeNameSpace;
using LootResultPanelNameSpace;
using MainGameNameSpace;
using System;

namespace DayTimeNameSpace
{
    public enum DayTimeStage{
        Loot,
        Weapon,
        Home,
    }
}


public class DayTimeManager : MonoBehaviour
{
    public static DayTimeManager m_Instance = null;
    private DayTimeStage m_GameStage = DayTimeStage.Loot;
    [SerializeField] private TMP_Text m_BotCountText;
    [SerializeField] private Button m_FinializeBtn;
    [SerializeField] private GameObject m_TopBar;

    [Header("Option")]
    [SerializeField] private Button m_OptionBtn;
    [SerializeField] private GameObject m_OptionPanel;

    #region From
    public Action m_ChangeFromLoot = null;
    public Action m_ChangeFromWeapon = null;
    public Action m_ChangeFromHome = null;
    #endregion

    #region To
    public Action m_ChangeToLoot = null;
    public Action m_ChangeToWeapon = null;
    public Action m_ChangeToHome = null;

    #endregion


    [Header("Loot")]
    [SerializeField] private Button m_LootBtn;
    [SerializeField] private LootMapController m_LootMapController;
    [SerializeField] private LootResultPanel m_LootResultPanel;
    [SerializeField] private TotalResourcePanel m_TotalResourcePanel;
    private int m_CurrentShowResultIndex = 0;
    private List<LootLocationController> m_AllLocationWithBot = new List<LootLocationController>();
    private ResourcesRecord m_TotalRescourseChanges = new ResourcesRecord();
    private Dictionary<int,LootLocationController> m_AllSpawnedLootLocation = new Dictionary<int, LootLocationController>();
    

    [Header("Weapon")]
    [SerializeField] private Button m_WeaponPanelBtn;
    [SerializeField] private WeaponPanelController m_WeaponPanelController;

    [Header("Home")]
    [SerializeField] private Button m_HomePanelBtn;
    [SerializeField] private HomePanel m_HomePanel;

    public static DayTimeManager GetInstance(){
        if(m_Instance==null){
            m_Instance = new GameObject().AddComponent<DayTimeManager>();
        }
        return m_Instance;
    }


    private void Awake() {
        if(m_Instance==null){
            m_Instance = this;
        }else{
            Destroy(this);
            return;
        }
    }

    private void Start() {
        SetUsedBotCountText();
        ChangeGameStage(DayTimeStage.Loot);

        m_WeaponPanelBtn.onClick.AddListener(()=>{
            ChangeGameStage(DayTimeStage.Weapon);
        });  
        m_HomePanelBtn.onClick.AddListener(()=>{
            ChangeGameStage(DayTimeStage.Home);
        });      
        
        m_LootBtn.onClick.AddListener(()=>{
            ChangeGameStage(DayTimeStage.Loot);
        });
        
        

        m_FinializeBtn.onClick.AddListener(()=>{
            OnClickResultPanelNextBtn();
        });

        m_OptionBtn.onClick.AddListener(()=>{
            m_OptionPanel.SetActive(true);
        });
    }

        public void ChangeGameStage(DayTimeStage newStage){
        switch (m_GameStage)
        {
            case DayTimeStage.Loot:
                m_ChangeFromLoot?.Invoke();
            break;
            case DayTimeStage.Weapon:
                m_ChangeFromWeapon?.Invoke();
            break;
            case DayTimeStage.Home:
                m_ChangeFromHome?.Invoke();
            break;
            default:
            break;
        }

        switch (newStage)
        {
            case DayTimeStage.Loot:
                m_ChangeToLoot?.Invoke();
            break;
            case DayTimeStage.Weapon:
                m_ChangeToWeapon?.Invoke();
            break;
            case DayTimeStage.Home:
                m_ChangeToHome?.Invoke();
            break;
            default:
            break;
        }

        m_GameStage = newStage;
    }

    public void SetTopBar(bool isActive){
        m_TopBar.SetActive(isActive);
    }

    public void SetWeaponDetailPanel(GunScriptable gunScriptable){
        m_WeaponPanelController.SetInfo(gunScriptable);
    }

    public int GetTotalBotUsed(){
        m_AllLocationWithBot = m_AllSpawnedLootLocation.Where(x=>x.Value.GetLootBotCount()>0).Select(x=>x.Value).ToList();
        int ans = 0;
        for (int i = 0; i < m_AllLocationWithBot.Count; i++)
        {
            ans += m_AllLocationWithBot[i].GetLootBotCount();
        }
        return ans;
    }

    public LootLocationController GetLootLocationController(int id){
        return m_AllSpawnedLootLocation[id];
    }

    public void OnClickResultPanelNextBtn(){
        SetTopBar(false);
        if(m_CurrentShowResultIndex>=m_AllLocationWithBot.Count){
            m_LootResultPanel.TurnOff();
            // show total resources changes
            m_TotalResourcePanel.Init(m_TotalRescourseChanges);
            return;
        }
        m_TotalRescourseChanges.Change( m_LootResultPanel.Init(
            m_AllLocationWithBot[m_CurrentShowResultIndex].GetScriptable(),
            m_AllLocationWithBot[m_CurrentShowResultIndex].GetLootBotCount()) );
        m_CurrentShowResultIndex ++;
    }

    public bool IsLocationIdUsed(int id){
        return m_AllSpawnedLootLocation.ContainsKey(id);
    }

    public void SetUsedBotCountText(){
        var totalBotCount = MainGameManager.GetInstance().GetOwnedBotCount();
        m_BotCountText.text = $"{totalBotCount - GetTotalBotUsed()} / { totalBotCount }";

    }

    
    public float GetHeatRadius(){
        return m_LootMapController.GetHeatRadius();
    }

    public void StoreLootLocationController(int id,LootLocationController lootLocationController){
        m_AllSpawnedLootLocation.Add(id,lootLocationController);
    }
    

}
