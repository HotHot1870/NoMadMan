using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MainGameNameSpace;
using UnityEngine.SceneManagement;

public class BaseDefenseResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Image m_Bg;

    [SerializeField] private TMP_Text m_Title;
    [SerializeField] private Button m_NextBtn;
    [SerializeField] private BaseDefenseResultPanelRowContent m_Wall;
    [SerializeField] private BaseDefenseResultPanelRowContent m_Raw;
    [SerializeField] private BaseDefenseResultPanelRowContent m_Scrap;
    [SerializeField] private BaseDefenseResultPanelRowContent m_Chem;
    [SerializeField] private BaseDefenseResultPanelRowContent m_Electronic;
    [SerializeField] private BaseDefenseResultPanelRowContent m_Bot;
    [SerializeField] private BaseDefenseResultPanelRowContent m_Heat;
    private ResourcesRecord m_BeforeBattleResourceData;
    private float m_BeforeBattleWallHp;

    private void Start()
    {
        RecordBeforeBattleData();
        m_Self.SetActive(false);
    }

    public void RecordBeforeBattleData()
    {
        m_BeforeBattleWallHp = MainGameManager.GetInstance().GetWallCurHp();
        var beforeBattleResourceData = MainGameManager.GetInstance().GetOwnedResources();
        m_BeforeBattleResourceData = new ResourcesRecord
        {
            Raw = beforeBattleResourceData.Raw,
            Scrap = beforeBattleResourceData.Scrap,
            Chem = beforeBattleResourceData.Chem,
            Electronic = beforeBattleResourceData.Electronic,
            Bot = beforeBattleResourceData.Bot,
            Heat = beforeBattleResourceData.Heat
        };
    }

    public void ShowResult(bool isLose)
    {
        m_Self.SetActive(true);
        m_Heat.gameObject.SetActive(false);

        // before
        m_Wall.Before.text = ((int)m_BeforeBattleWallHp).ToString();

        m_Raw.Before.text = ((int)m_BeforeBattleResourceData.Raw).ToString();
        m_Scrap.Before.text = ((int)m_BeforeBattleResourceData.Scrap).ToString();
        m_Chem.Before.text = ((int)m_BeforeBattleResourceData.Chem).ToString();
        m_Electronic.Before.text = ((int)m_BeforeBattleResourceData.Electronic).ToString();
        m_Bot.Before.text = ((int)m_BeforeBattleResourceData.Bot).ToString();
        //m_Heat.Before.text = ((int)m_BeforeBattleResourceData.Heat).ToString();

        // changes
        var afterBattleResourceData = MainGameManager.GetInstance().GetOwnedResources();

        float statValue = 0;
        statValue = MainGameManager.GetInstance().GetWallCurHp() - m_BeforeBattleWallHp;
        string textString = statValue >= 0 ? "+" : "";
        textString += ((int)statValue).ToString();
        m_Wall.Changes.text = textString;

        statValue = afterBattleResourceData.Raw - m_BeforeBattleResourceData.Raw;
        textString = statValue >= 0 ? "+" : "";
        textString += ((int)statValue).ToString();
        m_Raw.Changes.text = textString;

        statValue = afterBattleResourceData.Scrap - m_BeforeBattleResourceData.Scrap;
        textString = statValue >= 0 ? "+" : "";
        textString += ((int)statValue).ToString();
        m_Scrap.Changes.text = textString;

        statValue = afterBattleResourceData.Chem - m_BeforeBattleResourceData.Chem;
        textString = statValue >= 0 ? "+" : "";
        textString += ((int)statValue).ToString();
        m_Chem.Changes.text = textString;

        statValue = afterBattleResourceData.Electronic - m_BeforeBattleResourceData.Electronic;
        textString = statValue >= 0 ? "+" : "";
        textString += ((int)statValue).ToString();
        m_Electronic.Changes.text = textString;

        statValue = afterBattleResourceData.Bot - m_BeforeBattleResourceData.Bot;
        textString = statValue >= 0 ? "+" : "";
        textString += ((int)statValue).ToString();
        m_Bot.Changes.text = textString;
/*
        statValue = afterBattleResourceData.Heat - m_BeforeBattleResourceData.Heat;
        textString = statValue >= 0 ? "+" : "";
        textString += ((int)statValue).ToString();
        m_Heat.Changes.text = textString;*/



        // after
        m_Wall.After.text = ((int)MainGameManager.GetInstance().GetWallCurHp()).ToString();

        m_Raw.After.text = ((int)afterBattleResourceData.Raw).ToString();
        m_Scrap.After.text = ((int)afterBattleResourceData.Scrap).ToString();
        m_Chem.After.text = ((int)afterBattleResourceData.Chem).ToString();
        m_Electronic.After.text = ((int)afterBattleResourceData.Electronic).ToString();
        m_Bot.After.text = ((int)afterBattleResourceData.Bot).ToString();
        //m_Heat.After.text = ((int)afterBattleResourceData.Heat).ToString();

        m_NextBtn.onClick.RemoveAllListeners();


        m_NextBtn.onClick.AddListener(() =>
        {
            if(isLose){
                DamageReport();
            }else{
                BaseDefenseManager.GetInstance().SetTimmyAssitancePanel();
            }
        });
    }

    private void DamageReport(){
        m_Title.text = "Damage Report";
        m_Bg.color = Color.red;

        // Before
        m_Wall.gameObject.SetActive(false);
        m_Bot.gameObject.SetActive(false);
        m_Heat.gameObject.SetActive(true);
        var afterBattleResourceData = MainGameManager.GetInstance().GetOwnedResources();

        m_Raw.Before.text = ((int)afterBattleResourceData.Raw).ToString();
        m_Scrap.Before.text = ((int)afterBattleResourceData.Scrap).ToString();
        m_Chem.Before.text = ((int)afterBattleResourceData.Chem).ToString();
        m_Electronic.Before.text = ((int)afterBattleResourceData.Electronic).ToString();
        //m_Bot.Before.text = ((int)afterBattleResourceData.Bot).ToString();
        m_Heat.Before.text = ((int)afterBattleResourceData.Heat).ToString();


        // changes
        ResourcesRecord DamagedResource = new ResourcesRecord();
        float seedRandom = Random.Range(0f, 0.5f);
        DamagedResource.Raw = Mathf.Lerp(0, afterBattleResourceData.Raw, seedRandom);
        m_Raw.Changes.text = $"-{(int)DamagedResource.Raw}";

        seedRandom = Random.Range(0f, 0.5f);
        DamagedResource.Scrap = Mathf.Lerp(0, afterBattleResourceData.Scrap , seedRandom);
        m_Scrap.Changes.text = $"-{(int)DamagedResource.Scrap}";

        seedRandom = Random.Range(0f, 0.5f);
        DamagedResource.Chem = Mathf.Lerp(0, afterBattleResourceData.Chem, seedRandom);
        m_Chem.Changes.text = $"-{(int)DamagedResource.Chem}";

        seedRandom = Random.Range(0f, 0.5f);
        DamagedResource.Electronic = Mathf.Lerp(0, afterBattleResourceData.Electronic, seedRandom);
        m_Electronic.Changes.text = $"-{(int)DamagedResource.Electronic}";
/*
        seedRandom = Random.Range(0f, 0.5f);
        DamagedResource.Bot = (int)Mathf.Lerp(0, afterBattleResourceData.Bot, seedRandom);
        m_Bot.Changes.text =  $"-{(int)DamagedResource.Bot}";*/

        DamagedResource.Heat = afterBattleResourceData.Heat * 0.25f;
        m_Heat.Changes.text =  $"-{(int)DamagedResource.Heat}";

        MainGameManager.GetInstance().GainResources( DamagedResource.GetReverse() );        
        
        // after
        afterBattleResourceData = MainGameManager.GetInstance().GetOwnedResources();
        m_Raw.After.text = ((int)afterBattleResourceData.Raw).ToString();
        m_Scrap.After.text = ((int)afterBattleResourceData.Scrap).ToString();
        m_Chem.After.text = ((int)afterBattleResourceData.Chem).ToString();
        m_Electronic.After.text = ((int)afterBattleResourceData.Electronic).ToString();
        //m_Bot.After.text = ((int)afterBattleResourceData.Bot).ToString();
        m_Heat.After.text = ((int)afterBattleResourceData.Heat).ToString();

        m_NextBtn.onClick.RemoveAllListeners();

        m_NextBtn.onClick.AddListener(() =>
        {
            BaseDefenseManager.GetInstance().SetTimmyAssitancePanel();
        });
    }
}
