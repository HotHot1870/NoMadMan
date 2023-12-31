
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AssistPanelController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self; 
    [SerializeField] private Animator m_Animator;
    [SerializeField] private Button m_FireballBtn; 
    [SerializeField] private Button m_SwordBtn;
    [SerializeField] private Button m_NetBtn;
    [SerializeField] private Button m_ShieldBtn;
    [SerializeField] private Button m_CloseBtn;
    [SerializeField] private FireballEmitter m_FireballEmitter;
    [SerializeField] private TimmySlashController m_SlashController;
    [SerializeField] private KineticTransmitterController m_KineticTransmitterController;

    void Start(){
        m_FireballBtn.onClick.AddListener(OnClickFireball);
        m_SwordBtn.onClick.AddListener(OnClickSword);
        m_NetBtn.onClick.AddListener(OnClickNet);
        m_ShieldBtn.onClick.AddListener(OnClickShield);
        m_CloseBtn.onClick.AddListener(Close);
        m_Self.SetActive(false);

    }

    private void Close(){
        m_Animator.Play("Down");
    }

    public void Init(){
        m_Self.SetActive(true);
        m_Animator.Play("Up");

    }

    private void OnClickFireball(){
        m_FireballEmitter.ShootFireBall(50f,6f,30);
        Close();
    }


    private void OnClickSword(){
        m_SlashController.Slash();
        Close();
    }


    private void OnClickNet(){
        m_KineticTransmitterController.init();
        Close();
        
    }

    private void OnClickShield(){
        StartCoroutine(SetShield());
        Close();
        
    }


    private IEnumerator SetShield(){
        BaseDefenceManager.GetInstance().SetShieldStage(true);
        yield return new WaitForSeconds( (BaseDefenceManager.GetInstance().GetLocationScriptable().Level+1) * 2f);
        BaseDefenceManager.GetInstance().SetShieldStage(false);
    }

}
