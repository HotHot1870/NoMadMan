using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using BaseDefenceNameSpace;
using System;

public class CameraController : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera m_ShootCamera;
    [SerializeField] private float m_CrosshairAffection=0.5f;
    private Vector3 m_ShootCameraStartPos;



    private void Start()
    {
        m_ShootCameraStartPos = m_ShootCamera.transform.position;
    }


    public void ShootCameraMoveByCrosshair(Vector2 crosshairPosNormalized){
        
        m_ShootCamera.transform.position = m_ShootCameraStartPos + new Vector3(
            crosshairPosNormalized.x,
            crosshairPosNormalized.y,
            0) * m_CrosshairAffection;
    }

}
