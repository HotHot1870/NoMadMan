using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [SerializeField] private Transform m_Self;
    [Header("Move")]
    [SerializeField][Range(0.1f, 10f)] private float m_MoveSpeed = 5f;
    [SerializeField] private Transform m_MoveToTarget;
    private Vector3 m_LookTarget;
    [Header("Bounce")]
    [SerializeField][Range(0f, 50f)] private float m_BounceSpeed = 5f;
    [SerializeField][Range(0f, 0.9f)] private float m_BounceAmount = 0.15f;
    [SerializeField] private Transform m_BounceTarget;
    private float m_BounceNormalized = 0;
    private float m_PassedTime = 0;

    private Vector3 m_MousePosOnDown;

    private bool m_CanSetDestination = true;

    private void Start()
    {
        m_MoveToTarget.gameObject.SetActive(false);
    }

    private void Update()
    {
        VehicleBounce();
        MarkMoveToTarget();
        MoveVehicle();
    }

    private void MoveVehicle()
    {
        if (m_MoveToTarget.gameObject.activeSelf && Vector3.Distance(m_Self.position, m_LookTarget) > 0.25f)
        {
            m_Self.position += m_Self.forward * m_MoveSpeed * Time.deltaTime;
            MapManager.GetInstance().SetNearestLocation(m_Self.position);
            if(MapManager.GetInstance().GetNearestLocationController()!= null){
                MapManager.GetInstance().GetMapUIController().ChangeCheckLocationActive(true);
            }else{
                MapManager.GetInstance().GetMapUIController().ChangeCheckLocationActive(false);
            }
        }
    }

    private void MarkMoveToTarget()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // record mouse pos
            m_MousePosOnDown = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            // check if click without drag
            if (Vector3.Distance(m_MousePosOnDown, Input.mousePosition) < 25f && m_CanSetDestination)
            {
                // set move to location        
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                // hit Environment
                if (Physics.Raycast(ray, out hit, 500, 1 << 10))
                {
                    m_MoveToTarget.gameObject.SetActive(true);
                    m_MoveToTarget.position = hit.point;
                    m_LookTarget = new Vector3(
                        m_MoveToTarget.position.x,
                        m_Self.position.y,
                        m_MoveToTarget.position.z
                    );
                    m_Self.LookAt(m_LookTarget);
                }
            }
        }

    }

    public void ChangeMovable(bool canMove){
        m_CanSetDestination = canMove;
    }

    private void VehicleBounce()
    {
        m_PassedTime += (Time.deltaTime * m_BounceSpeed * Mathf.PI);
        m_BounceTarget.localScale = new Vector3(1, 1 - m_BounceAmount * m_BounceNormalized, 1);
        m_BounceNormalized = (Mathf.Sin(m_PassedTime) + 1f) / 2f;

    }

}
