using System.Collections;
using System.Collections.Generic;
using BaseDefenseNameSpace;
using ExtendedButtons;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairControl : MonoBehaviour
{
    [SerializeField] private RectTransform m_Top;
    [SerializeField] private RectTransform m_Left;
    [SerializeField] private RectTransform m_Right;
    [SerializeField] private RectTransform m_Down;
    [SerializeField] private RectTransform m_CrosshairParent;
 
    [SerializeField] private Button2D m_AimBtn;    
    private Vector2 m_AimDragMouseStartPos = Vector2.zero;
    private Vector2 m_AimDragMouseEndPos = Vector2.zero;
    private Vector3 m_CrossHairDragStartPos;
    private Vector3 m_MousePreviousPos = Vector3.zero;
    private bool m_IsCrosshairMoving = false;


    private void Start() {
        m_AimBtn.onDown.AddListener(() =>
        {
            m_IsCrosshairMoving = true;
            m_MousePreviousPos = Input.mousePosition;
            m_AimDragMouseStartPos = Input.mousePosition;
            m_CrossHairDragStartPos = m_CrosshairParent.position;
        });
        m_AimBtn.onUp.AddListener(() =>
        {
            m_IsCrosshairMoving = false;
            m_AimDragMouseStartPos = Vector2.zero;
            m_MousePreviousPos = Vector3.zero;
            m_CrossHairDragStartPos = m_CrosshairParent.position;
        });

        m_AimBtn.onExit.AddListener(() =>
        {
            m_IsCrosshairMoving = false;
            m_AimDragMouseStartPos = Vector2.zero;
            m_MousePreviousPos = Vector3.zero;
            m_CrossHairDragStartPos = m_CrosshairParent.position;
        });
    }

    private void Update() {
        if(BaseDefenseManager.GetInstance().GameStage == BaseDefenseStage.Result){
            // game over already
            return;
        }
        var curAcc = BaseDefenseManager.GetInstance().GetAccruacy();
        if(m_IsCrosshairMoving){
            OnCrosshairMove();
            curAcc = BaseDefenseManager.GetInstance().GetAccruacy();
        }else if( curAcc < 100 ){
            curAcc += Time.deltaTime*120f;
        }
        
        BaseDefenseManager.GetInstance().SetAccruacy( curAcc );

        SetCrosshairAccuracy(100f-curAcc);
    }
    public void OnCrosshairMove(){
        var curAcc = BaseDefenseManager.GetInstance().GetAccruacy();

        m_AimDragMouseEndPos = Input.mousePosition;
        Vector3 offset = MainGameManager.GetInstance().GetAimSensitivity() * (m_AimDragMouseEndPos - m_AimDragMouseStartPos) * 3;
        m_CrosshairParent.position = m_CrossHairDragStartPos + offset;

        OutOffBountPrevention();
        // accrucy lose for moving
        float mouseCurToPassDiatance = Vector3.Distance(m_MousePreviousPos,m_AimDragMouseEndPos);

        if (mouseCurToPassDiatance <=0f)
        {
            // draging but not moving , gain accruacy over time
            curAcc += Time.deltaTime*120f;
        }
        else
        {
            curAcc -= Time.deltaTime*mouseCurToPassDiatance*20f;
            m_MousePreviousPos = m_AimDragMouseEndPos;
        }
        BaseDefenseManager.GetInstance().SetAccruacy(curAcc);
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
        accuracyLose = Mathf.Clamp(accuracyLose,0f,100f);
        m_Top.localPosition = new Vector3(0,accuracyLose,0);
        m_Left.localPosition = new Vector3(-accuracyLose,0,0);
        m_Right.localPosition = new Vector3(accuracyLose,0,0);
        m_Down.localPosition = new Vector3(0,-accuracyLose,0);

    }

}
