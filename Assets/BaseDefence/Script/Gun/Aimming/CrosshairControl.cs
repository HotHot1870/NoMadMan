using System.Collections;
using System.Collections.Generic;
using BaseDefenceNameSpace;
using UnityEngine;

public class CrosshairControl : MonoBehaviour
{
    [SerializeField] private RectTransform m_Top;
    [SerializeField] private RectTransform m_Left;
    [SerializeField] private RectTransform m_Right;
    [SerializeField] private RectTransform m_Down;
    [SerializeField] private RectTransform m_CrosshairParent;
    [SerializeField] private float m_CrosshairAffectGunPosStrength=0.5f;
 
    private Vector2 m_AimDragMouseStartPos = Vector2.zero;
    private Vector2 m_AimDragMouseEndPos = Vector2.zero;
    private Vector3 m_CrossHairDragStartPos;
    private Vector3 m_MousePreviousPos = Vector3.zero;
    private bool m_IsCrosshairMoving = false;

    private Vector2 m_CrosshairToScreenOffsetNormalized = Vector2.zero;


    public float m_MaxAccuracyLose = 150f;


    public void OnAimBtnDown(){
        m_IsCrosshairMoving = true;
        m_MousePreviousPos = Input.mousePosition;
        m_AimDragMouseStartPos = Input.mousePosition;
        m_CrossHairDragStartPos = m_CrosshairParent.position;
    }

    public void OnAimBtnUp(){
        m_IsCrosshairMoving = false;
        m_AimDragMouseStartPos = Vector2.zero;
        m_MousePreviousPos = Vector3.zero;
        m_CrossHairDragStartPos = m_CrosshairParent.position;
    }


    private void Start() {
        BaseDefenceManager.GetInstance().m_ShootUpdateAction += OnShootUpdate;
    }

    public void OnShootUpdate() {
        if(BaseDefenceManager.GetInstance().GameStage == BaseDefenceStage.Result){
            // game over already
            return;
        }
        var curAcc = BaseDefenceManager.GetInstance().GetAccruacy();
        if(m_IsCrosshairMoving){
            OnCrosshairMove();
            curAcc = BaseDefenceManager.GetInstance().GetAccruacy();
        }else if( curAcc < 100 ){
            curAcc = GainAccOvertime(curAcc);
        }
        
        BaseDefenceManager.GetInstance().SetAccruacy( curAcc );
        SetCrosshairAccuracy(m_MaxAccuracyLose * ( 1 - Mathf.InverseLerp(0f,100f, BaseDefenceManager.GetInstance().GetAccruacy() )) );
    }

    private float GainAccOvertime(float curAcc){
        
        if(BaseDefenceManager.GetInstance().GetGunShootController().GetShootCoolDown()>0){
            // gain less acc on shoot cool down
            curAcc += Time.deltaTime*50f;
        }else{
            curAcc += Time.deltaTime*120f;
        }
        return curAcc;
    }

    public Vector3 GetCrosshairPos(){
        return m_CrosshairParent.position;
    }

    public void OnCrosshairMove(){
        var curAcc = BaseDefenceManager.GetInstance().GetAccruacy();

        m_AimDragMouseEndPos = Input.mousePosition;
        Vector3 offset = MainGameManager.GetInstance().GetAimSensitivity() * (m_AimDragMouseEndPos - m_AimDragMouseStartPos) * 3;
        m_CrosshairParent.position = m_CrossHairDragStartPos + offset;

        OutOffBountPrevention();
        // accrucy lose for moving
        float mouseCurToPassDiatance = Vector3.Distance(m_MousePreviousPos,m_AimDragMouseEndPos);

        if (mouseCurToPassDiatance <=0f)
        {
            // draging but not moving , gain accruacy over time
            curAcc = GainAccOvertime(curAcc);
        }
        else
        {
            m_CrosshairToScreenOffsetNormalized = new Vector2(
                    (m_CrosshairParent.position.x - (Screen.width/2f) ) /Screen.width,
                    (m_CrosshairParent.position.y - (Screen.height/2f) ) /Screen.height
                ) * m_CrosshairAffectGunPosStrength ;
            BaseDefenceManager.GetInstance().GetCameraController().ShootCameraMoveByCrosshair(m_CrosshairToScreenOffsetNormalized);
            curAcc -= Time.deltaTime*mouseCurToPassDiatance*20f;
            m_MousePreviousPos = m_AimDragMouseEndPos;
        }
        BaseDefenceManager.GetInstance().SetAccruacy(curAcc);
    }

    public Vector2 GetCrosshairToScreenOffsetNormalized(){
        return m_CrosshairToScreenOffsetNormalized;
    }

    private void OutOffBountPrevention(){
        float border = 80f;
        m_CrosshairParent.position = new Vector3(
            Mathf.Clamp(m_CrosshairParent.position.x, border, Screen.width-border - border),
            Mathf.Clamp(m_CrosshairParent.position.y, border, Screen.height-border - border),
            0
            );
    }

    public void SetCrosshairRecoil(Vector2 recoil){
        m_AimDragMouseStartPos += recoil;
    }

    public void SetCrosshairAccuracy(float accuracyLose){
        accuracyLose = Mathf.Clamp(accuracyLose,0f,m_MaxAccuracyLose);
        m_Top.localPosition = new Vector3(0,accuracyLose,0);
        m_Left.localPosition = new Vector3(-accuracyLose,0,0);
        m_Right.localPosition = new Vector3(accuracyLose,0,0);
        m_Down.localPosition = new Vector3(0,-accuracyLose,0);

    }

}
