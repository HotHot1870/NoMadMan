using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFreeCameraController : MonoBehaviour
{
    [SerializeField] private Transform m_CameraParent;
    [SerializeField] private Transform m_TopRight;
    [SerializeField] private Transform m_BottomLeft;
    [SerializeField][Range(0.5f,5f)] private float m_Sensitivity = 1f;
    //private Vector2 m_AimDragMouseStartPos = Vector2.zero;
    //private Vector2 m_AimDragMouseEndPos = Vector2.zero;
    private Vector2 m_MousePreviousPos = Vector2.zero;
    private bool m_IsCameraMoving = false;
    private Vector3 m_CameraDragStartPos;


    private void Start()
    {
        m_CameraDragStartPos = m_CameraParent.position;
    }

    private void Update()
    {
        if (m_IsCameraMoving)
        {
            // move free camera
            MoveFreeCamera();
        }
    }

    public void OnDragMapBtnDown()
    {
        m_MousePreviousPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        //m_AimDragMouseStartPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        m_CameraDragStartPos = m_CameraParent.position;
        m_IsCameraMoving = true;
    }

    public void OnDragMapBtnUp()
    {
        m_IsCameraMoving = false;
        //m_AimDragMouseStartPos = Vector2.zero;
        m_MousePreviousPos = Vector2.zero;
        m_CameraDragStartPos = m_CameraParent.position;
    }

    private void MoveFreeCamera()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        float screenSlopeLength = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height );
        float mouseToStartDistance = Vector2.Distance(mousePos, m_MousePreviousPos) / screenSlopeLength * m_Sensitivity*100f ;
        Vector2 direction = (m_MousePreviousPos - mousePos ).normalized;
        Vector3 directionVThree = new Vector3(direction.x,0,direction.y);
        m_CameraParent.position = m_CameraDragStartPos + directionVThree * mouseToStartDistance;
         CameraOutOfBoundPrevention();
    }

    private void CameraOutOfBoundPrevention(){
        m_CameraParent.position = new Vector3(
            Mathf.Clamp(m_CameraParent.position.x,m_TopRight.position.x,m_BottomLeft.position.x),
            m_CameraParent.position.y,
            Mathf.Clamp(m_CameraParent.position.z,m_TopRight.position.z,m_BottomLeft.position.z)
         ) ;
    }
}