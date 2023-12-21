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
    [SerializeField] private Transform m_BtnParent;
    [SerializeField] private Button m_NextDialogBtn;
    [SerializeField] private Button m_EndDailogBtn;
    [SerializeField] private TMP_Text m_EndDialogBtnText;
    private DialogScriptable m_CurDialogScriptable = null;

    public void Init(int startDialogId){
        m_Self.SetActive(true);
        m_CurDialogScriptable = GetDialogScritapble(startDialogId);
        m_BgAnimator.Play("Open");
        SetDialogRow();
    }

    void Start(){
        m_BgAnimator.Play("Hidden");
        m_NextDialogBtn.onClick.AddListener(()=>{
            if(m_CurDialogScriptable.NextId[0] == -1){
                // close dialog
                
                MapLocationScriptable locationData = MapManager.GetInstance().GetLocationController().GetScriptable();
                MainGameManager.GetInstance().SetBaseDefenceScene(locationData);
            }else{
                // TODO : choose at the end
                // next dialog
                m_CurDialogScriptable = GetDialogScritapble(m_CurDialogScriptable.NextId[0]);
                SetDialogRow();
            }
        });

        m_EndDailogBtn.onClick.AddListener(()=>{
            MapLocationScriptable locationData = MapManager.GetInstance().GetLocationController().GetScriptable();
            MainGameManager.GetInstance().SetBaseDefenceScene(locationData);
        });
        m_Self.SetActive(false);
    }
    private IEnumerator AddNewDialogRow(){
        var newBlock = Instantiate(m_DialogRowPrefab,m_DialogRowParent);

        newBlock.GetComponent<MapDialogRowController>().Init(m_CurDialogScriptable);

#if UNITY_EDITOR
        EditorUtility.SetDirty(m_DialogRowParent);
#endif
        m_BtnParent.SetAsLastSibling();
        m_ScrollRect.verticalNormalizedPosition = 0;
        yield return null;

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
