using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnterNamePanel : MonoBehaviour
{
    [SerializeField] private GameObject m_BtnPrefab;
    [SerializeField] private Transform m_BtnParent;
    [SerializeField] private TMP_Text m_NameText;
    [SerializeField] private TMP_Text m_PlayerName;
    [SerializeField] private Button m_BackBtn;
    [SerializeField] private Button m_ComfirmBtn;
    private string m_EnterName = "";
    void Start()
    {
        if( MainGameManager.GetInstance().GetData<string>("PlayerName", "").ToString().Trim() !=""){
            m_PlayerName.text = MainGameManager.GetInstance().GetData<string>("PlayerName", "").ToString().Trim() ;
            this.gameObject.SetActive(false);
            return;
        }
        for (int i = 0; i < m_BtnParent.childCount; i++)
        {
            Destroy(m_BtnParent.GetChild(i).gameObject);
        }
        char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
        for (int i = 0; i < alpha.Length; i++)
        {
            string alphaBit = alpha[i].ToString();
            var newBtn = Instantiate(m_BtnPrefab,m_BtnParent);
            newBtn.GetComponent<AlphaBitBtn>().m_Text.text = alphaBit;
            newBtn.GetComponent<Button>().onClick.AddListener(()=> OnClickAlphaBtn(alphaBit) );
        }
        m_BackBtn.onClick.AddListener(OnClickRemoveLast);
        m_ComfirmBtn.onClick.AddListener(OnCLickComfirm);
    }

    private void OnCLickComfirm(){
        // TODO : comfirm name
        MainGameManager.GetInstance().SaveData<string>("PlayerName",m_EnterName);
        m_PlayerName.text = m_EnterName;
        this.gameObject.SetActive(false);
    }

    private void OnClickRemoveLast(){
        m_EnterName = m_EnterName.Substring(0, m_EnterName.Length - 1);
        m_NameText.text = m_EnterName;
    }

    private void OnClickAlphaBtn(string alphaBit){
        m_EnterName = m_EnterName.Insert(m_EnterName.Length,alphaBit);
        m_NameText.text = m_EnterName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
