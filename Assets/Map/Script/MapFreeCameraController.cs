using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFreeCameraController : MonoBehaviour
{
    [SerializeField] private Transform m_CameraParent;
    private Vector2 m_AimDragMouseStartPos = Vector2.zero;
    private Vector2 m_AimDragMouseEndPos = Vector2.zero;
    private Vector3 m_MousePreviousPos = Vector3.zero;
    private bool m_IsCameraMoving = false;
    private Vector3 m_CameraDragStartPos;

    public void OnDragMapBtnDown(){
        m_IsCameraMoving = true;
        m_MousePreviousPos = Input.mousePosition;
        m_AimDragMouseStartPos = Input.mousePosition;
        m_CameraDragStartPos = m_CameraParent.position;
    }

    public void OnDragMapBtnUp(){
        m_IsCameraMoving = false;
        m_AimDragMouseStartPos = Vector2.zero;
        m_MousePreviousPos = Vector3.zero;
        m_CameraDragStartPos = m_CameraParent.position;
    }
}
