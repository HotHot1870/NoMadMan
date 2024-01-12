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
    [SerializeField] private Button m_ClearDataBtn;
    [SerializeField] private Button m_GainGooBtn;
    [SerializeField] private Button m_unlockAllLevelBtn;

    [SerializeField] private GameObject m_OptionPanel;


    void Start()
    {
        m_GainGooBtn.onClick.AddListener(()=>{
            float curGoo = PlayerPrefs.GetFloat("Goo", 0 );
            PlayerPrefs.SetFloat("Goo",curGoo+10000);
        });
        m_unlockAllLevelBtn.onClick.AddListener(MainGameManager.GetInstance().UnlockAllLevel);


        m_ClearDataBtn.onClick.AddListener(()=>{
            PlayerPrefs.DeleteAll();
            
            var allWeapon = MainGameManager.GetInstance().GetAllWeapon();
            
            // unlock pistol
            MainGameManager.GetInstance().SaveData<int>(allWeapon.Find(x=>x.Id ==0).DisplayName+0.ToString(),1);
            // can use pistol by default , if no gun selected
            if((int)System.Convert.ToSingle(MainGameManager.GetInstance().GetData<int>("SelectedWeapon"+0.ToString(), "-1")) < 0)
                MainGameManager.GetInstance().SaveData<int>("SelectedWeapon"+0.ToString(),0);
        });

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
