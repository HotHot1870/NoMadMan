using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BaseDefenceNameSpace;

namespace BaseDefenceNameSpace
{
    public class SpawnUIObjectForReloadPhaseConfig
    {
        public GameObject Prefab;
        public Vector2 Position;
        public Vector2 Size = new Vector2 (150,150);
        public string UnderText;
    }

    public class GunReloadControllerConfig
    {
        public GunScriptable GunScriptable;
        //public Action CancelReload;
        public Action<int> GainAmmo;
        public Action SetAmmoToFull;
        public Action SetAmmoToZero;
        public Func<bool> IsFullClipAmmo;
    }

    public enum BaseDefenceStage{
        Shoot,
        SwitchWeapon,
        Reload,
        Result
    }
    [System.Serializable]
    public class DefenceEnvironment
    {
        public string Stagename;
        public int Level;
        public GameObject Prefeb;
        public Material SkyBox;
    }
}

public class BaseDefenceManager : MonoBehaviour
{
    public static BaseDefenceManager m_Instance = null;
    [SerializeField] private EnemySpawnController m_EnemySpawnController;
    [SerializeField] private BaseDefenceUIController m_BaseDefenceUIController;
    [SerializeField] private GunShootController m_GunShootController;
    [SerializeField] private GunReloadController m_ReloadController;
    [SerializeField] private CameraController m_CameraController;
    [SerializeField] private BaseDefenceResultPanel m_BaseDefenceResultPanel;
    [SerializeField] private CrosshairControl m_CrosshairControl;
    [SerializeField] private GunModelComtroller m_GunModelController;
    [SerializeField] private GameObject m_ReloadControllerPanel;
    [SerializeField] private Transform m_EnvironmentParent;
    [SerializeField] private List<DefenceEnvironment> m_AllEnvironmentPrefab = new List<DefenceEnvironment>();

    private float m_CurrentAccruacy = 100f;

    // hp
    private float m_CurrentHp = 1000;
    [SerializeField]private float m_MaxHp = 1000;




    [Header("Enemy Hp Bars")]
    [SerializeField] private Transform m_EnemyHpBarParent;
    public Transform EnemyHpBarParent { get { return m_EnemyHpBarParent; } }
    private float m_TotalHpBarStayTime = 0;

    
    #region UpdateAction
    public Action m_ShootUpdateAction = null;
    public Action m_SwitchWeaponUpdateAction = null;
    public Action m_ReloadUpdateAction = null;
    public Action m_UpdateAction = null;
    #endregion

    #region Change Game Stage From
    public Action m_ChangeFromShootAction = null;
    public Action m_ChangeFromSwitchWeaponAction = null;
    public Action m_ChangeFromReloadAction = null;
    #endregion

    #region Change Game Stage To
    public Action m_ChangeToShootAction = null;
    public Action m_ChangeToSwitchWeaponAction = null;
    public Action m_ChangeToReloadAction = null;
    #endregion

    private MapLocationScriptable m_CurlocationData ;

    private BaseDefenceStage m_GameStage = BaseDefenceStage.Shoot;
    public BaseDefenceStage GameStage {get { return m_GameStage; }}


    private void Awake() {
        if(m_Instance==null){
            m_Instance = this;
        }else{
            Destroy(this);
        }
    }

    public static BaseDefenceManager GetInstance(){
        if(m_Instance==null){
            m_Instance = new GameObject().AddComponent<BaseDefenceManager>();
        }
        return m_Instance;
    }


    private void Start() {
        m_CurrentHp = m_MaxHp;
        //m_Controller.Init(m_Controller.GetMaxHp());

        m_ChangeFromReloadAction += CloseReloadPanel;
        //m_Controller.m_HpBarFiller.fillAmount = MainGameManager.GetInstance().GetCurHp() / MainGameManager.GetInstance().GetMaxHp();

    }


    

    public float GetCurHp(){
        return m_CurrentHp;
    }

    public float GetMaxHp(){
        return m_MaxHp;
    }

    public void ChangeHp(float changes){
        m_CurrentHp += changes;
        if(m_CurrentHp<0){
            m_CurrentHp = 0;
        }else if(m_CurrentHp>m_MaxHp){
            m_CurrentHp = m_MaxHp;
        }
    }

/*
    public List<Transform> GetAllEnemyTrans(){
        return m_EnemySpawnController.GetAllEnemyTrans();
    }*/
    public void AddEnemyToList(Transform trans){
        m_EnemySpawnController.AddEnemyToList(trans);
    }

    public void RemoveDeadEnemyFromList(Transform trans){
        m_EnemySpawnController.RemoveDeadEnemyFromList(trans);
    }

    public int GetEnemySpawnId(){
        return m_EnemySpawnController.GetEnemySpawnId();
    }

    public void StartWave(MapLocationScriptable locationInfo){
        var targetEnvironment = m_AllEnvironmentPrefab.Find(x=>x.Level==locationInfo.Level);
        if(targetEnvironment == null){
            Debug.LogError($"Level {locationInfo.Level} environment not found ");
            return;
        }

        // change sky box
        RenderSettings.skybox= targetEnvironment.SkyBox ;
        // change ground
        Instantiate(targetEnvironment.Prefeb,m_EnvironmentParent);


        m_CurlocationData = locationInfo;
        m_EnemySpawnController.StartWave( locationInfo);
        MainGameManager.GetInstance().SetSelectedLocation(m_CurlocationData);
    }

    public MapLocationScriptable GetLocationScriptable(){
        return m_CurlocationData;
    }


    private void Update() {
        switch (m_GameStage)
        {
            case BaseDefenceStage.Shoot:
                m_ShootUpdateAction?.Invoke();
            break;
            case BaseDefenceStage.SwitchWeapon:
                m_SwitchWeaponUpdateAction?.Invoke();
            break;
            case BaseDefenceStage.Reload:
                m_ReloadUpdateAction?.Invoke();
            break;
            default:
            break;
        }
        m_UpdateAction?.Invoke();
    }
    
    private void FixedUpdate()
    {
        //  hp bar stay time
        if(m_TotalHpBarStayTime>0){
            m_TotalHpBarStayTime -= Time.deltaTime;
        }else{
            m_BaseDefenceUIController.UISetActive(false);
        }
    }

    public void SetDamageText(float damage , Color color,Vector2 pos){
        m_BaseDefenceUIController.ShowDamageText(damage,color,pos);
    }

    public BaseDefenceUIController GetBaseDefenceUIController(){
        return m_BaseDefenceUIController;
    }

    
    public Vector3 GetCrosshairPos(){
        return m_CrosshairControl.GetCrosshairPos();
    }

    public CrosshairControl GetCrosshairController(){
        return m_CrosshairControl;
    }

    public CameraController GetCameraController(){
        return m_CameraController;
    }

    public void ChangeGameStage(BaseDefenceStage newStage){
        switch (m_GameStage)
        {
            case BaseDefenceStage.Shoot:
                m_ChangeFromShootAction?.Invoke();
            break;
            case BaseDefenceStage.SwitchWeapon:
                m_ChangeFromSwitchWeaponAction?.Invoke();
            break;
            case BaseDefenceStage.Reload:
                m_ChangeFromReloadAction?.Invoke();
            break;
            default:
            break;
        }

        switch (newStage)
        {
            case BaseDefenceStage.Shoot:
                m_ChangeToShootAction?.Invoke();
            break;
            case BaseDefenceStage.SwitchWeapon:
                m_ChangeToSwitchWeaponAction?.Invoke();
            break;
            case BaseDefenceStage.Reload:
                m_ChangeToReloadAction?.Invoke();
            break;
            default:
            break;
        }

        m_GameStage = newStage;
    }

    public GunModelComtroller GetGunModelController(){
        return m_GunModelController;
    }

    public void GameOver(bool isLose = false){
        ChangeGameStage(BaseDefenceStage.Result);
        //m_BaseDefenceResultPanel.ShowResult(isLose);
    }

    public float GetAccruacy(){
        return m_CurrentAccruacy;
    }
    public void SetAccruacy(float newAccuracy){
        // acc cannot be lower than handling
        float handling = (float) System.Convert.ToSingle(m_GunShootController.GetSelectedGun().GetStatValue(GunScriptableStatEnum.Handling));
        var acc = Mathf.Max( (float) System.Convert.ToSingle(m_GunShootController.GetSelectedGun().GetStatValue(GunScriptableStatEnum.Accuracy)),handling);
        m_CurrentAccruacy = Mathf.Clamp(newAccuracy, handling , acc);
    }

    public GunShootController GetGunShootController(){
        return m_GunShootController;
    }

    public void OnPlayerHit(float damage){
        if(m_GameStage == BaseDefenceStage.Result){
            // game over already
            return;
        }
        m_TotalHpBarStayTime = 4;
        ChangeHp(-damage);
        //float CurHp = MainGameManager.GetInstance().GetCurHp();
        //m_Controller.ChangeHp(-damage);
        m_BaseDefenceUIController.SetHpUI();
        if(m_CurrentHp<=0){
            // lose 
            m_GameStage = BaseDefenceStage.Result;
            m_BaseDefenceUIController.SetResultPanel(false);
        }
        
        /*
        if(CurHp<=0)
            GameOver(true);*/
    }

    public void DoneSwitchWeapon(){
        m_CameraController.CameraLookUp(m_BaseDefenceUIController.OnClickCloseSwitchWeaponPanel );
    }


    public void StartReload(GunReloadControllerConfig gunReloadConfig){
        m_ReloadControllerPanel.SetActive(true);
        m_ReloadController.StartReload( gunReloadConfig );
    }

    public void CloseReloadPanel(){
        m_ReloadControllerPanel.SetActive(false);

    }


    public Animator GetCurrentGunAnimator(){
        return m_GunModelController.GetCurrentGunAnimator();
    }

    public ParticleSystem GetCurrentGunMuzzelPartical(){
        return m_GunModelController.GetCurrentGunMuzzelPartical();
    }

    public void SwitchSelectedWeapon(int slotIndex){
        List<GunScriptable> selectedGunlist = MainGameManager.GetInstance().GetAllSelectedWeapon();
        GunScriptable gun;
        if(slotIndex >= selectedGunlist.Count || selectedGunlist[slotIndex] == null){
            // no selected weapon
            return;
        }
        gun = selectedGunlist[slotIndex];
        m_GunModelController.ChangeGunModel(gun);
        m_GunShootController.SetSelectedGun(gun, slotIndex);
    }

}
