using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MapDialogController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private GameObject m_DialogRowPrefab;
    [SerializeField] private Transform m_DialogRowParent;
    [SerializeField] private ScrollRect m_ScrollRect;
    [SerializeField] private Animator m_BgAnimator;
    [SerializeField] private Button m_NextDialogBtn;
    [SerializeField] private Button m_EndDialogBtn;
    [SerializeField] private TMP_Text m_EndDialogBtnText;
    private DialogScriptable m_CurDialogScriptable = null;

    public void Init(int startDialogId , Action onDialogEnd){
        if(startDialogId == -1){
            onDialogEnd?.Invoke();
            return;
        }
        
        for (int i = 0; i < m_DialogRowParent.childCount-1; i++)
        {
            Destroy(m_DialogRowParent.GetChild(i).gameObject);
        }


        m_Self.SetActive(true);
        
        m_NextDialogBtn.onClick.RemoveAllListeners();
        m_NextDialogBtn.onClick.AddListener(()=>{
            if(m_CurDialogScriptable.NextId[0] == -1){
                // close dialog
                onDialogEnd?.Invoke();
                m_BgAnimator.Play("Close");
            }else{
                // TODO : choose at the end
                // next dialog
                m_CurDialogScriptable = GetDialogScritapble(m_CurDialogScriptable.NextId[0]);
                SetDialogRow();
            }
        });

        m_EndDialogBtn.onClick.RemoveAllListeners();
        m_EndDialogBtn.onClick.AddListener(()=>{
            onDialogEnd?.Invoke();
            m_BgAnimator.Play("Close");
        });

        m_CurDialogScriptable = GetDialogScritapble(startDialogId);
        m_BgAnimator.Play("Open");
        SetDialogRow();
    }

    void Start(){
        m_BgAnimator.Play("Hidden");

        m_Self.SetActive(false);
    }
    private IEnumerator AddNewDialogRow(){
        var newBlock = Instantiate(m_DialogRowPrefab,m_DialogRowParent);
        newBlock.transform.SetSiblingIndex(newBlock.transform.GetSiblingIndex()-1);

        newBlock.GetComponent<MapDialogRowController>().Init(m_CurDialogScriptable);

#if UNITY_EDITOR
        EditorUtility.SetDirty(m_DialogRowParent);
#endif
        yield return null;
        // scroll to bottom 
        float passTime = 0f;
        float duration = 0.35f;
        float startVerticalNormalizedPosition = m_ScrollRect.verticalNormalizedPosition;
        
        while (passTime<duration)
        {
            passTime += Time.deltaTime;
            yield return null;
            m_ScrollRect.verticalNormalizedPosition = Mathf.Lerp(startVerticalNormalizedPosition,0,passTime/duration);
        }
        m_ScrollRect.verticalNormalizedPosition = 0;
        

    }

    private void SetDialogRow(){
        StartCoroutine(AddNewDialogRow());
        // hide next dialog btn if no next dialog
        m_NextDialogBtn.gameObject.SetActive(m_CurDialogScriptable.NextId[0] != -1);
        m_EndDialogBtnText.text = m_CurDialogScriptable.NextId[0] != -1?"Skip":"End";
    }

    private DialogScriptable GetDialogScritapble(int id){
        return MainGameManager.GetInstance().GetDialog(id);
    }

}
