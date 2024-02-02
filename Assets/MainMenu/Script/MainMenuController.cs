
using ExtendedButtons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button2D m_StartGameBtn;
    [SerializeField] private Button2D m_OptionBtn;
    [SerializeField] private Button2D m_QuitGameBtn;
    [SerializeField] private Button m_ClearDataBtn;
    [SerializeField] private Button m_GainGooBtn;
    [SerializeField] private Button m_unlockAllLevelBtn;
    [SerializeField] private GameObject m_OptionPanel;
    [SerializeField] private TMP_Text m_PlayerName;


    void Start()
    {
        m_OptionPanel.GetComponent<OptionMenuController>().Init(null);
        m_PlayerName.text = MainGameManager.GetInstance().GetData<string>("PlayerName", "").ToString().Trim();
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


        m_StartGameBtn.onDown.AddListener(()=>{
            MainGameManager.GetInstance().OnClickStartSound();
        });
        m_StartGameBtn.onUp.AddListener(()=>{
            MainGameManager.GetInstance().OnClickEndSound();
        });
        m_StartGameBtn.onClick.AddListener(()=>{
            MainGameManager.GetInstance().SetMapScene();
        });
        

        
        m_OptionBtn.onDown.AddListener(()=>{
            MainGameManager.GetInstance().OnClickStartSound();
        });
        m_OptionBtn.onUp.AddListener(()=>{
            MainGameManager.GetInstance().OnClickEndSound();
        });
        m_OptionBtn.onClick.AddListener(()=>{
            var optionController = m_OptionPanel.GetComponent<OptionMenuController>();
            optionController.Open();
        });

        m_QuitGameBtn.onDown.AddListener(()=>{
            MainGameManager.GetInstance().OnClickStartSound();
        });
        m_QuitGameBtn.onUp.AddListener(()=>{
            MainGameManager.GetInstance().OnClickEndSound();
        });
        m_QuitGameBtn.onClick.AddListener(()=>{
            Application.Quit();
        });
    }

}
