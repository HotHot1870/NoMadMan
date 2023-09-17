using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using BaseDefenceNameSpace;
using System;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineBrain m_CameraBrain;

    [SerializeField] private CinemachineVirtualCamera m_ShootCamera;
    [SerializeField] private CinemachineVirtualCamera m_SwitchWeaponCamera;
    [SerializeField] private float m_CrosshairAffection=0.5f;
    private Vector3 m_ShootCameraStartPos;



    private void Start()
    {
        m_ShootCameraStartPos = m_ShootCamera.transform.position;

        SetAllCameraPriorityToZero();
        m_ShootCamera.Priority = 1;
    }


    public void CameraLookUp(Action UIUpdate){
        if (m_CameraBrain.IsBlending)
            return;

        SetAllCameraPriorityToZero();
        m_ShootCamera.Priority = 1;
        
        UIUpdate?.Invoke();

        StartCoroutine(SetGameStage(m_CameraBrain.m_DefaultBlend.m_Time,BaseDefenceStage.Shoot));
    }


    public void CameraLookDown(Action UIUpdate){
        if (m_CameraBrain.IsBlending)
            return;

        SetAllCameraPriorityToZero();
        m_SwitchWeaponCamera.Priority = 1;
        
        UIUpdate?.Invoke();

        StartCoroutine(SetGameStage(m_CameraBrain.m_DefaultBlend.m_Time,BaseDefenceStage.SwitchWeapon));
    }

    private void SetAllCameraPriorityToZero(){
        m_ShootCamera.Priority = 0;
        m_SwitchWeaponCamera.Priority = 0;
    }

    public void ShootCameraMoveByCrosshair(Vector2 crosshairPosNormalized){
        m_ShootCamera.transform.position = m_ShootCameraStartPos + new Vector3(
            crosshairPosNormalized.x,
            crosshairPosNormalized.y,
            0) * m_CrosshairAffection;
    }

    private IEnumerator SetGameStage(float waitTime, BaseDefenceStage gameStage)
    {
        yield return new WaitForSeconds(waitTime);
        
        BaseDefenceManager.GetInstance().ChangeGameStage(gameStage);
    }
}
