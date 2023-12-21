using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapDialogRowController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    // TODO : show action
    [SerializeField] private Animator m_BgAnimator;
    [SerializeField] private TMP_Text m_DialogText;  
    [SerializeField] private TMP_Text m_SpeakText;  

    public void Init(DialogScriptable dialogScriptable){
        m_DialogText.text = dialogScriptable.EngDialog;
        m_SpeakText.text = dialogScriptable.SpeakerName;
    }
}
