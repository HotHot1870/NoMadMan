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
 
    private Vector2 m_AimDragTouchStartPos = Vector2.zero;
    private Vector2 m_AimDragTouchEndPos = Vector2.zero;
    private Vector3 m_CrossHairDragStartPos;
    private Vector3 m_AimTouchPreviousPos = Vector3.zero;
    private bool m_IsCrosshairMoving = false;
    private int m_TouchIndex = -1;

    private Vector2 m_CrosshairToScreenOffsetNormalized = Vector2.zero;


    public float m_MaxAccuracyLose = 100f;


    public void OnAimBtnDown(){
        m_IsCrosshairMoving = true;
        m_CrossHairDragStartPos = m_CrosshairParent.position;

#if UNITY_EDITOR
        m_AimTouchPreviousPos = Input.mousePosition;
        m_AimDragTouchStartPos = Input.mousePosition;
#endif
    }

    public void OnAimBtnUp(){
        m_IsCrosshairMoving = false;
        m_TouchIndex = -1;
        m_AimDragTouchStartPos = Vector2.zero;
        m_AimTouchPreviousPos = Vector3.zero;
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
            curAcc += Time.deltaTime*25f;
        }else{
            curAcc += Time.deltaTime*50f;
        }
        return curAcc;
    }

    public Vector3 GetCrosshairPos(){
        return m_CrosshairParent.position;
    }

    public void OnCrosshairMove(){
        
#if !UNITY_EDITOR
        if(m_TouchIndex<0){
            m_TouchIndex = Input.touchCount-1;
            if(m_TouchIndex<0){
                // no touch detected
                return;
            }
            m_AimTouchPreviousPos = Input.GetTouch(m_TouchIndex).position;
            m_AimDragTouchStartPos = Input.GetTouch(m_TouchIndex).position;
        }
#endif

        var curAcc = BaseDefenceManager.GetInstance().GetAccruacy();

#if UNITY_EDITOR
    m_AimDragTouchEndPos = Input.mousePosition;
#endif

#if !UNITY_EDITOR
        m_AimDragTouchEndPos = Input.GetTouch(m_TouchIndex).position; 
#endif
        Vector3 offset = MainGameManager.GetInstance().GetAimSensitivity() * (m_AimDragTouchEndPos - m_AimDragTouchStartPos) * 3;
        m_CrosshairParent.position = m_CrossHairDragStartPos + offset;

        OutOffBountPrevention();
        // accrucy lose for moving
        float mouseCurToPassDiatance = Vector3.Distance(m_AimTouchPreviousPos,m_AimDragTouchEndPos);

            // draging but almost not moving , gain accruacy over time
            curAcc = GainAccOvertime(curAcc);
            m_CrosshairToScreenOffsetNormalized = new Vector2(
                    (m_CrosshairParent.position.x - (Screen.width/2f) ) /Screen.width,
                    (m_CrosshairParent.position.y - (Screen.height/2f) ) /Screen.height
                ) ;
            BaseDefenceManager.GetInstance().GetCameraController().ShootCameraMoveByCrosshair(m_CrosshairToScreenOffsetNormalized);
        BaseDefenceManager.GetInstance().SetAccruacy(curAcc);
    }

    public Vector2 GetCrosshairToScreenOffsetNormalized(){
        return m_CrosshairToScreenOffsetNormalized;
    }

    private void OutOffBountPrevention(){
        float border = 80f;
        m_CrosshairParent.position = new Vector3(
            Mathf.Clamp(m_CrosshairParent.position.x, Screen.width - Screen.height*16f/9f +border , Screen.height*16f/9f-border ),
            Mathf.Clamp(m_CrosshairParent.position.y, border, Screen.height - border),
            0
            );
    }
/*
    public void SetCrosshairRecoil(Vector2 recoil){
        m_AimDragTouchStartPos += recoil;
    }*/

    public void SetCrosshairAccuracy(float accuracyLose){
        accuracyLose = Mathf.Clamp(accuracyLose,0f,m_MaxAccuracyLose);
        m_Top.localPosition = new Vector3(0,accuracyLose,0);
        m_Left.localPosition = new Vector3(-accuracyLose,0,0);
        m_Right.localPosition = new Vector3(accuracyLose,0,0);
        m_Down.localPosition = new Vector3(0,-accuracyLose,0);

    }

}
