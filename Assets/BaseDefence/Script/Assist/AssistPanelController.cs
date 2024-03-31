
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using ExtendedButtons;

public class AssistPanelController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self; 
    [SerializeField] private Animator m_Animator;
    [SerializeField] private Button2D m_FireballBtn; 
    [SerializeField] private Image m_FireballImage;
    [SerializeField] private Button2D m_SwordBtn;
    [SerializeField] private Image m_SwordImage;
    [SerializeField] private Button2D m_NetBtn;
    [SerializeField] private Image m_NetImage;
    [SerializeField] private Button2D m_ReloadBtn;
    [SerializeField] private Image m_ReloadImage;

    [SerializeField] private Button2D m_CloseBtn;
    [SerializeField] private FireballEmitter m_FireballEmitter;
    [SerializeField] private TimmySlashController m_SlashController;
    [SerializeField] private KineticTransmitterController m_KineticTransmitterController;

    // TODO : CD on use , not disable

    [Header("Check is used")]
    private bool m_IsFireBallUsed = false;
    private bool m_IsTimmySwordUsed = false;
    private bool m_IsNetUsed = false;
    private bool m_IsReloadUsed = false;

    [Header("Audio")]
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_NetSoundClip;
    [SerializeField] private AudioClip m_ReloadSoundClip;


    public enum AssistType
    {
        FireBall = 0,
        Sword = 1,
        Net = 2,
        Reload = 3
    }

    void Start(){
        MainGameManager.GetInstance().AddNewAudioSource(m_AudioSource);
        if( (int)MainGameManager.GetInstance().GetData<int>("Win1") == 1 ){
            MainGameManager.GetInstance().AddOnClickBaseAction(m_FireballBtn,m_FireballBtn.GetComponent<RectTransform>());
            m_FireballBtn.onClick.AddListener(OnClickFireball);
        }else{
            m_FireballBtn.gameObject.SetActive(false);
        }

        if( (int)MainGameManager.GetInstance().GetData<int>("Win3") == 1 ){
            MainGameManager.GetInstance().AddOnClickBaseAction(m_NetBtn,m_NetBtn.GetComponent<RectTransform>());
            m_NetBtn.onClick.AddListener(OnClickNet);
        }else{
            m_NetBtn.gameObject.SetActive(false);
        }

        if( (int)MainGameManager.GetInstance().GetData<int>("Win7") == 1 ){
            MainGameManager.GetInstance().AddOnClickBaseAction(m_SwordBtn,m_SwordBtn.GetComponent<RectTransform>());
            m_SwordBtn.onClick.AddListener(OnClickSword);
        }else{
            m_SwordBtn.gameObject.SetActive(false);
        }
        
        if( (int)MainGameManager.GetInstance().GetData<int>("Win12") == 1 ){
            MainGameManager.GetInstance().AddOnClickBaseAction(m_ReloadBtn,m_ReloadBtn.GetComponent<RectTransform>());
            m_ReloadBtn.onClick.AddListener(OnClickReload);
        }else{
            m_ReloadBtn.gameObject.SetActive(false);
        }

        MainGameManager.GetInstance().AddOnClickBaseAction(m_CloseBtn,m_CloseBtn.GetComponent<RectTransform>());
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
        StartCoroutine(AssistCoolDown(AssistType.FireBall));

        m_IsFireBallUsed = true;
        m_FireballBtn.GetComponent<Image>().color = Color.gray;
    }

    private IEnumerator AssistCoolDown(AssistType assistType){
        float passTime = 0;
        float duration = assistType==AssistType.Sword?10f: 45f;
        while (passTime<duration)
        {   
            yield return null;
            switch (assistType)
            {
                case AssistType.FireBall:
                    m_FireballImage.fillAmount = passTime/duration;
                break;
                case AssistType.Sword:
                    m_SwordImage.fillAmount = passTime/duration;
                break;
                case AssistType.Net:
                    m_NetImage.fillAmount = passTime/duration;
                break;
                case AssistType.Reload:
                    m_ReloadImage.fillAmount = passTime/duration;
                break;
                default:
                break;
            }
            passTime += Time.deltaTime;
        }

        switch (assistType)
        {
            case AssistType.FireBall:
                m_IsFireBallUsed = false;
            break;
            case AssistType.Sword:
                m_IsTimmySwordUsed = false;
            break;
            case AssistType.Net:
                m_IsNetUsed = false;
            break;
            case AssistType.Reload:
                m_IsReloadUsed = false;
            break;
            default:
            break;
        }
    }


    private void OnClickSword(){
        if(m_IsTimmySwordUsed)
            return;

        m_SlashController.Slash();
        StartCoroutine(AssistCoolDown(AssistType.Sword));
        m_IsTimmySwordUsed = true;
        m_SwordBtn.GetComponent<Image>().color = Color.gray;
    }


    private void OnClickNet(){
        if(m_IsNetUsed)
            return;

        m_AudioSource.clip = m_NetSoundClip;
        m_AudioSource.Play();
        m_KineticTransmitterController.init();
        StartCoroutine(AssistCoolDown(AssistType.Net));
        m_IsNetUsed = true;
        m_NetBtn.GetComponent<Image>().color = Color.gray;
        
    }

    private void OnClickReload(){
        if(m_IsReloadUsed)
            return;

        m_AudioSource.clip = m_ReloadSoundClip;
        m_AudioSource.Play();
        Reload();
        StartCoroutine(AssistCoolDown(AssistType.Reload));
        m_IsReloadUsed = true;
        m_ReloadBtn.GetComponent<Image>().color = Color.gray;
        
    }


    private void Reload(){
        // ability reload 
        BaseDefenceManager.GetInstance().SetFullAmmo();
    }

}
