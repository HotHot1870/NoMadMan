using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GunModelComtroller : MonoBehaviour
{
    [SerializeField] private Transform m_ModelAim;
    [SerializeField] private Transform m_ModelShake;
    [SerializeField] private Vector2 m_CrosshairOffsetStrength = Vector2.one;
    private GameObject m_GunModel;
    private Vector3 m_ModelStartPos;
    private Vector3 m_PosOffset = Vector3.zero;
    private Coroutine m_ShakeRecover = null;
    private Vector3 m_ModelAimStartRotation ; 



    private void Start() {
        BaseDefenceManager.GetInstance().m_ChangeToShootAction += ShowFPSGunModel;
        BaseDefenceManager.GetInstance().m_ChangeFromShootAction += HideFPSGunModel;
        BaseDefenceManager.GetInstance().m_ShootUpdateAction += ShootUpdate;
    }

    private void ShootUpdate() {

        GunModelParentOffsetHandler();

    }

    public void ShakeGunByShoot(){
        // TODO : Shake animation
        /*
        if(m_ShakeRecover != null){
            StopCoroutine(m_ShakeRecover);
        }
        m_ShakeRecover = StartCoroutine(ShakeGun(shakeAmount));*/
    }

    public void HideFPSGunModel(){
        m_ModelAim.gameObject.SetActive(false);
    }

    private void ShowFPSGunModel(){
        m_ModelAim.gameObject.SetActive(true);
    }

    public void ChangeGunModel(GunScriptable gun){
        
        m_ModelAim.localPosition = Vector3.zero;
        m_ModelAim.localEulerAngles = Vector3.zero;
        if(m_GunModel != null){
            Destroy(m_GunModel);
        }
        m_GunModel = Instantiate(gun.FPSPrefab,m_ModelShake);
        var gunTrans = m_GunModel.transform;
        
        m_ModelAim.localPosition = m_GunModel.transform.localPosition;
        m_GunModel.transform.localPosition = Vector3.zero;
        
        m_ModelAim.localEulerAngles = m_GunModel.transform.localEulerAngles;
        m_ModelAimStartRotation = m_GunModel.transform.localEulerAngles;
        m_GunModel.transform.localEulerAngles = Vector3.zero;
        
    }

    private void GunModelParentOffsetHandler(){
        GunModelOffset(BaseDefenceManager.GetInstance().GetCrosshairController().GetCrosshairToScreenOffsetNormalized());

    }

    private void GunModelOffset(Vector2 crosshairPosNormalized){
        // x offset 
        m_ModelAim.localEulerAngles = m_ModelAimStartRotation + new Vector3(
            crosshairPosNormalized.y * -m_CrosshairOffsetStrength.x,
            crosshairPosNormalized.x * m_CrosshairOffsetStrength.y,
            0
        ) ;
    }

}

