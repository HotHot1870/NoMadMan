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
    //private MapLocationScriptable m_SelectedLocation = null;

    [Header("Location")]
    [SerializeField] private GameObject m_LocationDetailPanel; 
    [SerializeField] private TMP_Text m_LocationName;  
    [SerializeField] private TMP_Text m_FortifyCost;  
    [SerializeField] private TMP_Text m_FortifyRewardText;  
    [SerializeField] private Image m_RewardImage;
    [SerializeField] private TMP_Text m_EnemyText;  
    [SerializeField] private Transform m_EnemyList; 
    [SerializeField] private Button m_FortifyBtn;  
    [SerializeField] private Button m_DefenceBtn;
    [SerializeField] private Button m_CancelLocationDetailBtn;

    [Header("Fortify Detail")]
    [SerializeField] private GameObject m_FortifyPanel; 
    [SerializeField] private TMP_Text m_FortifyCostInDetail; 
    [SerializeField] private TMP_Text m_RewardName;
    [SerializeField] private Image m_RewardImageInDetail;
    [SerializeField] private Button m_YesFortifyBtn;  
    [SerializeField] private Button m_NoFortifyBtn;  

    [Header("ChooseWeapon")]
    [SerializeField] private GameObject m_ChooseWeaponPanel;
    [SerializeField] private List<MapChooseWeaponSlot> m_ChooseWeaponBtns = new List<MapChooseWeaponSlot>();
    [SerializeField] private Transform m_ChooseWeaponEnemyList;
    [SerializeField] private Button m_StartDefenceBtn;  
    [SerializeField] private Button m_CancelChooseWeaponBtn;  

    [Header("Weaponlist")]
    [SerializeField] private GameObject m_WeaponListPanel;
    [SerializeField] private Image m_SelectedSlotWeapon;
    [SerializeField] private Transform m_WeaponListSlotParent;
    [SerializeField] private GameObject m_WeaponListSlotPrefab;
    [SerializeField] private Button m_ComfirmWeaponChangeBtn;  




    private void Start() {
        m_CheckLocationBtn.gameObject.SetActive(false);
        TurnOffAllPanel();
        var freeCameraController = MapManager.GetInstance().GetMapFreeCameraController();
        m_MapDragBtn.onDown.AddListener(freeCameraController.OnDragMapBtnDown);
        m_MapDragBtn.onUp.AddListener(freeCameraController.OnDragMapBtnUp);
        m_MapDragBtn.onExit.AddListener(freeCameraController.OnDragMapBtnUp);

        m_CheckLocationBtn.onClick.AddListener(ShowLocationDetail);

        // location detail
        m_FortifyBtn.onClick.AddListener(OnClickFortify);
        m_DefenceBtn.onClick.AddListener(OnClickDefence);
        m_CancelLocationDetailBtn.onClick.AddListener(CancelLocationDetail);

        // fortify detail panel
        m_NoFortifyBtn.onClick.AddListener(OnClickFortifyNo);
        m_YesFortifyBtn.onClick.AddListener(OnClickFortifyYes);

        // choose weapon
        m_StartDefenceBtn.onClick.AddListener(OnClickStartDefence);
        m_CancelChooseWeaponBtn.onClick.AddListener(OnClickCancelInChooseWeapon);

        // weapon list panel
        m_ComfirmWeaponChangeBtn.onClick.AddListener(OnClickComfirmInWeaponList);
    }

    private void CancelLocationDetail() {
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
        TurnOffAllPanel();
        m_ChooseWeaponPanel.SetActive(true);
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
        m_LocationName.text = locationData.LocationName;
        m_FortifyCost.text = "Fortify Cost : "+locationData.FortifyCost;
        m_FortifyRewardText.text = "Fortify Reward : "+locationData.Reward.DisplayName;


        m_RewardImage.sprite = locationData.Reward.DisplayImage;
        m_EnemyText.text = " Enemy : ( Danger "+locationData.WaveData.NormalWavesDangerValue+","+locationData.WaveData.FinalWaveDangerValue+" )";
        // TODO : enemy list
        MapManager.GetInstance().GetVehicleController().ChangeMovable(false);

        m_CheckLocationBtn.gameObject.SetActive(false);

    }

    public void ShowWeaponListPanel(int weaponSlotIndex){
        TurnOffAllPanel();
        m_WeaponListPanel.SetActive(true);
        var allSelectedWeawpon = MainGameManager.GetInstance().GetAllSelectedWeapon();
        m_SelectedSlotWeapon.sprite = allSelectedWeawpon[weaponSlotIndex].DisplayImage;

        var allWeapon = MainGameManager.GetInstance().GetAllWeapon();
        for (int i = 0; i < m_WeaponListSlotParent.childCount; i++)
        {
            Destroy(m_WeaponListSlotParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < allWeapon.Count; i++)
        {
            // >>>>  check if other slot have the smae weapon  <<<<<
            if(allWeapon[i].IsOwned && !allSelectedWeawpon.Contains(allWeapon[i].Gun)){
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


    private void OnClickFortify(){
        MapLocationScriptable locationData = MapManager.GetInstance().GetNearestLocationController().GetScriptable();
        if(locationData==null)
            return;

        // show fortify reward
        m_FortifyPanel.SetActive(true);
        m_FortifyCostInDetail.text = "Fortify Cost : "+locationData.FortifyCost;
        m_RewardName.text = "Fortify Reward : "+locationData.Reward.DisplayName;
        m_RewardImageInDetail.sprite = locationData.Reward.DisplayImage;
    }

    private void OnClickFortifyYes(){
        // TODO : comfirm fortify
    }

    public void OnClickWeaponListSlot(GunScriptable selectedGunScriptable, int weaponSlotIndex, MapWeaponListGrid mapWeaponListGrid) {
        var allSelectedWeawpon = MainGameManager.GetInstance().GetAllSelectedWeapon();
        m_SelectedSlotWeapon.sprite = selectedGunScriptable.DisplayImage;
        mapWeaponListGrid.Init(allSelectedWeawpon[weaponSlotIndex], weaponSlotIndex);

        MainGameManager.GetInstance().ChangeSelectedWeapon(weaponSlotIndex, selectedGunScriptable);
    }

    private void OnClickFortifyNo(){
        // cancel fortify
        m_FortifyPanel.SetActive(false);

    }

    private void TurnOffAllPanel(){
        m_ChooseWeaponPanel.SetActive(false);
        m_LocationDetailPanel.SetActive(false);
        m_FortifyPanel.SetActive(false);
        m_WeaponListPanel.SetActive(false);
    }
}
