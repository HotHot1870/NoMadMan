using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExtendedButtons;
using TMPro;

public class MapUIController : MonoBehaviour
{
    [SerializeField] private Button2D m_MapDragBtn;  
    [SerializeField] private Button m_CheckLocationBtn;

    [Header("Option")]
    [SerializeField] private Button m_OptionBtn;
    [SerializeField] private GameObject m_OptionPanel;

    [Header("Location")]
    [SerializeField] private GameObject m_LocationDetailPanel; 
    [SerializeField] private TMP_Text m_LocationName;  
    //[SerializeField] private TMP_Text m_FortifyCost;  
    //[SerializeField] private TMP_Text m_FortifyRewardText;  
    //[SerializeField] private Image m_RewardImage;
    [SerializeField] private TMP_Text m_EnemyText;  
    [SerializeField] private Transform m_EnemyList; 
    [SerializeField] private Button m_FortifyBtn;  
    [SerializeField] private Button m_DefenceBtn;
    [SerializeField] private Button m_CancelLocationDetailBtn;


    [Header("WeaponUpgradeSelect")]
    [SerializeField] private WorkShopChooseWeaponController m_WorkShopChooseWeaponController;
    [SerializeField] private Button m_WorkShopBtn;
    

    /*
    [SerializeField] private GameObject m_FortifyPanel; 
    [SerializeField] private TMP_Text m_FortifyCostInDetail; 
    [SerializeField] private TMP_Text m_RewardName;
    [SerializeField] private Image m_RewardImageInDetail;
    [SerializeField] private Button m_YesFortifyBtn;  
    [SerializeField] private Button m_NoFortifyBtn; */ 

    [Header("ChooseWeapon")]
    [SerializeField] private GameObject m_ChooseWeaponPanel;
    [SerializeField] private List<MapChooseWeaponSlot> m_ChooseWeaponBtns = new List<MapChooseWeaponSlot>();
    [SerializeField] private Transform m_ChooseWeaponEnemyList;
    [SerializeField] private Button m_StartDefenceBtn;  
    [SerializeField] private Button m_CancelChooseWeaponBtn;  

    [Header("Weaponlist")]
    [SerializeField] private GameObject m_WeaponListPanel;
    [SerializeField] private Image m_SelectedSlotWeapon;

    [SerializeField] private TMP_Text m_SelectedWeaponName;
    [SerializeField] private TMP_Text m_SelectedWeaponDamage;
    [SerializeField] private TMP_Text m_SelectedWeaponClipSize;
    [SerializeField] private TMP_Text m_SelectedWeaponAcc;
    [SerializeField] private TMP_Text m_SelectedWeaponRecoil;
    [SerializeField] private TMP_Text m_SelectedWeaponHandling;
    [SerializeField] private TMP_Text m_SelectedWeaponFireRate;


    [SerializeField] private Transform m_WeaponListSlotParent;
    [SerializeField] private GameObject m_WeaponListSlotPrefab;
    [SerializeField] private Button m_ComfirmWeaponChangeBtn;  

    [Header("Goo")]
    [SerializeField] private TMP_Text m_GooText;




    private void Start() {
        m_GooText.text = "Goo : "+MainGameManager.GetInstance().GetGooAmount();
        m_CheckLocationBtn.gameObject.SetActive(false);

        // TODO : Split all ui panel
        TurnOffAllPanel();
        m_WorkShopChooseWeaponController.gameObject.SetActive(true);



        var freeCameraController = MapManager.GetInstance().GetMapFreeCameraController();
        m_MapDragBtn.onDown.AddListener(freeCameraController.OnDragMapBtnDown);
        m_MapDragBtn.onUp.AddListener(freeCameraController.OnDragMapBtnUp);
        m_MapDragBtn.onExit.AddListener(freeCameraController.OnDragMapBtnUp);

        m_CheckLocationBtn.onClick.AddListener(ShowLocationDetail);

        // location detail
        //m_FortifyBtn.onClick.AddListener(OnClickFortify);
        m_DefenceBtn.onClick.AddListener(OnClickDefence);
        m_CancelLocationDetailBtn.onClick.AddListener(CancelLocationDetail);

        // WorkShop
        m_WorkShopBtn.onClick.AddListener(OnClickWorkShop);

        // choose weapon
        m_StartDefenceBtn.onClick.AddListener(OnClickStartDefence);
        m_CancelChooseWeaponBtn.onClick.AddListener(OnClickCancelInChooseWeapon);

        // weapon list panel
        m_ComfirmWeaponChangeBtn.onClick.AddListener(OnClickComfirmInWeaponList);

        m_OptionBtn.onClick.AddListener(()=>{
            m_OptionPanel.SetActive(true);
        });
    }
    
    private void OnClickWorkShop(){
        TurnOffAllPanel();
        m_WorkShopChooseWeaponController.Init();
        m_WorkShopChooseWeaponController.gameObject.SetActive(true);
    }

    private void CancelLocationDetail() {
        
        m_CheckLocationBtn.gameObject.SetActive(true);
        m_LocationDetailPanel.SetActive(false);
        MapManager.GetInstance().GetNearestLocationController().SetLocationCameraPiority(0);
        StartCoroutine(SetMovable());
        
    }

    private IEnumerator SetMovable(){
        // wait until location detail panel are hidden , then set movable
        yield return null;
        MapManager.GetInstance().GetVehicleController().ChangeMovable(true);

    }

    private void OnClickDefence(){
        TurnOffAllPanel();

        // set choose weapon panel
        m_ChooseWeaponPanel.SetActive(true);

        var allSelectedWeapon = MainGameManager.GetInstance().GetAllSelectedWeapon();
        for (int i = 0; i < 4; i++)
        {
            int index = i;
            var selectedWeapon = allSelectedWeapon[i];
            if(selectedWeapon!=null){
                m_ChooseWeaponBtns[i].Init(index ,selectedWeapon.DisplayImage);
            }
        }
    }

    private void OnClickComfirmInWeaponList(){
        OnClickDefence();
    }

    private void OnClickCancelInChooseWeapon(){
        m_ChooseWeaponPanel.SetActive(false);
        m_LocationDetailPanel.SetActive(true);
    }

    private void OnClickStartDefence() {
        MapLocationScriptable locationData = MapManager.GetInstance().GetNearestLocationController().GetScriptable();
        if(locationData==null)
            return;
            
        MainGameManager.GetInstance().SetBaseDefenceScene(locationData);
    }

    private void ShowLocationDetail(){
        MapLocationScriptable locationData = MapManager.GetInstance().GetNearestLocationController().GetScriptable();
        if(locationData==null)
            return;

        
        MapManager.GetInstance().GetNearestLocationController().SetLocationCameraPiority(10);

        m_LocationDetailPanel.SetActive(true);
        m_LocationName.text = locationData.DisplayName;
/*
        // no Reward
        if(locationData.RewardGunId!=-1f){
            var allWeapon = MainGameManager.GetInstance().GetAllWeapon();
            var rewardGun = allWeapon.Find(x=>x.Gun.Id == locationData.RewardGunId).Gun;

            if(allWeapon.Find(x=>x.Gun.Id == locationData.RewardGunId).IsOwned){
                // already own weapon
                m_FortifyCost.text = "Fortified";
                m_FortifyBtn.gameObject.SetActive(false);
            }else{
                m_FortifyCost.text = "Fortify Cost : "+locationData.FortifyCost;
                m_FortifyBtn.gameObject.SetActive(true);
            }
            m_FortifyRewardText.text = "Fortify Reward : "+rewardGun.DisplayName;

            m_RewardImage.sprite = rewardGun.DisplayImage;
            // TODO :  m_EnemyText.text = " Enemy : ( Danger "+locationData.WaveData.NormalWavesStrength+","+locationData.WaveData.FinalWaveStrength+" )";
        }else{
            m_FortifyCost.text = "Fortified";
            m_FortifyBtn.gameObject.SetActive(false);
        }*/

        
        

        // TODO : enemy list
        MapManager.GetInstance().GetVehicleController().ChangeMovable(false);

        m_CheckLocationBtn.gameObject.SetActive(false);

    }

    public void ShowWeaponListPanel(int weaponSlotIndex){
        TurnOffAllPanel();
        m_WeaponListPanel.SetActive(true);
        var allSelectedWeawpon = MainGameManager.GetInstance().GetAllSelectedWeapon();
        List<int> allSelectedWeaponId = new List<int>();
        foreach (var item in allSelectedWeawpon)
        {
            allSelectedWeaponId.Add(item.Id);
        }

        SetWeaponlistSelectedWeaponData(allSelectedWeawpon[weaponSlotIndex]);

        var allWeapon = MainGameManager.GetInstance().GetAllWeapon();
        
        for (int i = 0; i < m_WeaponListSlotParent.childCount; i++)
        {
            Destroy(m_WeaponListSlotParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < allWeapon.Count; i++)
        {
            //Debug.Log("check weawpon"+allWeapon[i].Gun.Id);
            // >>>>  check if other slot have the smae weapon  <<<<<
            if(allWeapon[i].IsOwned && !allSelectedWeaponId.Contains( allWeapon[i].Gun.Id)){
                var newWeaponListSlot = Instantiate(m_WeaponListSlotPrefab, m_WeaponListSlotParent);
                newWeaponListSlot.GetComponent<MapWeaponListGrid>().Init(allWeapon[i].Gun,weaponSlotIndex);
            }
        }
    }

    public void ChangeCheckLocationActive(bool isActive){
        if(m_LocationDetailPanel.activeSelf){
            // no check location btn if already checking
            return;
        }
            m_CheckLocationBtn.gameObject.SetActive(isActive);
    }

/*
    private void OnClickFortify(){
        MapLocationScriptable locationData = MapManager.GetInstance().GetNearestLocationController().GetScriptable();
        if(locationData==null)
            return;

        var rewardGunOwnership = MainGameManager.GetInstance().GetAllWeapon().Find(x=>x.Gun.Id == locationData.RewardGunId);
        var rewardGun = rewardGunOwnership.Gun;
        // if alredy unlocked No Show
        if( rewardGunOwnership.IsOwned ){
            Debug.Log("already unlocked gun"+rewardGun.DisplayName);
            return;
        }


        // show fortify reward
        m_FortifyPanel.SetActive(true);
        var gooOwned = MainGameManager.GetInstance().GetGooAmount();
        m_FortifyCostInDetail.text = "Fortify Cost : "+locationData.FortifyCost+" ("+gooOwned.ToString()+")";
        m_RewardName.text = "Fortify Reward : "+rewardGun.DisplayName;
        m_RewardImageInDetail.sprite = rewardGun.DisplayImage;
        if( gooOwned < locationData.FortifyCost ){
            m_YesFortifyBtn.gameObject.SetActive(false);
        }else{
            m_YesFortifyBtn.gameObject.SetActive(true);
        }
    }*/
/*
    private void OnClickFortifyYes(){
        var locationScriptable = MapManager.GetInstance().GetNearestLocationController().GetScriptable();
        MainGameManager.GetInstance().ChangeGooAmount(
            locationScriptable.FortifyCost * -1f
        );

        MainGameManager.GetInstance().UnlockGun(locationScriptable.RewardGunId);

        m_FortifyPanel.SetActive(false);
        m_LocationDetailPanel.SetActive(true);
        
        m_GooText.text = "Goo : "+MainGameManager.GetInstance().GetGooAmount();

        ShowLocationDetail();
    }*/

    public void OnClickWeaponListSlot(GunScriptable selectedGunScriptable, int weaponSlotIndex, MapWeaponListGrid mapWeaponListGrid) {

        SetWeaponlistSelectedWeaponData(selectedGunScriptable);
        var allSelectedWeawpon = MainGameManager.GetInstance().GetAllSelectedWeapon();
        mapWeaponListGrid.Init(allSelectedWeawpon[weaponSlotIndex], weaponSlotIndex);

        MainGameManager.GetInstance().ChangeSelectedWeapon(weaponSlotIndex, selectedGunScriptable);
    }


    private void SetWeaponlistSelectedWeaponData(GunScriptable selectedGunScriptable){
        m_SelectedWeaponName.text = "Name : "+selectedGunScriptable.DisplayName;
        m_SelectedWeaponFireRate.text = "Fire Rate : "+selectedGunScriptable.GunStats.FireRate;
        m_SelectedWeaponDamage.text = "Damage : "+selectedGunScriptable.GunStats.DamagePerPellet+" x "+selectedGunScriptable.GunStats.PelletPerShot.ToString();
        m_SelectedWeaponClipSize.text = "Clip Size : "+selectedGunScriptable.GunStats.ClipSize;
        m_SelectedWeaponAcc.text = "Acc : "+selectedGunScriptable.GunStats.Accuracy.ToString();
        m_SelectedWeaponRecoil.text = "Recoil : "+selectedGunScriptable.GunStats.Recoil.ToString();
        m_SelectedWeaponHandling.text = "Handling : "+selectedGunScriptable.GunStats.Handling.ToString();
        m_SelectedSlotWeapon.sprite = selectedGunScriptable.DisplayImage;

    }
/*
    private void OnClickFortifyNo(){
        // cancel fortify
        m_FortifyPanel.SetActive(false);

    }*/

    private void TurnOffAllPanel(){
        m_ChooseWeaponPanel.SetActive(false);
        m_LocationDetailPanel.SetActive(false);
        m_WorkShopChooseWeaponController.gameObject.SetActive(false);
        m_WeaponListPanel.SetActive(false);
        m_OptionPanel.SetActive(false);
    }
}
