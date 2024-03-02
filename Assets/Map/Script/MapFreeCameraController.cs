using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MapToOtherLevelBtnStage{
    NoShow = 0,
    ToLast ,
    ToNext
}

public class MapFreeCameraController : MonoBehaviour
{
    [SerializeField] private Transform m_CameraParent;
    [SerializeField][Range(0.5f,5f)] private float m_Sensitivity = 1f;
    [SerializeField] private Vector2 m_CameraBottomLeft;
    [SerializeField] private Vector2 m_CameraTopRight;
    private Vector2 m_MousePreviousPos = Vector2.zero;
    private Vector2 m_MouseStartPos = Vector2.zero;
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
        m_MouseStartPos = Input.mousePosition;
        m_IsCameraMoving = true;
    }

    public void OnDragMapBtnUp()
    {
        // click building
        if( Vector3.Distance( m_MouseStartPos , Input.mousePosition ) <5f && MapManager.GetInstance().ShouldShowLocationDetail()){
            Ray ray = Camera.main.ScreenPointToRay(m_MousePreviousPos);
            RaycastHit hitBuilding;
        
            if( Physics.Raycast(ray,out hitBuilding,500, 1<<10) ){
                var mapLocationController = hitBuilding.transform.GetComponent<MapLocationController>();
                if(mapLocationController != null){
                    MapManager.GetInstance().SetLocation( mapLocationController );
                    MapManager.GetInstance().ShowLocationDetail();
                }
            }
        }

        // reset
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
            Mathf.Clamp(m_CameraParent.position.x,m_CameraBottomLeft.x, m_CameraTopRight.x),
            m_CameraParent.position.y,
            Mathf.Clamp(m_CameraParent.position.z,m_CameraBottomLeft.y, m_CameraTopRight.y)
         ) ;

        if(m_CameraParent.position.x>=m_CameraTopRight.x*0.85f){
            // to next 
            MapManager.GetInstance().SetToOtherLevelBtnStage( MapToOtherLevelBtnStage.ToNext);
        }else if(m_CameraParent.position.x<=m_CameraBottomLeft.x*0.85f){
            // to Last
            MapManager.GetInstance().SetToOtherLevelBtnStage( MapToOtherLevelBtnStage.ToLast);
        }else{
            MapManager.GetInstance().SetToOtherLevelBtnStage( MapToOtherLevelBtnStage.NoShow);
        }
    }
}
