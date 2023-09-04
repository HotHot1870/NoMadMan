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
            SceneManager.LoadScene("BaseDefense");
        });
        
        m_OptionBtn.onClick.AddListener(()=>{
            m_OptionPanel.SetActive(true);
        });

        m_QuitGameBtn.onClick.AddListener(()=>{
            Application.Quit();
        });
    }

}
