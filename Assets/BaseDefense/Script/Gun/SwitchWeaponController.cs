using System.Collections;
using System.Collections.Generic;
using ExtendedButtons;
using UnityEngine;
using EZCameraShake;
using BaseDefenseNameSpace;

public class SwitchWeaponController : MonoBehaviour
{
    [SerializeField] private Button2D m_LookDownBtn;
    [SerializeField] private Button2D m_LookUpBtn;
    [SerializeField] private Transform m_CameraParent;
    [SerializeField] private CameraShaker m_CameraShaker;
    [SerializeField] private GameObject m_ShootPanel;
    [SerializeField][Range(0.1f,1.5f)] private float m_LookDownTime = 0.3f;
    private float m_TimePassAfterLookDownNormalized = 0;
    private bool m_IsLookUp = false;
    private Vector3 m_LooedDownCameraPos = new Vector3(0,-15.5f,0);
    

    private void Start() {
        BaseDefenseManager.GetInstance().m_SwitchWeaponUpdateAction += SwitchWeaponUpdate;
        BaseDefenseManager.GetInstance().m_ChangeToSwitchWeaponAction += ChangeGameStageToSwitchWeapon;
        BaseDefenseManager.GetInstance().m_ChangeFromSwitchWeaponAction += ChangeGameStageFromSwitchweapon;


        m_LookDownBtn.onDown.AddListener(()=>{
            BaseDefenseManager.GetInstance().ChangeGameStage(BaseDefenseStage.SwitchWeapon);
        });

        m_LookUpBtn.onDown.AddListener(()=>{ 
            // cannot leave switch weapon panel while looking up / down           
            if(m_TimePassAfterLookDownNormalized < 1)
                return;

            m_IsLookUp = true;
            m_TimePassAfterLookDownNormalized = 1;
            m_CameraParent.position = m_LooedDownCameraPos;
        });

        m_LookUpBtn.gameObject.SetActive(false);
    }    

    private void ChangeGameStageToSwitchWeapon(){
        m_IsLookUp = false;
        m_CameraShaker.enabled = false;
        m_CameraParent.position = new Vector3(0,0,0);
        m_ShootPanel.SetActive(false);
        m_LookUpBtn.gameObject.SetActive(true);
        m_TimePassAfterLookDownNormalized = 0;
    }

    private void ChangeGameStageFromSwitchweapon(){
        m_LookUpBtn.gameObject.SetActive(false);
        m_ShootPanel.SetActive(true);
        m_CameraShaker.enabled = true;
        m_CameraParent.position = new Vector3(0,0,0);
    }
    
    public void SwitchWeaponUpdate() {

        if(m_IsLookUp){        
            LookingUp();

        }else{
            LookingDown();
            SwitchWeaponInputHandler();
        }
    }

    private void SwitchWeaponInputHandler(){
        // select weapon
        if(Input.GetMouseButtonDown(0) && !m_ShootPanel.activeSelf && m_TimePassAfterLookDownNormalized>1 ){
            
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            for (int i = 0; i < hits.Length; i++)
            {
                hits[i].collider.TryGetComponent<WeaponToBeSwitch>(out var weaponToBeSwitch);         
                if(weaponToBeSwitch != null && weaponToBeSwitch.m_Gun != null ){
                    SwitchWeapon(weaponToBeSwitch.m_Gun,weaponToBeSwitch.m_SlotIndex);
                    m_LookUpBtn.onDown.Invoke();
                    return;
                }
            }
        }
    }

    private void LookingUp(){
        if(m_TimePassAfterLookDownNormalized>0 ){
            m_CameraParent.position = Vector3.Lerp(new Vector3(0,0,0) , m_LooedDownCameraPos , m_TimePassAfterLookDownNormalized);
            m_TimePassAfterLookDownNormalized-=Time.deltaTime/m_LookDownTime;
        }else{
            BaseDefenseManager.GetInstance().ChangeGameStage(BaseDefenseStage.Shoot);
        }
    }

    private void LookingDown(){
        if(m_TimePassAfterLookDownNormalized<1 ){
            m_CameraParent.position = Vector3.Lerp(new Vector3(0,0,0) , m_LooedDownCameraPos , m_TimePassAfterLookDownNormalized);
            m_TimePassAfterLookDownNormalized+=Time.deltaTime/m_LookDownTime;

        }
    }

    private void SwitchWeapon(GunScriptable gun, int slotIndex){
        BaseDefenseManager.GetInstance().SwitchSelectedWeapon(gun, slotIndex);
    }
}
