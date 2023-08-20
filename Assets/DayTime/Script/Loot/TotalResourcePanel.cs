using System.Collections;
using System.Collections.Generic;
using LootResultPanelNameSpace;
using MainGameNameSpace;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TotalResourcePanel : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private LootResultResourcesText m_Before;
    [SerializeField] private LootResultResourcesText m_After;
    [SerializeField] private Button m_NextBtn;

    private void Start() {
        m_NextBtn.onClick.AddListener(()=>{
            SceneManager.LoadScene("BaseDefense");
        });
        m_Self.SetActive(false);
    }

    public void Init(ResourcesRecord totalRescourseChanges){
        m_Self.SetActive(true);

        // resource before changes
        var ownedResource = MainGameManager.GetInstance().GetOwnedResources();
        m_Before.Raw.text = $"{(int)ownedResource.Raw}";
        m_Before.Scrap.text = $"{(int)ownedResource.Scrap}";
        m_Before.Chem.text = $"{(int)ownedResource.Chem}";
        m_Before.Electronic.text = $"{(int)ownedResource.Electronic }";
        m_Before.Bot.text = $"{(int)ownedResource.Bot }";
        m_Before.Heat.text = $"{MainGameManager.GetInstance().GetHeat() }";

        MainGameManager.GetInstance().GainResources( totalRescourseChanges );

        
        // resource after changes
        ownedResource = MainGameManager.GetInstance().GetOwnedResources();
        m_After.Raw.text = $"{(int)ownedResource.Raw}";
        m_After.Scrap.text = $"{(int)ownedResource.Scrap}";
        m_After.Chem.text = $"{(int)ownedResource.Chem}";
        m_After.Electronic.text = $"{(int)ownedResource.Electronic }";
        m_After.Bot.text = $"{(int)ownedResource.Bot }";
        m_After.Heat.text = $"{MainGameManager.GetInstance().GetHeat() }";
    }

}
