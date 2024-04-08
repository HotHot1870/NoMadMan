
using System.Collections;
using ExtendedButtons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button2D m_StartGameBtn;
    [SerializeField] private Button2D m_OptionBtn;
    [SerializeField] private Button2D m_CreditBtn;
    [SerializeField] private Button2D m_QuitGameBtn;
    [SerializeField] private Button m_ClearDataBtn;
    [SerializeField] private Button m_GainGooBtn;
    [SerializeField] private Button m_UnlockAllLevelBtn;
    [SerializeField] private Button m_ADToggleBtn;
    [SerializeField] private TMP_Text m_ADToggleText;
    [SerializeField] private Image m_Black;
    [SerializeField] private AnimationCurve m_BlackFadeCurve;
    [SerializeField] private GameObject m_OptionPanel;
    [SerializeField] private CreditController m_CreditController;


    void Start()
    {
        StartCoroutine(BlackFadeOut());
        m_OptionPanel.GetComponent<OptionMenuController>().Init(null);
        //m_PlayerName.text = MainGameManager.GetInstance().GetData<string>("PlayerName", "").ToString().Trim();
        m_GainGooBtn.onClick.AddListener(()=>{
            float curGoo = PlayerPrefs.GetFloat("Goo", 0 );
            PlayerPrefs.SetFloat("Goo",curGoo+100000);
        });
        m_UnlockAllLevelBtn.onClick.AddListener(MainGameManager.GetInstance().UnlockAllLevel);

        string toggleText = (int)MainGameManager.GetInstance().GetData<int>("AD")==1?"ON":"OFF";
        m_ADToggleText.text = "AD " +toggleText;
        m_ADToggleBtn.onClick.AddListener(ToggleAd);

        m_ClearDataBtn.onClick.AddListener(()=>{
            PlayerPrefs.DeleteAll();
            
            var allWeapon = MainGameManager.GetInstance().GetAllWeapon();
            
            // unlock pistol
            MainGameManager.GetInstance().SaveData<int>("WeaponUnlock"+0.ToString(),1);
            // can use pistol by default , if no gun selected
            if((int)System.Convert.ToSingle(MainGameManager.GetInstance().GetData<int>("SelectedWeapon"+0.ToString(), "-1")) < 0)
                MainGameManager.GetInstance().SaveData<int>("SelectedWeapon"+0.ToString(),0);
        });


        MainGameManager.GetInstance().AddOnClickBaseAction(m_StartGameBtn,m_StartGameBtn.GetComponent<RectTransform>());
        m_StartGameBtn.onClick.AddListener(()=>{
            MainGameManager.GetInstance().SetMapScene();
        });
        

        MainGameManager.GetInstance().AddOnClickBaseAction(m_OptionBtn,m_OptionBtn.GetComponent<RectTransform>());
        m_OptionBtn.onClick.AddListener(()=>{
            var optionController = m_OptionPanel.GetComponent<OptionMenuController>();
            optionController.Open();
        });

        MainGameManager.GetInstance().AddOnClickBaseAction(m_QuitGameBtn,m_QuitGameBtn.GetComponent<RectTransform>());
        m_QuitGameBtn.onClick.AddListener(()=>{
            Application.Quit();
        });

        MainGameManager.GetInstance().AddOnClickBaseAction(m_CreditBtn,m_CreditBtn.GetComponent<RectTransform>());
        m_CreditBtn.onClick.AddListener(ShowCredit);
    }

    private void ToggleAd(){
        MainGameManager.GetInstance().SaveData<int>("AD",
        (int)MainGameManager.GetInstance().GetData<int>("AD")==1?0:1);

        string toggleText = (int)MainGameManager.GetInstance().GetData<int>("AD")==1?"ON":"OFF";
        m_ADToggleText.text = "AD " +toggleText;
    }

    private void ShowCredit(){
        m_CreditController.Init(true);
    }

    private IEnumerator BlackFadeOut(){
        float passTime = 0;
        float duration = 1.5f;
        while (passTime<duration)
        {
            yield return null;
            passTime += Time.deltaTime;
            m_Black.color = Color.Lerp(Color.black,Color.clear, m_BlackFadeCurve.Evaluate(passTime/duration));
        }
        Destroy(m_Black.gameObject);
    }

}
