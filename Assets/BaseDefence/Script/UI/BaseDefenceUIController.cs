using System.Collections;
using System.Collections.Generic;
using ExtendedButtons;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseDefenceUIController : MonoBehaviour
{
    private bool m_IsWin = false;
    
    [Header("BG")]
    [SerializeField] private Animator m_BGAnimator;

    [Header("ShootingPanel")]
    [SerializeField] private GameObject m_ShootPanel;
    [SerializeField] private TMP_Text m_AmmoText;
    [SerializeField] private Button m_ReloadBtn;
    [SerializeField] private Button m_OptionBtn;
    [SerializeField] private Button2D m_AimBtn;    
    [SerializeField] private Button2D m_ShootBtn;
    [SerializeField] private Button2D m_SwitchWeaponBtn;
    [SerializeField] private Button2D m_AssistBtn;
    [SerializeField] private AssistPanelController m_AssistPanel;
    [SerializeField] private Button2D m_CloseSwitchWeaponBtn;

    
    [Header("OptionPanel")]
    [SerializeField] private GameObject m_OptionPanel;

    [Header("Hp")]
    [SerializeField] private GameObject m_HpParent;
    [SerializeField] private Image m_HpBarFiller;
    [SerializeField] private TMP_Text m_HpText;

    [Header("Result")]
    [SerializeField] private GameObject m_ResultParent;
    [SerializeField] private TMP_Text m_ResultTitle;
    [SerializeField] private Button2D m_BackFromResultBtn;
    
    [Header("Reload")]
    [SerializeField] private GameObject m_ReloadPanel;

    [Header("Switch weapon")]
    [SerializeField] private Animator m_SwitchWeaponAnimator;

    [Header("Dmagae text")]
    [SerializeField] private GameObject m_DamageText;
    [SerializeField] private Transform m_DamageTextParent;




    private void Start() {
        m_BGAnimator.Play("Hidden");
        m_ResultParent.SetActive(false);
        m_BackFromResultBtn.onClick.AddListener(OnClickBackFromResult);

        var crosshairController = BaseDefenceManager.GetInstance().GetCrosshairController();

        m_AimBtn.onDown.AddListener(crosshairController.OnAimBtnDown);
        m_AimBtn.onUp.AddListener(crosshairController.OnAimBtnUp);
        m_AimBtn.onExit.AddListener(crosshairController.OnAimBtnUp);

        var gunShootController = BaseDefenceManager.GetInstance().GetGunShootController();

        m_ReloadBtn.onClick.AddListener(gunShootController.OnClickReload);

        m_ShootBtn.onDown.AddListener(()=>{
            
        var shootBtnRect = m_ShootBtn.GetComponent<RectTransform>();
        var shootBtnBGRect = m_ShootBtn.transform.parent.GetComponent<RectTransform>();
#if !UNITY_EDITOR

        var touchIndex = Input.touchCount-1;
        if(touchIndex<0){
            // no touch detected
            return;
        } 
        if(Vector2.Distance(Input.GetTouch(touchIndex).position, shootBtnRect.position)< shootBtnBGRect.sizeDelta.x/1.75f ){
            gunShootController.OnShootBtnDown();
        }else{
            gunShootController.SetGunIdle();
        }    
#endif
#if UNITY_EDITOR
        if(Vector2.Distance(Input.mousePosition , shootBtnRect.position)< shootBtnBGRect.sizeDelta.x/2f ){
            gunShootController.OnShootBtnDown();
        }else{
            gunShootController.SetGunIdle();
        }      
#endif
            
        });
        m_ShootBtn.onUp.AddListener(gunShootController.OnShootBtnUp);
        m_ShootBtn.onExit.AddListener(gunShootController.OnShootBtnUp);
        
        m_OptionBtn.onClick.AddListener(()=>{
            TurnOffAllPanel();
            var optionController = m_OptionPanel.GetComponent<OptionMenuController>();
            optionController.Init(TurnOnAllPanel);
            optionController.Open();

        });

        var cameraController = BaseDefenceManager.GetInstance().GetCameraController();

        m_SwitchWeaponBtn.onDown.AddListener(OnClickShowSwitchWeaponPanel);
        m_AssistBtn.onDown.AddListener(ShowAssistPanel);

        m_CloseSwitchWeaponBtn.onDown.AddListener(OnClickCloseSwitchWeaponPanel);
    }


    private void ShowAssistPanel(){
        m_AssistPanel.Init();
    }

    private IEnumerable DelayAction(System.Action action, float delayDuration){
        float passTime = 0;
        while (passTime<delayDuration)
        {
            passTime += Time.deltaTime;
            yield return null;
        }
        action?.Invoke();
    }

    public void ShowDamageText(float damage, Color color,Vector2 pos){
        var newDamageText = Instantiate(m_DamageText,m_DamageTextParent);
        var TMP = newDamageText.GetComponent<TMP_Text>();
        TMP.text = System.String.Format("{0:0.#}", damage);
        TMP.color = color;
        newDamageText.GetComponent<RectTransform>().anchoredPosition = pos;
    }

    private void TurnOffAllPanel(){
        m_ShootPanel.SetActive(false);
        m_ReloadPanel.SetActive(false);
    }
    
    private void TurnOnAllPanel(){
        m_ShootPanel.SetActive(true);
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

    public void SetHpUI(){
        float Hp = BaseDefenceManager.GetInstance().GetCurHp();
        float MapHp = BaseDefenceManager.GetInstance().GetMaxHp();
        UISetActive(true);
        m_HpText.text = $"{Hp.ToString("0.#")} / {MapHp}";
        m_HpBarFiller.fillAmount = Hp/MapHp;
    }

    public void UISetActive(bool isActive){
        m_HpParent.SetActive(isActive);
    }

    public void OnClickBackFromResult(){
        MainGameManager.GetInstance().LoadSceneWithTransition("Map",ShowDialogOnEndGame);
    }
    private void ShowDialogOnEndGame(){
        if(m_IsWin)
            MapManager.GetInstance().ShowEndDefenceDialog();
    }


    public void SetResultPanel(bool isWin){
        TurnOffAllPanel();
        m_ResultParent.SetActive(true);
        m_BGAnimator.Play("Open");
        m_IsWin = isWin;
        m_ResultTitle.text = isWin?"You Win":"Loser";

        if(isWin){
            var locationData = BaseDefenceManager.GetInstance().GetLocationScriptable();
            MainGameManager.GetInstance().SaveData<int>(locationData.DisplayName+locationData.Id, 1);
        }
    }
}
