
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

    [Header("CoolDown")]
    private bool m_IsFireBallUsed = false;
    private bool m_IsTimmySwordUsed = false;
    private bool m_IsNetUsed = false;
    private bool m_IsShieldUsed = false;


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
        if(m_IsFireBallUsed)
            return;

        m_FireballEmitter.ShootFireBall(65f,6f,13 + BaseDefenceManager.GetInstance().GetLocationScriptable().Level *3);
        Close();

        m_IsFireBallUsed = true;
        m_FireballBtn.GetComponent<Image>().color = Color.gray;
    }


    private void OnClickSword(){
        if(m_IsTimmySwordUsed)
            return;

        m_SlashController.Slash();
        Close();
        m_IsTimmySwordUsed = true;
        m_SwordBtn.GetComponent<Image>().color = Color.gray;
    }


    private void OnClickNet(){
        if(m_IsNetUsed)
            return;

        m_KineticTransmitterController.init();
        Close();
        m_IsNetUsed = true;
        m_NetBtn.GetComponent<Image>().color = Color.gray;
        
    }

    private void OnClickShield(){
        if(m_IsShieldUsed)
            return;

        StartCoroutine(SetShield());
        Close();
        m_IsShieldUsed = true;
        m_ShieldBtn.GetComponent<Image>().color = Color.gray;
        
    }


    private IEnumerator SetShield(){
        BaseDefenceManager.GetInstance().SetShieldStage(true);
        yield return new WaitForSeconds( (BaseDefenceManager.GetInstance().GetLocationScriptable().Level+1) * 2f);
        BaseDefenceManager.GetInstance().SetShieldStage(false);
    }

}
