using System.Collections;
using System.Collections.Generic;
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

        float tmp = 50f;

    private void Start() {
        m_AimBtn.onDown.AddListener(() =>
        {
            m_MousePreviousPos = Input.mousePosition;
            m_AimDragMouseStartPos = Input.mousePosition;
            m_CrossHairDragStartPos = m_CrosshairParent.position;
        });
        m_AimBtn.onUp.AddListener(() =>
        {
            m_AimDragMouseStartPos = Vector2.zero;
            m_MousePreviousPos = Vector3.zero;
        });

        m_AimBtn.onExit.AddListener(() =>
        {
            m_AimDragMouseStartPos = Vector2.zero;
            m_MousePreviousPos = Vector3.zero;
            m_CrossHairDragStartPos = m_CrosshairParent.position;
        });
    }

    private void Update() {
        tmp = Time.deltaTime*25f + tmp;
        if(tmp>100f){
            tmp = 0;
        }

        SetCrosshairByRecoil(tmp);
        //OnCrosshairMove();
    }
    public void OnCrosshairMove(){
        m_AimDragMouseEndPos = Input.mousePosition;

        Vector3 offset = MainGameManager.GetInstance().GetAimSensitivity() * (m_AimDragMouseEndPos - m_AimDragMouseStartPos) * 3;
        m_CrosshairParent.position = m_CrossHairDragStartPos + offset;

        // accrucy lose for moving
        /*
        float mouseMoveAmound = Vector3.Distance(m_MousePreviousPos, mousePos) /
            (Mathf.Sqrt(Screen.height * Screen.height + Screen.width * Screen.width) / 2) * 1000;*/
/*
        if (mouseMoveAmound == 0)
        {
            // draging but not moving , gain accruacy over time
            AccruacyGainOvertime();
        }
        else
        {
            m_CurrentAccruacy -= Time.deltaTime * mouseMoveAmound * (100 - m_SelectedGun.Stability) * 0.75f;
            m_MousePreviousPos = mousePos;
        }*/

        //CrossHairOutOfBoundPrevention();

        // light follow crossHair
        //var crossHairworldPos = Camera.main.ScreenToWorldPoint(m_CrossHair.position);
        OutOffBountPrevention();
    }

    private void OutOffBountPrevention(){
        float border = 80f;
        m_CrosshairParent.position = new Vector3(
            Mathf.Clamp(m_CrosshairParent.position.x, border, Screen.width-border - border),
            Mathf.Clamp(m_CrosshairParent.position.y, border, Screen.height-border - border),
            0
            );
    }



    public void SetCrosshairByRecoil(float recoil){
        m_Top.localPosition = new Vector3(0,recoil,0);
        m_Left.localPosition = new Vector3(-recoil,0,0);
        m_Right.localPosition = new Vector3(recoil,0,0);
        m_Down.localPosition = new Vector3(0,-recoil,0);

    }

}
