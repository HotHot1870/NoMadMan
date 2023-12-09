using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button m_StartGameBtn;
    [SerializeField] private Button m_OptionBtn;
    [SerializeField] private Button m_QuitGameBtn;

    [SerializeField] private GameObject m_OptionPanel;


    void Start()
    {
        m_StartGameBtn.onClick.AddListener(()=>{
            MainGameManager.GetInstance().SetMapScene();
        });
        
        m_OptionBtn.onClick.AddListener(()=>{
            var optionController = m_OptionPanel.GetComponent<OptionMenuController>();
            optionController.Open();
        });

        m_QuitGameBtn.onClick.AddListener(()=>{
            Application.Quit();
        });
    }

}
