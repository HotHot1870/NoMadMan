using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MainGameNameSpace;
using UnityEngine.SceneManagement;

public class BaseDefenceResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    /*
    [SerializeField] private Image m_Bg;

    [SerializeField] private TMP_Text m_Title;
    [SerializeField] private Button m_NextBtn;
    [SerializeField] private BaseDefenceResultPanelRowContent m_Wall;
    [SerializeField] private BaseDefenceResultPanelRowContent m_Raw;
    [SerializeField] private BaseDefenceResultPanelRowContent m_Scrap;
    [SerializeField] private BaseDefenceResultPanelRowContent m_Chem;
    [SerializeField] private BaseDefenceResultPanelRowContent m_Electronic;
    [SerializeField] private BaseDefenceResultPanelRowContent m_Bot;
    [SerializeField] private BaseDefenceResultPanelRowContent m_Heat;
    private float m_BeforeBattleWallHp;
*/
    private void Start()
    {
        //RecordBeforeBattleData();
        m_Self.SetActive(false);
    }
/*
    public void RecordBeforeBattleData()
    {
        m_BeforeBattleWallHp = MainGameManager.GetInstance().GetWallCurHp();
    }

    public void ShowResult(bool isLose)
    {
        m_Self.SetActive(true);
        m_Heat.gameObject.SetActive(false);

        // before
        m_Wall.Before.text = ((int)m_BeforeBattleWallHp).ToString();


        float statValue = 0;
        statValue = MainGameManager.GetInstance().GetWallCurHp() - m_BeforeBattleWallHp;
        string textString = statValue >= 0 ? "+" : "";
        textString += ((int)statValue).ToString();
        m_Wall.Changes.text = textString;

        // after
        m_Wall.After.text = ((int)MainGameManager.GetInstance().GetWallCurHp()).ToString();

        m_NextBtn.onClick.RemoveAllListeners();


        m_NextBtn.onClick.AddListener(() =>
        {
            if(isLose){
                DamageReport();
            }else{
                //BaseDefenceManager.GetInstance().SetTimmyAssitancePanel();
            }
        });
    }

    private void DamageReport(){
        m_Title.text = "Damage Report";
        m_Bg.color = Color.red;

        // Before
        m_Wall.gameObject.SetActive(false);

        m_NextBtn.onClick.RemoveAllListeners();

        m_NextBtn.onClick.AddListener(() =>
        {
            //BaseDefenceManager.GetInstance().SetTimmyAssitancePanel();
        });
    }
    */
}