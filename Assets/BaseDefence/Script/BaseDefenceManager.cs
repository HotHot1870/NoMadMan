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

    private float m_CurrentAccruacy = 100f;

    private bool m_IsWin = false;


    [Header("Enemy Hp Bars")]
    [SerializeField] private Transform m_EnemyHpBarParent;
    public Transform EnemyHpBarParent { get { return m_EnemyHpBarParent; } }

    //[Header("Wall")]
    //[SerializeField] private WallController m_WallController;
    private float m_TotalWallHpBarStayTime = 0;

    
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
        //m_WallController.Init(m_WallController.GetWallMaxHp());

        m_ChangeFromReloadAction += CloseReloadPanel;
        //m_WallController.m_HpBarFiller.fillAmount = MainGameManager.GetInstance().GetWallCurHp() / MainGameManager.GetInstance().GetWallMaxHp();


    }


    public List<Transform> GetAllEnemyTrans(){
        return m_EnemySpawnController.GetAllEnemyTrans();
    }
    public void AddEnemyToList(Transform trans){
        m_EnemySpawnController.AddEnemyToList(trans);
    }

    public void RemoveDeadEnemyFromList(Transform trans){
        m_EnemySpawnController.RemoveDeadEnemyFromList(trans);
    }

    public void StartWave(MapLocationScriptable locationInfo){

        m_EnemySpawnController.StartWave( MainGameManager.GetInstance().GetAllWave()[locationInfo.WaveId]);
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
        // wall hp bar stay time
        if(m_TotalWallHpBarStayTime>0){
            m_TotalWallHpBarStayTime -= Time.deltaTime;
        }else{
            m_BaseDefenceUIController.WallUISetActive(false);
        }
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
        m_CurrentAccruacy = Mathf.Clamp(newAccuracy, m_GunShootController.GetSelectedGun().GunStats.Handling , m_GunShootController.GetSelectedGun().GunStats.Accuracy);
    }

    public GunShootController GetGunShootController(){
        return m_GunShootController;
    }

    public void OnWallHit(float damage){
        if(m_GameStage == BaseDefenceStage.Result){
            // game over already
            return;
        }
        m_TotalWallHpBarStayTime = 4;
        MainGameManager.GetInstance().ChangeWallHp(-damage);
        //float wallCurHp = MainGameManager.GetInstance().GetWallCurHp();
        //m_WallController.ChangeHp(-damage);
        m_BaseDefenceUIController.SetWallHpUI();
        if(MainGameManager.GetInstance().GetWallCurHp()<=0){
            // lose 
            m_GameStage = BaseDefenceStage.Result;
            m_BaseDefenceUIController.SetResultPanel(false);
        }
        
        /*
        if(wallCurHp<=0)
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
        GunScriptable gun = MainGameManager.GetInstance().GetAllSelectedWeapon()[slotIndex];
        m_GunModelController.ChangeGunModel(gun);
        m_GunShootController.SetSelectedGun(gun, slotIndex);
    }

}
