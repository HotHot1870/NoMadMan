using System.Collections;
using ExtendedButtons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkShopChooseWeaponController : MonoBehaviour
{
    [SerializeField] private Button2D m_BackBtn; 
    [SerializeField] private MapUpgradeWeaponPanel m_MapUpgradeWeaponPanel; 
    [SerializeField] private Button2D m_UpgradeBtn; 
    [SerializeField] private TMP_Text m_GooText;

    [Header("Unlock")]
    [SerializeField] private Button2D m_UnlockBtn; 
    [SerializeField] private Image m_UnlockFill;
    [SerializeField] private TMP_Text m_SelectedWeaponUnlockCost;
    private bool m_IsEnoughGoo = true;
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
    [SerializeField] private TMP_Text m_SelectedWeaponExplodeRadius;
    [SerializeField] private TMP_Text m_SelectedWeaponSemiAuto;
    [SerializeField] private TMP_Text m_SelectedWeaponPuncture;


    void Start(){
        
        m_GooText.text = "Goo : "+MainGameManager.GetInstance().GetGooAmount();


        m_BackBtn.onDown.AddListener(()=>{
            MainGameManager.GetInstance().OnClickStartSound();
        });
        m_BackBtn.onUp.AddListener(()=>{
            MainGameManager.GetInstance().OnClickEndSound();
        });
        m_BackBtn.onClick.AddListener(()=>{
            this.gameObject.SetActive(false);
            });


        
        m_UpgradeBtn.onDown.AddListener(()=>{
            MainGameManager.GetInstance().OnClickStartSound();
        });
        m_UpgradeBtn.onUp.AddListener(()=>{
            MainGameManager.GetInstance().OnClickEndSound();
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
    private void SetWeaponlistSelectedWeaponData(GunScriptable gunScriptable){
        if(m_Unfilling != null)
            StopCoroutine( m_Unfilling);

        if(m_Filling != null)
            StopCoroutine( m_Filling);
            
        m_UnlockFill.fillAmount = 0;


        m_SelectedGun = gunScriptable;
        
        m_SelectedWeaponId = m_SelectedGun.Id;
        m_SelectedWeaponImage.sprite = m_SelectedGun.DisplayImage;
        m_SelectedWeaponName.text = "Name : "+m_SelectedGun.DisplayName;
        m_SelectedWeaponFireRate.text = "Fire Rate : "+ System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.FireRate));
        m_SelectedWeaponDamage.text = "Damage : "+ System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.Damage))+" x "+ 
            System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.Pellet));
        m_SelectedWeaponClipSize.text = "Clip Size : "+ System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.ClipSize));
        m_SelectedWeaponAcc.text = "Acc : "+ System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.Accuracy));
        m_SelectedWeaponRecoil.text = "Recoil : "+ System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.Recoil));
        m_SelectedWeaponHandling.text = "Handling : "+ System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.Handling));
        m_SelectedWeaponExplodeRadius.text = "Explode Radius : "+ System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.ExplodeRadius));

        float unlockCost = m_SelectedGun.UnlockCost;
        float gooOwnedAmount = MainGameManager.GetInstance().GetGooAmount();
        m_SelectedWeaponUnlockCost.text = "Unlock : "+unlockCost.ToString()+" / "+gooOwnedAmount.ToString();
        m_IsEnoughGoo = true;
        if(unlockCost>gooOwnedAmount){
            // not enough goo too unlock
            m_IsEnoughGoo = false;
        }
        m_SelectedWeaponUnlockCost.color = unlockCost>gooOwnedAmount?Color.red:Color.white;
        
        //m_SelectedWeaponExplodeRadius.gameObject.SetActive(m_SelectedGun.ExplodeRadius>0);

        m_SelectedWeaponSemiAuto.text = "Semi_Auto : "+ m_SelectedGun.GetStatValue(GunScriptableStatEnum.IsSemiAuto);
        //m_SelectedWeaponSemiAuto.gameObject.SetActive(m_SelectedGun.GunStats.IsSemiAuto);

        string punctureText = (m_SelectedGun.GunStats.BulletType == BulletType.Puncture).ToString();
        m_SelectedWeaponPuncture.text = "Puncture : "+punctureText;
        //m_SelectedWeaponPuncture.gameObject.SetActive(m_SelectedGun.GunStats.BulletType == BulletType.Puncture);


        string gunUnlockKey = m_SelectedGun.DisplayName+m_SelectedGun.Id;

        bool isOwned = (int)System.Convert.ToSingle( MainGameManager.GetInstance().GetData<int>(gunUnlockKey)) == 1;
        m_SelectedWeaponUnlockCost.gameObject.SetActive(!isOwned);
        m_UnlockBtn.gameObject.SetActive(!isOwned);
        m_UpgradeBtn.gameObject.SetActive(isOwned);
    }


    public void Init(){
        m_UnlockFill.fillAmount = 0;
        m_GooText.text = "Goo : "+MainGameManager.GetInstance().GetGooAmount();
        
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
            
            string gunUnlockKey = gun.DisplayName+gun.Id.ToString();

            bool isOwned = (int)System.Convert.ToSingle( MainGameManager.GetInstance().GetData<int>(gunUnlockKey)) == 1;
            var config = new WeaponUpgradeGridConfig{
                isLock = !isOwned,
                gunScriptsble = gun,
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
        MainGameManager.GetInstance().OnClickStartSound();
        if(m_Unfilling != null)
            StopCoroutine( m_Unfilling);

        if(!m_IsEnoughGoo){
            // not enough goo
            return;
        }
        m_Filling = StartCoroutine(FillUnlockImage());
    }

    private void OnLetGoUnlockBtn(){
        MainGameManager.GetInstance().OnClickEndSound();
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
        SetWeaponlistSelectedWeaponData( m_SelectedGun);
        
        if(m_Filling != null){
            StopCoroutine( m_Filling);
            m_Filling = null;
        }
    }
/*
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

    }*/



}
