using System.Collections;
using System.Collections.Generic;
using ExtendedButtons;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseDefenceUIController : MonoBehaviour
{
    [Header("ShootingPanel")]
    [SerializeField] private GameObject m_ShootPanel;
    [SerializeField] private TMP_Text m_AmmoText;
    [SerializeField] private Button m_ReloadBtn;
    [SerializeField] private Button m_OptionBtn;
    [SerializeField] private Button2D m_AimBtn;    
    [SerializeField] private Button2D m_ShootBtn;
    [SerializeField] private Button2D m_LookDownBtn;
    [SerializeField] private Button2D m_LookUpBtn;

    
    [Header("OptionPanel")]
    [SerializeField] private GameObject m_OptionPanel;

    [Header("Wall")]
    [SerializeField] private GameObject m_WallParent;
    [SerializeField] private Image m_HpBarFiller;
    [SerializeField] private TMP_Text m_WallHpText;

    [Header("Result")]
    [SerializeField] private GameObject m_ResultParent;
    [SerializeField] private TMP_Text m_ResultTitle;
    [SerializeField] private Button2D m_BackFromResultBtn;
    

    [Header("Switch weapon")]
    [SerializeField] private Animator m_SwitchWeaponAnimator;

    [Header("Goo")]
    [SerializeField] private TMP_Text m_GooText;




    private void Start() {
        SetGooText();
        m_ResultParent.SetActive(false);
        m_BackFromResultBtn.onClick.AddListener(OnClickBackFromResult);

        var crosshairController = BaseDefenceManager.GetInstance().GetCrosshairController();

        m_AimBtn.onDown.AddListener(crosshairController.OnAimBtnDown);
        m_AimBtn.onUp.AddListener(crosshairController.OnAimBtnUp);
        m_AimBtn.onExit.AddListener(crosshairController.OnAimBtnUp);

        var gunShootController = BaseDefenceManager.GetInstance().GetGunShootController();

        m_ReloadBtn.onClick.AddListener(gunShootController.OnClickReload);

        m_ShootBtn.onDown.AddListener(gunShootController.OnShootBtnDown);
        m_ShootBtn.onUp.AddListener(gunShootController.OnShootBtnUp);
        m_ShootBtn.onExit.AddListener(gunShootController.OnShootBtnUp);
        
        m_OptionBtn.onClick.AddListener(()=>{
            m_OptionPanel.SetActive(true);
        });

        var cameraController = BaseDefenceManager.GetInstance().GetCameraController();

        m_LookDownBtn.onDown.AddListener(OnClickShowSwitchWeaponPanel);

        m_LookUpBtn.onDown.AddListener(OnClickCloseSwitchWeaponPanel);
    }

    public void SetGooText(){
        m_GooText.text = "Goo : "+MainGameManager.GetInstance().GetGooAmount();
    }

    public void OnClickCloseSwitchWeaponPanel(){
        m_SwitchWeaponAnimator.Play("Down");
        BaseDefenceManager.GetInstance().ChangeGameStage(BaseDefenceNameSpace.BaseDefenceStage.Shoot);
        m_ShootPanel.SetActive(true);
    }

    private void OnClickShowSwitchWeaponPanel(){
        m_SwitchWeaponAnimator.Play("Up");
        BaseDefenceManager.GetInstance().ChangeGameStage(BaseDefenceNameSpace.BaseDefenceStage.SwitchWeapon);
        m_ShootPanel.SetActive(false);
    }

    public void SetAmmoText(string text){
        m_AmmoText.text = text;
    }

    public void SetWallHpUI(){
        float wallHp = MainGameManager.GetInstance().GetWallCurHp();
        float wallMapHp = MainGameManager.GetInstance().GetWallMaxHp();
        WallUISetActive(true);
        m_WallHpText.text = $"{wallHp} / {wallMapHp}";
        m_HpBarFiller.fillAmount = wallHp/wallMapHp;
    }

    public void WallUISetActive(bool isActive){
        m_WallParent.SetActive(isActive);
    }

    public void OnClickBackFromResult(){
        SceneManager.LoadScene("Map");
    }

    public void SetResultPanel(bool isWin){
        m_ResultParent.SetActive(true);
        m_ResultTitle.text = isWin?"You Win":"Loser";
    }
}
