
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
    [SerializeField] private Button m_ReloadBtn;
    [SerializeField] private Button m_CloseBtn;
    [SerializeField] private FireballEmitter m_FireballEmitter;
    [SerializeField] private TimmySlashController m_SlashController;
    [SerializeField] private KineticTransmitterController m_KineticTransmitterController;

    // TODO : "thrid hand" reload gun on use
    // TODO : gain ability AFTER complete level

    [Header("Check is used")]
    private bool m_IsFireBallUsed = false;
    private bool m_IsTimmySwordUsed = false;
    private bool m_IsNetUsed = false;
    private bool m_IsShieldUsed = false;



    void Start(){
        if( (int)MainGameManager.GetInstance().GetData<int>("Win1") == 1 ){
            m_FireballBtn.onClick.AddListener(OnClickFireball);
            m_SwordBtn.onClick.AddListener(OnClickSword);
        }else{
            m_FireballBtn.gameObject.SetActive(false);
            m_SwordBtn.gameObject.SetActive(false);
        }

        if( (int)MainGameManager.GetInstance().GetData<int>("Win3") == 1 ){
            m_NetBtn.onClick.AddListener(OnClickNet);
        }else{
            m_NetBtn.gameObject.SetActive(false);
        }

        if( (int)MainGameManager.GetInstance().GetData<int>("Win7") == 1 ){
            m_SwordBtn.onClick.AddListener(OnClickSword);
        }else{
            m_SwordBtn.gameObject.SetActive(false);
        }
        
        if( (int)MainGameManager.GetInstance().GetData<int>("Win12") == 1 ){
            m_ReloadBtn.onClick.AddListener(OnClickReload);
        }else{
            m_ReloadBtn.gameObject.SetActive(false);
        }
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

    private void OnClickReload(){
        if(m_IsShieldUsed)
            return;

        Reload();
        Close();
        m_IsShieldUsed = true;
        m_ReloadBtn.GetComponent<Image>().color = Color.gray;
        
    }


    private void Reload(){
        // ability reload 
        BaseDefenceManager.GetInstance().SetFullAmmo();
    }

}
