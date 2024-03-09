using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExtendedButtons;
using TMPro;
using System;
using Unity.VisualScripting;

public class MapUIController : MonoBehaviour
{
    [SerializeField] private Button2D m_MapDragBtn;  
    [SerializeField] private Button2D m_NextLevelBtn;
    [SerializeField] private Button2D m_LastLevelBtn;
    [SerializeField]private Animator m_CoverAnimator;

    [Header("Option")]
    [SerializeField] private Button2D m_OptionBtn;
    [SerializeField] private GameObject m_OptionPanel;


    [Header("WeaponUpgradeSelect")]
    [SerializeField] private WorkShopChooseWeaponController m_WorkShopChooseWeaponController;
    [SerializeField] private Button2D m_WorkShopBtn;


    [Header("ChooseWeapon")]
    [SerializeField] private GameObject m_ChooseWeaponPanel;
    [SerializeField] private List<MapChooseWeaponSlot> m_ChooseWeaponBtns = new List<MapChooseWeaponSlot>();
   // [SerializeField] private Transform m_ChooseWeaponEnemyList;
    [SerializeField] private Button2D m_StartDefenceBtn;  
    [SerializeField] private Button2D m_CancelChooseWeaponBtn;  
    
    [Header("Weaponlist")]
    [SerializeField] private MapChangeWeaponInSlotController m_ChangeWeaponInSlotController;

    [Header("Dialog")]
    [SerializeField] private MapDialogController m_DialogController;
    [Header("Location")]
    [SerializeField] private LocationPanelController m_LocationPanelController;


    private void Start() {
        m_OptionPanel.GetComponent<OptionMenuController>().Init(null) ;
        // TODO : Split all ui panel
        m_CoverAnimator.Play("Hidden");
        TurnOffAllPanel();
        m_WorkShopChooseWeaponController.gameObject.SetActive(true);

        m_NextLevelBtn.onClick.RemoveAllListeners();
        MainGameManager.GetInstance().AddOnClickBaseAction(m_NextLevelBtn,m_NextLevelBtn.GetComponent<RectTransform>());
        m_NextLevelBtn.onClick.AddListener(()=>{
            // to next level
            m_NextLevelBtn.gameObject.SetActive(false);
            CoverAnimtion(()=>ChangeLevel(1));
            
        });
        m_NextLevelBtn.gameObject.SetActive(false);

        m_LastLevelBtn.onClick.RemoveAllListeners();

        
        MainGameManager.GetInstance().AddOnClickBaseAction(m_LastLevelBtn,m_LastLevelBtn.GetComponent<RectTransform>());
        m_LastLevelBtn.onClick.AddListener(()=>{
            // to last level
            m_LastLevelBtn.gameObject.SetActive(false);
            CoverAnimtion(()=>ChangeLevel(-1));
            
        });
        m_LastLevelBtn.gameObject.SetActive(false);


        var freeCameraController = MapManager.GetInstance().GetMapFreeCameraController();
        m_MapDragBtn.onDown.AddListener(freeCameraController.OnDragMapBtnDown);
        m_MapDragBtn.onUp.AddListener(freeCameraController.OnDragMapBtnUp);
        m_MapDragBtn.onExit.AddListener(freeCameraController.OnDragMapBtnUp);



        // WorkShop
        MainGameManager.GetInstance().AddOnClickBaseAction(m_WorkShopBtn,m_WorkShopBtn.GetComponent<RectTransform>());
        m_WorkShopBtn.onClick.AddListener(OnClickWorkShop);



        MainGameManager.GetInstance().AddOnClickBaseAction(m_StartDefenceBtn,m_StartDefenceBtn.GetComponent<RectTransform>());
        m_StartDefenceBtn.onClick.AddListener(OnClickStartDefence);

        
        MainGameManager.GetInstance().AddOnClickBaseAction(m_CancelChooseWeaponBtn,m_CancelChooseWeaponBtn.GetComponent<RectTransform>());
        m_CancelChooseWeaponBtn.onClick.AddListener(OnClickCancelInChooseWeapon);
/*
        // weapon list panel
        m_ComfirmWeaponChangeBtn.onClick.AddListener(OnClickComfirmInWeaponList);*/

        
        
        MainGameManager.GetInstance().AddOnClickBaseAction(m_OptionBtn,m_OptionBtn.GetComponent<RectTransform>());
        m_OptionBtn.onClick.AddListener(()=>{
            var optionController = m_OptionPanel.GetComponent<OptionMenuController>();
            optionController.Open();
        });
    }

    private void ChangeLevel(int levelChanges){
        var curSelectedlocationLevel = MainGameManager.GetInstance().GetSelectedLocation().Level;
        MapLocationScriptable newSelectedLocation =null;
        if(levelChanges<0){
            newSelectedLocation = MainGameManager.GetInstance().GetAllLocation().FindLast(x=>x.Level==curSelectedlocationLevel+levelChanges);
        }else{
            newSelectedLocation = MainGameManager.GetInstance().GetAllLocation().Find(x=>x.Level==curSelectedlocationLevel+levelChanges);
        }
        MainGameManager.GetInstance().SetSelectedLocation(newSelectedLocation);
        MapManager.GetInstance().SpawnAllLocation();
        MapManager.GetInstance().CameraLookAtSelectedLocation();

    }


    private void CoverAnimtion(Action onCoveredAction){
        StartCoroutine(CoverAnimation(onCoveredAction));
    }

    private IEnumerator CoverAnimation(Action onCoveredAction){
        m_CoverAnimator.Play("Open");
        float passTime = 0;
        float duration = 1.5f;
        bool isActioned = false;
        while (passTime<duration)
        {
            passTime += Time.deltaTime;
            yield return null;
            if(passTime > 0.75f && !isActioned ){
                onCoveredAction?.Invoke();
                isActioned = true;
            }
        }
        m_CoverAnimator.Play("Close");
    }

    public void SetToOtherLevelBtn(MapToOtherLevelBtnStage btnStage ){
        // hide if no next level
        var selectedLocation = MainGameManager.GetInstance().GetSelectedLocation();
        m_LastLevelBtn.gameObject.SetActive(btnStage==MapToOtherLevelBtnStage.ToLast && selectedLocation.Level >0);
        m_NextLevelBtn.gameObject.SetActive(btnStage==MapToOtherLevelBtnStage.ToNext && selectedLocation.Level <3);
    }
    
    private void OnClickWorkShop(){
        TurnOffAllPanel();
        m_WorkShopChooseWeaponController.Init();
        m_WorkShopChooseWeaponController.gameObject.SetActive(true);
    }



    public void ShowChangeWeaponInSlotPanel(int weaponSlotIndex){
        m_ChangeWeaponInSlotController.ShowWeaponListPanel(weaponSlotIndex);
    }

    public void OnClickDefence(){
        TurnOffAllPanel();

        // set choose weapon panel
        m_ChooseWeaponPanel.SetActive(true);

        var allWeapon = MainGameManager.GetInstance().GetAllWeapon();

        for (int i = 0; i < 4; i++)
        {
            int index = i;
            var targetGunId = (int)MainGameManager.GetInstance().GetData<int>("SelectedWeapon"+index.ToString(),"-1");
            GunScriptable targetGunScriptable = allWeapon.Find(x=>x.Id == targetGunId);
            if( targetGunScriptable == null ){
                //no selected weapon
                m_ChooseWeaponBtns[index].Init(index , null);
            }else{
                m_ChooseWeaponBtns[index].Init(index ,targetGunScriptable.DisplayImage);

            }
            
        }
    }

    public void ResetPreFightPanel(){
        OnClickDefence();
    }

    private void OnClickCancelInChooseWeapon(){
        m_ChooseWeaponPanel.SetActive(false);
        m_LocationPanelController.gameObject.SetActive(true);
    }

    public void OnEndDefenceShowDialog(){
        MapLocationScriptable locationData = MainGameManager.GetInstance().GetSelectedLocation();

        if(locationData==null)
            return;

        m_DialogController.Init(locationData.EndDialogId,null);
        
    }

    private void OnClickStartDefence() {
        MapLocationScriptable locationData = MapManager.GetInstance().GetLocationController().GetScriptable();
        if(locationData==null)
            return;

        m_DialogController.Init(locationData.StartDialogId,OnDialogEndByStartDefence);
    }

    private void OnDialogEndByStartDefence(){
        MapLocationScriptable locationData = MapManager.GetInstance().GetLocationController().GetScriptable();
        MainGameManager.GetInstance().SetBaseDefenceScene(locationData);
    }

    public void ShowWeaponListPanel(int weaponSlotIndex){
        TurnOffAllPanel();
        /*
        m_WeaponListPanel.SetActive(true);

        //get all selected weapon , prevent showing selected weapon in list 
        List<GunScriptable> allSelectedWeawpon = MainGameManager.GetInstance().GetAllSelectedWeapon();
        List<GunScriptable> allWeapon = MainGameManager.GetInstance().GetAllWeapon();
        List<int> allSelectedWeaponId = new List<int>();
        foreach (var item in allSelectedWeawpon)
        {
            if(item != null)
                allSelectedWeaponId.Add(item.Id);
        }

        int curSelectedWeaponId =  (int)MainGameManager.GetInstance().GetData<int>("SelectedWeapon"+weaponSlotIndex.ToString(),"-1") ; 
        // RESET show detail if no selected weapon
        if(curSelectedWeaponId > -1){
            GunScriptable targetGun = allWeapon.Find(x=>x.Id == curSelectedWeaponId);
            bool isWeaponOwned = (int)MainGameManager.GetInstance().GetData<int>(targetGun.DisplayName+targetGun.Id.ToString(),"-1") ==1 ; 
            SetWeaponListSelectedWeaponData( targetGun, isWeaponOwned);
        }else{
            ReseretWeaponListSelectedWeaponData();
        }   

        // show all weapon
        
        for (int i = 0; i < m_WeaponListSlotParent.childCount; i++)
        {
            Destroy(m_WeaponListSlotParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < allWeapon.Count; i++)
        {
            string gunUnlockKey = allWeapon[i].DisplayName+allWeapon[i].Id.ToString();
            bool isUnlocked = System.Convert.ToSingle(MainGameManager.GetInstance().GetData<int>(gunUnlockKey))==1;
            bool isEquipbyOtherSlot = allSelectedWeaponId.Contains( allWeapon[i].Id);
            if( !isEquipbyOtherSlot ){
                var newWeaponListSlot = Instantiate(m_WeaponListSlotPrefab, m_WeaponListSlotParent);
                newWeaponListSlot.GetComponent<MapWeaponListGrid>().Init(allWeapon[i],weaponSlotIndex,isUnlocked);
            }
        }*/
    }

/*
    private void OnClickFortify(){
        MapLocationScriptable locationData = MapManager.GetInstance().GetLocationController().GetScriptable();
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
        var locationScriptable = MapManager.GetInstance().GetLocationController().GetScriptable();
        MainGameManager.GetInstance().ChangeGooAmount(
            locationScriptable.FortifyCost * -1f
        );

        MainGameManager.GetInstance().UnlockGun(locationScriptable.RewardGunId);

        m_FortifyPanel.SetActive(false);
        m_LocationDetailPanel.SetActive(true);
        
        m_GooText.text = "Goo : "+MainGameManager.GetInstance().GetGooAmount();

        ShowLocationDetail();
    }*/

    public void OnClickWeaponListSlot(bool isWeaponLocked,GunScriptable selectedGunScriptable, int weaponSlotIndex, MapWeaponListGrid mapWeaponListGrid) {
        
        m_ChangeWeaponInSlotController.OnClickWeaponListSlot(isWeaponLocked, selectedGunScriptable, weaponSlotIndex, mapWeaponListGrid);

    }
/*
    private void ReseretWeaponListSelectedWeaponData(){
        m_SelectedWeaponName.text = "Name : ";
        m_SelectedWeaponFireRate.text = "Fire Rate : ";
        m_SelectedWeaponDamage.text = "Damage : ";
        m_SelectedWeaponClipSize.text = "Clip Size : ";
        m_SelectedWeaponAcc.text = "Acc : ";
        m_SelectedWeaponRecoil.text = "Recoil : ";
        m_SelectedWeaponHandling.text = "Handling : ";
        m_SelectedSlotWeapon.sprite = null;
        m_SelectedSlotWeapon.color = Color.black;

    }

    private void SetWeaponListSelectedWeaponData(GunScriptable selectedGunScriptable, bool isWeaponOwned = false){
        m_SelectedWeaponName.text = "Name : "+selectedGunScriptable.DisplayName;
        m_SelectedWeaponFireRate.text = "Fire Rate : "+System.Convert.ToSingle(selectedGunScriptable.GetStatValue(GunScriptableStatEnum.FireRate));
        m_SelectedWeaponDamage.text = "Damage : "+System.Convert.ToSingle(selectedGunScriptable.GetStatValue(GunScriptableStatEnum.Damage))+
            " x "+ System.Convert.ToSingle(selectedGunScriptable.GetStatValue(GunScriptableStatEnum.Pellet));
        m_SelectedWeaponClipSize.text = "Clip Size : "+ System.Convert.ToSingle(selectedGunScriptable.GetStatValue(GunScriptableStatEnum.ClipSize));
        m_SelectedWeaponAcc.text = "Acc : "+ System.Convert.ToSingle(selectedGunScriptable.GetStatValue(GunScriptableStatEnum.Accuracy));
        m_SelectedWeaponRecoil.text = "Recoil : "+ System.Convert.ToSingle(selectedGunScriptable.GetStatValue(GunScriptableStatEnum.Recoil));
        m_SelectedWeaponHandling.text = "Handling : "+ System.Convert.ToSingle(selectedGunScriptable.GetStatValue(GunScriptableStatEnum.Handling));
        m_SelectedSlotWeapon.sprite = selectedGunScriptable.DisplayImage;
        m_SelectedSlotWeapon.color = isWeaponOwned ? Color.white : Color.black;

    }*/
/*
    private void OnClickFortifyNo(){
        // cancel fortify
        m_FortifyPanel.SetActive(false);

    }*/

    private void TurnOffAllPanel(){
        m_NextLevelBtn.gameObject.SetActive(false);
        m_LastLevelBtn.gameObject.SetActive(false);
        m_ChooseWeaponPanel.SetActive(false);
        m_LocationPanelController.gameObject.SetActive(false);
        m_WorkShopChooseWeaponController.gameObject.SetActive(false);
        //m_WeaponListPanel.SetActive(false);
    }
}
