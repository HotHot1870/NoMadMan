using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class AssistPanelController : MonoBehaviour
{

    [Header("Location")]
    [SerializeField] private GameObject m_Self; 
    [SerializeField] private Animator m_Animator;
    [SerializeField] private TMP_Text m_LocationName;  
    [SerializeField] private Button m_FireballBtn; 
    [SerializeField] private Button m_SwordBtn;
    [SerializeField] private Button m_NetBtn;
    [SerializeField] private Button m_ShieldBtn;

    void Start(){
        m_FireballBtn.onClick.AddListener(OnClickFireball);
        m_SwordBtn.onClick.AddListener(OnClickSword);
        m_NetBtn.onClick.AddListener(OnClickNet);
        m_ShieldBtn.onClick.AddListener(OnClickShield);
        m_Animator.Play("Hidden");
        m_Self.SetActive(false);

    }

    private void OnClickFireball(){

    }


    private void OnClickSword(){
        
    }


    private void OnClickNet(){
        
    }

    private void OnClickShield(){
        
    }

}
