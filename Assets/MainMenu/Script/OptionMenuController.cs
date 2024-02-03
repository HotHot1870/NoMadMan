using System;
using ExtendedButtons;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionMenuController : MonoBehaviour
{
    
    [SerializeField] private Button2D m_ResumeBtn;
    [SerializeField] private Button2D m_QuitGameBtn;

    [SerializeField] private Slider m_AimSensitivitySlider;
    [SerializeField] private Slider m_SoundVolumeSlider;
    [SerializeField] private GameObject m_OptionPanel;
    [SerializeField] private Animator m_BgAnimator;
    [Header("BGM")]
    [SerializeField] private Slider m_BGMVolumeSlider;
    [SerializeField] private AudioClip m_MainBGM;
    [SerializeField] private AudioClip m_MapBGM;
    [SerializeField] private AudioClip m_BattleBGM;

    void Start(){
        m_BgAnimator?.Play("Hidden");
        
        m_AimSensitivitySlider.normalizedValue = Mathf.InverseLerp(0.1f,1f, MainGameManager.GetInstance().GetAimSensitivity() );
        m_SoundVolumeSlider.normalizedValue = Mathf.InverseLerp(0f,1.25f, MainGameManager.GetInstance().GetVolume() );
        m_BGMVolumeSlider.normalizedValue = Mathf.InverseLerp(0f,1.25f, MainGameManager.GetInstance().GetBGMVolume() );


        m_AimSensitivitySlider.onValueChanged.AddListener((x)=>{
            MainGameManager.GetInstance().SetAimSensitivity( Mathf.Lerp(0.1f, 1f,m_AimSensitivitySlider.normalizedValue) );
        });

        m_SoundVolumeSlider.onValueChanged.AddListener((x)=>{
            MainGameManager.GetInstance().SetSoundVolume( Mathf.Lerp(0f, 1.25f,m_SoundVolumeSlider.normalizedValue) );
            MainGameManager.GetInstance().UpdateSoundVolume();
        });

        
        m_BGMVolumeSlider.value = Mathf.InverseLerp(0f,2f, MainGameManager.GetInstance().GetBGMVolume() );

        m_BGMVolumeSlider.onValueChanged.AddListener((x)=>{
            MainGameManager.GetInstance().SetBGMVolume( Mathf.Lerp(0f, 1.25f,m_BGMVolumeSlider.normalizedValue) );
            MainGameManager.GetInstance().UpdateBGMVolume();
        });
        
        m_ResumeBtn.onDown.AddListener(()=>{
            MainGameManager.GetInstance().OnClickStartSound();
        });
        m_ResumeBtn.onUp.AddListener(()=>{
            MainGameManager.GetInstance().OnClickEndSound();
        });
        
        m_ResumeBtn.onClick.AddListener(()=>{
            m_BgAnimator?.Play("Close");
        });

        
        m_QuitGameBtn.onDown.AddListener(()=>{
            MainGameManager.GetInstance().OnClickStartSound();
        });
        m_QuitGameBtn.onUp.AddListener(()=>{
            MainGameManager.GetInstance().OnClickEndSound();
        });
        
        m_QuitGameBtn.onClick.AddListener(()=>{
            switch (SceneManager.GetActiveScene().name)
            {
                case "BaseDefence":
                    MainGameManager.GetInstance().LoadSceneWithTransition("Map");
                return;
                case "Map":
                    MainGameManager.GetInstance().LoadSceneWithTransition("MainMenu");
                return;
                default:
                    m_BgAnimator?.Play("Hidden");
                break;
            }
        });

        
    }

    public void Init(Action onClickResume)
    {
        m_AimSensitivitySlider.normalizedValue = Mathf.InverseLerp(0.1f,2f, MainGameManager.GetInstance().GetAimSensitivity() );
        m_SoundVolumeSlider.normalizedValue = Mathf.InverseLerp(0f,2f, MainGameManager.GetInstance().GetVolume() );


        m_AimSensitivitySlider.onValueChanged.AddListener((x)=>{
            MainGameManager.GetInstance().SetAimSensitivity( Mathf.Lerp(0.1f, 1.25f,m_AimSensitivitySlider.normalizedValue) );
        });

        m_SoundVolumeSlider.onValueChanged.AddListener((x)=>{
            MainGameManager.GetInstance().SetSoundVolume( Mathf.Lerp(0f, 1.25f,m_SoundVolumeSlider.normalizedValue) );
            MainGameManager.GetInstance().UpdateSoundVolume();
        });        
        
        m_BGMVolumeSlider.normalizedValue = Mathf.InverseLerp(0f,1.25f, MainGameManager.GetInstance().GetBGMVolume() );


        m_BGMVolumeSlider.onValueChanged.AddListener((x)=>{
            MainGameManager.GetInstance().SetBGMVolume( Mathf.Lerp(0f, 1.25f,m_BGMVolumeSlider.normalizedValue) );
            MainGameManager.GetInstance().UpdateBGMVolume();
        });
        
        
        m_ResumeBtn.onClick.AddListener(()=>{
            m_BgAnimator?.Play("Close");
        });

        
        m_QuitGameBtn.onClick.AddListener(()=>{
            switch (SceneManager.GetActiveScene().name)
            {
                case "BaseDefence":
                    MainGameManager.GetInstance().PlayBGM(m_MapBGM);
                    MainGameManager.GetInstance().LoadSceneWithTransition("Map");
                return;
                case "Map":
                    MainGameManager.GetInstance().PlayBGM(m_MainBGM);
                    MainGameManager.GetInstance().LoadSceneWithTransition("MainMenu");
                return;
                default:
                    MainGameManager.GetInstance().PlayBGM(m_BattleBGM);
                    m_BgAnimator?.Play("Hidden");
                break;
            }
        });

        

        
        m_ResumeBtn.onClick.AddListener(()=>{
             onClickResume?.Invoke();
             if(onClickResume==null){
                m_BgAnimator?.Play("Close");
             }
        });
        m_BgAnimator?.Play("Hidden");
        
    }

    public void Open(){
        m_BgAnimator.Play("Open");
    }
}
