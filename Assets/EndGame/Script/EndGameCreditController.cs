using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameCreditController : MonoBehaviour
{
    [SerializeField] private List<CanvasGroup> m_AllTextGroup = new List<CanvasGroup>();
    [SerializeField] private GameObject m_PlayerTextGroup;
    [SerializeField] private Text m_PlayerNameText ;

    void Start(){
        // close all text
        for (int i = 0; i < m_AllTextGroup.Count; i++)
        {
            m_AllTextGroup[i].alpha = 0;
        }
        StartCoroutine(ShowCridit());
    }

    private IEnumerator ShowCridit(){
        float passTime = 0f;

        // row cridit
        for (int i = 0; i < m_AllTextGroup.Count; i++)
        {
            int index = i;
            // fade in
            passTime = 0f;
            float fadeIn = 0.5f;
            while (passTime < fadeIn)
            {
                m_AllTextGroup[index].alpha = passTime/fadeIn;
                passTime += Time.deltaTime;
                yield return null;
            }
            m_AllTextGroup[index].alpha = 1;
            // stay
            passTime = 0f;
            float stayTime = 3f;
            while (passTime < stayTime)
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

        
    }
}
