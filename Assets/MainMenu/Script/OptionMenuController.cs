using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionMenuController : MonoBehaviour
{
    
    [SerializeField] private Button m_ResumeBtn;
    [SerializeField] private Button m_QuitGameBtn;

    [SerializeField] private Slider m_AimSensitivitySlider;
    [SerializeField] private Slider m_VolumeSlider;
    [SerializeField] private GameObject m_OptionPanel;
    [SerializeField] private Animator m_BgAnimator;

    void Start(){
        m_BgAnimator?.Play("Hidden");
        
        m_AimSensitivitySlider.normalizedValue = Mathf.InverseLerp(0.1f,1f, MainGameManager.GetInstance().GetAimSensitivity() );
        m_VolumeSlider.normalizedValue = Mathf.InverseLerp(0f,1f, MainGameManager.GetInstance().GetVolume() );


        m_AimSensitivitySlider.onValueChanged.AddListener((x)=>{
            MainGameManager.GetInstance().SetAimSensitivity( Mathf.Lerp(0.1f, 1f,m_AimSensitivitySlider.normalizedValue) );
        });

        m_VolumeSlider.onValueChanged.AddListener((x)=>{
            MainGameManager.GetInstance().SetVolume( Mathf.Lerp(0f, 1f,m_VolumeSlider.normalizedValue) );
            MainGameManager.GetInstance().UpdateVolume();
        });
        
        
        m_ResumeBtn.onClick.AddListener(()=>{
            m_BgAnimator?.Play("Close");
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
        m_AimSensitivitySlider.normalizedValue = Mathf.InverseLerp(0.1f,1f, MainGameManager.GetInstance().GetAimSensitivity() );
        m_VolumeSlider.normalizedValue = Mathf.InverseLerp(0f,1f, MainGameManager.GetInstance().GetVolume() );


        m_AimSensitivitySlider.onValueChanged.AddListener((x)=>{
            MainGameManager.GetInstance().SetAimSensitivity( Mathf.Lerp(0.1f, 1f,m_AimSensitivitySlider.normalizedValue) );
        });

        m_VolumeSlider.onValueChanged.AddListener((x)=>{
            MainGameManager.GetInstance().SetVolume( Mathf.Lerp(0f, 1f,m_VolumeSlider.normalizedValue) );
            MainGameManager.GetInstance().UpdateVolume();
        });
        
        
        m_ResumeBtn.onClick.AddListener(()=>{
            m_BgAnimator?.Play("Close");
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

        

        
        m_ResumeBtn.onClick.AddListener(()=>{
             onClickResume?.Invoke();
        });
        m_BgAnimator?.Play("Hidden");
        
    }

    public void Open(){
        m_BgAnimator.Play("Open");
    }
}
