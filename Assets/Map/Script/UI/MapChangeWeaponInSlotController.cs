using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapChangeWeaponInSlotController : MonoBehaviour
{
    // chosing weapon on a slot
    [SerializeField] private GameObject m_WeaponInSlotPanel;
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
    [SerializeField] private Button m_CancelWeaponChangeBtn; 

    private int m_SlotIndex = -1;
    private GunScriptable m_SelectedGunScriptable;

    IEnumerator Start(){
        yield return null;
        m_CancelWeaponChangeBtn.onClick.AddListener(()=>{
            MapManager.GetInstance().GetMapUIController().ResetPreFightPanel();
            m_WeaponInSlotPanel.SetActive(false);
        });
        m_WeaponInSlotPanel.SetActive(false);
    }
    public void ShowWeaponListPanel(int weaponSlotIndex){
        m_SlotIndex = weaponSlotIndex;
        m_WeaponInSlotPanel.SetActive(true);
        m_ComfirmWeaponChangeBtn.onClick.RemoveAllListeners();

        m_ComfirmWeaponChangeBtn.onClick.AddListener(()=>{
            MainGameManager.GetInstance().SaveData<int>("SelectedWeapon"+m_SlotIndex.ToString(),m_SelectedGunScriptable.Id);
            MapManager.GetInstance().GetMapUIController().ResetPreFightPanel();
            m_WeaponInSlotPanel.SetActive(false);
            });
        
        

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
        if(curSelectedWeaponId > -1){
            GunScriptable targetGun = allWeapon.Find(x=>x.Id == curSelectedWeaponId);
            bool isWeaponOwned = (int)MainGameManager.GetInstance().GetData<int>(targetGun.DisplayName+targetGun.Id.ToString(),"-1") ==1 ; 
            SetWeaponListSelectedWeaponData( targetGun, isWeaponOwned);
            m_SelectedGunScriptable = targetGun;
        }else{
            // RESET show detail if no selected weapon
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
            if( !isEquipbyOtherSlot && isUnlocked){
                var newWeaponListSlot = Instantiate(m_WeaponListSlotPrefab, m_WeaponListSlotParent);
                newWeaponListSlot.GetComponent<MapWeaponListGrid>().Init(allWeapon[i],weaponSlotIndex,isUnlocked);
            }
        }
    }

    
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

    }

    

    public void OnClickWeaponListSlot(bool isWeaponLocked,GunScriptable selectedGunScriptable, int weaponSlotIndex, MapWeaponListGrid mapWeaponListGrid) {
        if(selectedGunScriptable == null)
            return;

        SetWeaponListSelectedWeaponData(selectedGunScriptable,!isWeaponLocked);
        
        m_SelectedGunScriptable = selectedGunScriptable;
        m_ComfirmWeaponChangeBtn.gameObject.SetActive(!isWeaponLocked);

        //var allSelectedWeawpon = MainGameManager.GetInstance().GetAllSelectedWeapon();
        //string gunUnlockKey = selectedGunScriptable.DisplayName+selectedGunScriptable.Id.ToString();
        //bool isUnlocked = System.Convert.ToSingle(MainGameManager.GetInstance().GetData<int>(gunUnlockKey,"-1"))==1;
        mapWeaponListGrid.Init(m_SelectedGunScriptable, weaponSlotIndex, !isWeaponLocked);


    }

}
