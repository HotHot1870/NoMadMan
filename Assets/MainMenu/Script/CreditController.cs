using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private GameObject m_BG;
    [SerializeField] private List<CanvasGroup> m_AllTextGroup = new List<CanvasGroup>();
    [SerializeField] private CanvasGroup m_PlayerTextTitle;
    [SerializeField] private CanvasGroup m_PlayerTextContent;
    [SerializeField] private TMP_Text m_PlayerNameText;
    [SerializeField] private Button m_SkipFullCreenBtn;
    private bool m_IsSkipNext = false;

    void Start(){
        m_SkipFullCreenBtn.onClick.AddListener(()=>{m_IsSkipNext = true;});
        // close all text
        for (int i = 0; i < m_AllTextGroup.Count; i++)
        {
            m_AllTextGroup[i].alpha = 0;
        }
        m_PlayerTextTitle.alpha = 0;
        m_PlayerTextContent.alpha = 0;
        m_Self.SetActive(false);
    }

    public void Init(bool showBG = false){
        m_Self.SetActive(true);
        if(showBG)
            StartCoroutine(ShowBG());

        StartCoroutine(ShowCridit());
    }

    private IEnumerator ShowBG(){
        m_BG.SetActive(true);
        float passTime = 0f;
        float duration = 0.5f;
        var bgImage = m_BG.GetComponent<Image>();
        while (passTime < duration)
        {
            bgImage.color = new Color(0,0,0,passTime/ (duration*1.05f));
            passTime += Time.deltaTime;
            yield return null;
        }

    }

    private IEnumerator ShowCridit(){
        float passTime = 0f;

        // row cridit
        for (int i = 0; i < m_AllTextGroup.Count; i++)
        {
            m_IsSkipNext = false;
            int index = i;
            // fade in
            passTime = 0f;
            float fadeIn = 0.5f;
            while (passTime < fadeIn && !m_IsSkipNext)
            {
                m_AllTextGroup[index].alpha = passTime/fadeIn;
                passTime += Time.deltaTime;
                yield return null;
            }
            m_AllTextGroup[index].alpha = 1;
            // stay
            passTime = 0f;
            var stayTime = 3;
            while (passTime < stayTime && !m_IsSkipNext)
            {
                passTime += Time.deltaTime;
                yield return null;
            }

            // fade out
            passTime = 0f;
            float fadeOut = 0.5f;
            while (passTime < fadeOut)
            {
                m_AllTextGroup[index].alpha = (fadeIn - passTime)/fadeIn;
                passTime += Time.deltaTime;
                yield return null;
            }
            m_AllTextGroup[index].alpha = 0;
            yield return new WaitForSeconds(0.75f);
        }

        // TODO : player credit
        passTime = 0f;
        float thankPlayerFadeIn = 0.5f;
        while (passTime < thankPlayerFadeIn)
        {
            m_PlayerTextTitle.alpha = (thankPlayerFadeIn - passTime)/thankPlayerFadeIn;
            passTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        
        passTime = 0f;
        float thankPlayerSecondFadeIn = 0.5f;
        while (passTime < thankPlayerSecondFadeIn)
        {
            m_PlayerTextContent.alpha = (thankPlayerSecondFadeIn - passTime)/thankPlayerSecondFadeIn;
            passTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(3f);

        passTime = 0f;
        while (passTime < 0.5f)
        {
            m_PlayerTextContent.alpha = (0.5f - passTime)/0.5f;
            m_PlayerTextTitle.alpha = (0.5f - passTime)/0.5f;
            passTime += Time.deltaTime;
            yield return null;
        }

        m_PlayerTextTitle.alpha = 0;
        m_PlayerTextContent.alpha = 0;

        // go backm to main if end game
        if(SceneManager.GetActiveScene().name=="EndGame"){
            SceneManager.LoadSceneAsync("MainMenu");
        }
    }
}
