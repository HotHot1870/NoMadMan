using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GunModelComtroller : MonoBehaviour
{
    [SerializeField] private Transform m_ModelParent;
    [SerializeField] private Vector3 m_CrosshairOffsetStrength = Vector3.one;
    private GameObject m_GunModel;

    private Vector3 m_ModelStartPos;

    private Vector3 m_PosOffset = Vector3.zero;

    private void Start() {
        BaseDefenseManager.GetInstance().m_ChangeToShootAction += ShowFPSGunModel;
        BaseDefenseManager.GetInstance().m_ChangeFromShootAction += HideFPSGunModel;
        BaseDefenseManager.GetInstance().m_ShootUpdateAction += ShootUpdate;
        m_ModelStartPos = m_ModelParent.position;
    }

    private void ShootUpdate() {
        
        Ray ray = Camera.main.ScreenPointToRay(BaseDefenseManager.GetInstance().GetCrosshairPos());
        RaycastHit hit;
        // hit Environment
        if (Physics.Raycast(ray, out hit, 100, 1<<10))
        {
            m_ModelParent.LookAt(hit.point);
        }

        GunModelParentOffsetHandler();

    }

    private void HideFPSGunModel(){
        m_ModelParent.gameObject.SetActive(false);
    }

    private void ShowFPSGunModel(){
        m_ModelParent.gameObject.SetActive(true);
    }

    public void ChangeGunModel(GunScriptable gun){
        if(m_GunModel != null){
            Destroy(m_GunModel);
        }
        m_GunModel = Instantiate(gun.FPSPrefab,m_ModelParent);
        var gunTrans = m_GunModel.transform;
        m_PosOffset = gun.FPSPos;
        gunTrans.localEulerAngles = gun.FPSRot;
        gunTrans.localScale = gun.FPSScale;
    }

    private void GunModelParentOffsetHandler(){
        GunModelOffset(BaseDefenseManager.GetInstance().GetCrosshairController().GetCrosshairToScreenOffsetNormalized());

    }

    private void GunModelOffset(Vector2 crosshairPosNormalized){
        // x offset 
        var startPos = new Vector3(
            m_ModelStartPos.x + Mathf.Abs(crosshairPosNormalized.x) *3f,
            m_ModelStartPos.y,
            m_ModelStartPos.z
        );

        m_ModelParent.position = startPos + new Vector3(
            crosshairPosNormalized.x * m_CrosshairOffsetStrength.x,
            crosshairPosNormalized.y * m_CrosshairOffsetStrength.y,
            crosshairPosNormalized.y*-1f * m_CrosshairOffsetStrength.z
        ) ;
        m_ModelParent.localPosition += m_PosOffset;

    }

}

