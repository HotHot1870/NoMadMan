using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using ExtendedButtons;

public class MapDialogController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Animator m_Animator;
    [SerializeField] private Button2D m_NextDialogBtn;
    [SerializeField] private Button2D m_EndDialogBtn;
    [SerializeField] private TMP_Text m_EndDialogBtnText;
    [SerializeField] private TMP_Text m_ContentText;
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_VoiceRecorder;
    private DialogScriptable m_CurDialogScriptable = null;


    public void Init(int startDialogId , Action onDialogEnd){
        if(startDialogId == -1){
            onDialogEnd?.Invoke();
            return;
        }
        m_Self.SetActive(true);
        
        m_ContentText.text = "";
        
        m_NextDialogBtn.onClick.RemoveAllListeners();
        MainGameManager.GetInstance().AddOnClickBaseAction(m_NextDialogBtn,m_NextDialogBtn.GetComponent<RectTransform>());
        m_NextDialogBtn.onClick.AddListener(()=>{
            if(m_CurDialogScriptable.NextId[0] == -1){
                // close dialog
                onDialogEnd?.Invoke();
            }else{
                // next dialog
                m_CurDialogScriptable = GetDialogScritapble(m_CurDialogScriptable.NextId[0]);
                SetDialogRow();
            }
        });

        m_EndDialogBtn.onClick.RemoveAllListeners();
        MainGameManager.GetInstance().AddOnClickBaseAction(m_EndDialogBtn,m_EndDialogBtn.GetComponent<RectTransform>());
        m_EndDialogBtn.onClick.AddListener(()=>{
            onDialogEnd?.Invoke();
            m_Self.SetActive(false);
        });

        m_CurDialogScriptable = GetDialogScritapble(startDialogId);
        m_AudioSource.PlayOneShot(m_VoiceRecorder);
        m_Animator.Play("Show");
        SetDialogRow();
    }

    void Start(){
        MainGameManager.GetInstance().AddNewAudioSource(m_AudioSource);
        m_EndDialogBtn.onClick.RemoveAllListeners();
        m_EndDialogBtn.onClick.AddListener(()=>m_Self.SetActive(false));
        MainGameManager.GetInstance().AddOnClickBaseAction(m_EndDialogBtn,m_EndDialogBtn.GetComponent<RectTransform>());
        m_Animator.Play("Hidden");
        m_Self.SetActive(false);
    }/*
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
        

    }*/

    private void SetContentText(){
        m_ContentText.text = m_CurDialogScriptable.Content;
    } 

    private void SetDialogRow(){
        SetContentText();
        // hide next dialog btn if no next dialog
        m_NextDialogBtn.gameObject.SetActive(m_CurDialogScriptable.NextId[0] != -1);
        m_EndDialogBtn.gameObject.SetActive(m_CurDialogScriptable.NextId[0] == -1);
    }

    private DialogScriptable GetDialogScritapble(int id){
        return MainGameManager.GetInstance().GetDialog(id);
    }

}
