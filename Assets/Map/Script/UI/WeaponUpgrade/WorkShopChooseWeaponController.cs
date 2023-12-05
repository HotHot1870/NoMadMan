using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using ExtendedButtons;
using MainGameNameSpace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkShopChooseWeaponController : MonoBehaviour
{
    [SerializeField] private Button m_BackBtn; 
    [SerializeField] private MapUpgradeWeaponPanel m_MapUpgradeWeaponPanel; 
    [SerializeField] private Button m_UpgradeBtn; 

    [Header("Unlock")]
    [SerializeField] private Button2D m_UnlockBtn; 
    [SerializeField] private Image m_UnlockFill;
    private Coroutine m_Filling;
    private Coroutine m_Unfilling;
    private int m_SelectedWeaponId;
    private GunScriptable m_SelectedGun = null;

    [Header("WeaponList")]
    [SerializeField] private GameObject m_WeaponListGrid;
    [SerializeField] private Transform m_WeaponListContent;

    [Header("WeaponDetail")]
    [SerializeField] private Image m_SelectedWeaponImage;
    [SerializeField] private TMP_Text m_SelectedWeaponName;
    [SerializeField] private TMP_Text m_SelectedWeaponDamage;
    [SerializeField] private TMP_Text m_SelectedWeaponClipSize;
    [SerializeField] private TMP_Text m_SelectedWeaponAcc;
    [SerializeField] private TMP_Text m_SelectedWeaponRecoil;
    [SerializeField] private TMP_Text m_SelectedWeaponHandling;
    [SerializeField] private TMP_Text m_SelectedWeaponFireRate;
    [SerializeField] private TMP_Text m_SelectedWeaponUnlockCost;
    [SerializeField] private TMP_Text m_SelectedWeaponExplodeRadius;
    [SerializeField] private TMP_Text m_SelectedWeaponSemiAuto;
    [SerializeField] private TMP_Text m_SelectedWeaponPuncture;


    void Start(){
        m_BackBtn.onClick.AddListener(()=>{
            this.gameObject.SetActive(false);
            });
        m_UpgradeBtn.onClick.AddListener(()=>{
            m_MapUpgradeWeaponPanel.gameObject.SetActive(true);
            m_MapUpgradeWeaponPanel.Init(m_SelectedGun);
        });
        m_UnlockBtn.onDown.AddListener(OnDownUnlockBtn);
        m_UnlockBtn.onUp.AddListener(OnLetGoUnlockBtn);
        m_UnlockBtn.onExit.AddListener(OnLetGoUnlockBtn);
        this.gameObject.SetActive(false);
        m_UpgradeBtn.gameObject.SetActive(false);
    }
    private void SetWeaponlistSelectedWeaponData(WeaponOwnership selectedGunOwnership){
        if(m_Unfilling != null)
            StopCoroutine( m_Unfilling);

        if(m_Filling != null)
            StopCoroutine( m_Filling);
            
        m_UnlockFill.fillAmount = 0;


        m_SelectedGun = selectedGunOwnership.Gun;
        
        m_SelectedWeaponId = m_SelectedGun.Id;
        m_SelectedWeaponImage.sprite = m_SelectedGun.DisplayImage;
        m_SelectedWeaponName.text = "Name : "+m_SelectedGun.DisplayName;
        m_SelectedWeaponFireRate.text = "Fire Rate : "+m_SelectedGun.GetStatValue("FireRate");
        m_SelectedWeaponDamage.text = "Damage : "+m_SelectedGun.GetStatValue("Damage")+" x "+m_SelectedGun.GetStatValue("Pellet");
        m_SelectedWeaponClipSize.text = "Clip Size : "+m_SelectedGun.GetStatValue("ClipSize");
        m_SelectedWeaponAcc.text = "Acc : "+m_SelectedGun.GetStatValue("Accuracy");
        m_SelectedWeaponRecoil.text = "Recoil : "+m_SelectedGun.GetStatValue("Recoil");
        m_SelectedWeaponHandling.text = "Handling : "+m_SelectedGun.GetStatValue("Handling");
        m_SelectedWeaponUnlockCost.text = "Unlock : "+m_SelectedGun.UnlockCost.ToString()+" / "+MainGameManager.GetInstance().GetGooAmount().ToString();
        m_SelectedWeaponExplodeRadius.text = "Explode Radius : "+m_SelectedGun.GetStatValue("ExplodeRadius");
        //m_SelectedWeaponExplodeRadius.gameObject.SetActive(m_SelectedGun.ExplodeRadius>0);

        m_SelectedWeaponSemiAuto.text = "Semi_Auto : "+m_SelectedGun.GunStats.IsSemiAuto.ToString();
        //m_SelectedWeaponSemiAuto.gameObject.SetActive(m_SelectedGun.GunStats.IsSemiAuto);

        string punctureText = (m_SelectedGun.GunStats.BulletType == BulletType.Puncture).ToString();
        m_SelectedWeaponPuncture.text = "Puncture : "+punctureText;
        //m_SelectedWeaponPuncture.gameObject.SetActive(m_SelectedGun.GunStats.BulletType == BulletType.Puncture);


        m_SelectedWeaponUnlockCost.gameObject.SetActive(!selectedGunOwnership.IsOwned);
        m_UnlockBtn.gameObject.SetActive(!selectedGunOwnership.IsOwned);
        m_UpgradeBtn.gameObject.SetActive(selectedGunOwnership.IsOwned);
    }


    public void Init(){
        m_UnlockFill.fillAmount = 0;
        
        var allWeapon = MainGameManager.GetInstance().GetAllWeapon();
        
        // clear all spawn slot
        for (int i = 0; i < m_WeaponListContent.childCount; i++)
        {
            Destroy(m_WeaponListContent.GetChild(i).gameObject);
        }

        for (int i = 0; i < allWeapon.Count; i++)
        {
            int index = i;
            var newWeaponListSlot = Instantiate(m_WeaponListGrid, m_WeaponListContent);
            var gun = allWeapon[index];
            var config = new WeaponUpgradeGridConfig{
                isLock = !allWeapon[index].IsOwned,
                gunOwnership = gun,
                onClickAction = SetWeaponlistSelectedWeaponData
            };
            newWeaponListSlot.GetComponent<MapWeaponUpgradeGridController>().Init(config);
            if(m_SelectedGun == null){
                SetWeaponlistSelectedWeaponData(allWeapon[index]);
            }
        }
    }

    private void OnClickUpgrade(){
        // show upgrade panel

    }

    private void OnDownUnlockBtn(){
        if(m_Unfilling != null)
            StopCoroutine( m_Unfilling);

        m_Filling = StartCoroutine(FillUnlockImage());
    }

    private void OnLetGoUnlockBtn(){
        if(m_Filling != null)
            StopCoroutine( m_Filling);
            
        m_UnlockFill.fillAmount = 0;

        //m_Unfilling = StartCoroutine(UnfillUnlockImage());

    }

    private IEnumerator FillUnlockImage(){
        float duration = 1;
        float timePass = Mathf.Max(0.15f,m_UnlockFill.fillAmount) * duration;
        while (timePass<duration && m_UnlockFill.fillAmount <1)
        {
            timePass+=Time.deltaTime;
            m_UnlockFill.fillAmount = timePass/duration;
            yield return null;
            
        }
        m_UnlockFill.fillAmount = 1;
        // Unlock weapon
        MainGameManager.GetInstance().ChangeGooAmount(-1f*m_SelectedGun.UnlockCost);
        MainGameManager.GetInstance().UnlockGun(m_SelectedGun.Id);
        Init();
        var weaponOwnership = new WeaponOwnership{
            Gun = m_SelectedGun,
            IsOwned = true

        };
        SetWeaponlistSelectedWeaponData(weaponOwnership);
        
        if(m_Filling != null){
            StopCoroutine( m_Filling);
            m_Filling = null;
        }
    }

    private IEnumerator UnfillUnlockImage(){
        float duration = 1;
        float timePass = m_UnlockFill.fillAmount * duration;
        while (timePass<duration && m_UnlockFill.fillAmount >0)
        {
            timePass-=Time.deltaTime;
            m_UnlockFill.fillAmount = timePass/duration;
            yield return null;
            
        }
        m_UnlockFill.fillAmount = 0;
        if(m_Unfilling != null){
            StopCoroutine( m_Unfilling);
            m_Unfilling = null;
        }

    }



}
