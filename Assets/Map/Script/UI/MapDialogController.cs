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

    void Start(){
        m_BgAnimator.Play("Hidden");
        m_DialogBtn.onClick.AddListener(()=>{
            StartCoroutine(AddNewDialogRow());
        });
        m_Self.SetActive(false);
    }
    private IEnumerator AddNewDialogRow(){
        var newBlock = Instantiate(m_DialogRowPrefab,m_DialogRowParent);
#if UNITY_EDITOR
        EditorUtility.SetDirty(m_DialogRowParent);
#endif
        yield return null;
        m_ScrollRect.verticalNormalizedPosition = 0;

    }

    public void Init(){

    }

}
