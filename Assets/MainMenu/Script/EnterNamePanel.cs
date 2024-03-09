using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ExtendedButtons;

public class EnterNamePanel : MonoBehaviour
{
    [SerializeField] private GameObject m_BtnPrefab;
    [SerializeField] private Transform m_BtnParent;
    [SerializeField] private TMP_Text m_NameText;
    [SerializeField] private Button2D m_BackBtn;
    [SerializeField] private Button2D m_ComfirmBtn;
    private string m_EnterName = "";
    [SerializeField] private Image m_Black;
    [SerializeField] private AnimationCurve m_BlackFadeCurve;
    void Start()
    {
        if( MainGameManager.GetInstance().GetData<string>("PlayerName", "").ToString().Trim() !=""){
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

            var btnComponent = newBtn.GetComponent<Button2D>();

            MainGameManager.GetInstance().AddOnClickBaseAction(btnComponent,newBtn.GetComponent<RectTransform>());
            btnComponent.onClick.AddListener(()=> OnClickAlphaBtn(alphaBit) );
        }

        MainGameManager.GetInstance().AddOnClickBaseAction(m_BackBtn,m_BackBtn.GetComponent<RectTransform>());
        m_BackBtn.onClick.AddListener(OnClickRemoveLast);

        
        
        MainGameManager.GetInstance().AddOnClickBaseAction(m_ComfirmBtn,m_ComfirmBtn.GetComponent<RectTransform>());
        m_ComfirmBtn.onClick.AddListener(OnCLickComfirm);
    }

    private void OnCLickComfirm(){
        // comfirm name
        MainGameManager.GetInstance().SaveData<string>("PlayerName",m_EnterName);
        StartCoroutine(BlackFadeOut());
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

    private IEnumerator BlackFadeOut(){
        float passTime = 0;
        float duration = 1.5f;
        while (passTime<duration)
        {
            yield return null;
            passTime += Time.deltaTime;
            m_Black.color = Color.Lerp(Color.black,Color.clear, m_BlackFadeCurve.Evaluate(passTime/duration));
        }
        Destroy(m_Black.gameObject);
    }
}
