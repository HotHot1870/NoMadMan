using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapDialogController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private GameObject m_DialogRowPrefab;
    [SerializeField] private Transform m_DialogRowParent;
    [SerializeField] private ScrollRect m_ScrollRect;
    [SerializeField] private Animator m_BgAnimator;
    [SerializeField] private Button m_DialogBtn;
    [SerializeField] private TMP_Text m_DialogBtnText;  
    private DialogScriptable m_CurDialogScriptable = null;

    public void Init(int startDialogId){
        m_CurDialogScriptable = GetDialogScritapble(startDialogId);
        SetDialogRow();
    }

    void Start(){
        m_BgAnimator.Play("Hidden");
        m_DialogBtn.onClick.AddListener(()=>{
            if(m_CurDialogScriptable.NextId[0] == -1){
                // close dialog
            }else{
                // TODO : choose at the end
                // next dialog
                m_CurDialogScriptable = GetDialogScritapble(m_CurDialogScriptable.NextId[0]);
                SetDialogRow();
            }
        });
        m_Self.SetActive(false);
    }
    private IEnumerator AddNewDialogRow(){
        var newBlock = Instantiate(m_DialogRowPrefab,m_DialogRowParent);

        newBlock.GetComponent<MapDialogRowController>().Init(m_CurDialogScriptable);

#if UNITY_EDITOR
        EditorUtility.SetDirty(m_DialogRowParent);
#endif
        yield return null;
        m_ScrollRect.verticalNormalizedPosition = 0;

    }

    private void SetDialogRow(){
        StartCoroutine(AddNewDialogRow());
        if(m_CurDialogScriptable.NextId[0] == -1){
            // set dialog btn text to close
            m_DialogBtnText.text = "Close";
        }else{
            m_DialogBtnText.text = "Next";
        }

    }

    private DialogScriptable GetDialogScritapble(int id){
        return MainGameManager.GetInstance().GetDialog(id);
    }

}
