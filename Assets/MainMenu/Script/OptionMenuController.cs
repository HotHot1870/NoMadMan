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

    void Start()
    {
        m_AimSensitivitySlider.normalizedValue = Mathf.InverseLerp(0.1f,1.5f, MainGameManager.GetInstance().GetAimSensitivity() );
        m_VolumeSlider.normalizedValue = Mathf.InverseLerp(0.1f,1.5f, MainGameManager.GetInstance().GetVolume() );


        m_AimSensitivitySlider.onValueChanged.AddListener((x)=>{
            MainGameManager.GetInstance().SetAimSensitivity( Mathf.Lerp(0.1f, 4f,m_AimSensitivitySlider.normalizedValue) );
        });

        m_VolumeSlider.onValueChanged.AddListener((x)=>{
            MainGameManager.GetInstance().SetVolume( Mathf.Lerp(0.1f, 1.5f,m_VolumeSlider.normalizedValue) );
            MainGameManager.GetInstance().UpdateVolume();
        });
        
        
        m_ResumeBtn.onClick.AddListener(()=>{
            m_OptionPanel.SetActive(false);
        });

        
        m_QuitGameBtn.onClick.AddListener(()=>{
            Application.Quit();
        });

        
        m_OptionPanel.SetActive(false);
    }
}
