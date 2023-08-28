using System.Collections;
using System.Collections.Generic;
using ExtendedButtons;
using UnityEngine;
using Cinemachine;
using BaseDefenseNameSpace;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Button2D m_LookDownBtn;
    [SerializeField] private Button2D m_LookUpBtn;
    [SerializeField] private GameObject m_ShootPanel;
    [SerializeField] private CinemachineBrain m_CameraBrain;

    [SerializeField] private CinemachineVirtualCamera m_ShootCamera;
    [SerializeField] private CinemachineVirtualCamera m_SwitchWeaponCamera;
    private Vector3 m_ShootCameraStartPos;



    private void Start()
    {
        BaseDefenseManager.GetInstance().m_SwitchWeaponUpdateAction += SwitchWeaponUpdate;
        m_ShootCameraStartPos = m_ShootCamera.transform.position;

        m_LookDownBtn.onDown.AddListener(() =>
        {
            if (m_CameraBrain.IsBlending)
                return;

            SetAllCameraPriorityToZero();
            m_SwitchWeaponCamera.Priority = 1;
            
            m_ShootPanel.SetActive(false);
            m_LookUpBtn.gameObject.SetActive(true);

            SetGameStage(m_CameraBrain.m_DefaultBlend.m_Time,BaseDefenseStage.SwitchWeapon);
        });

        m_LookUpBtn.onDown.AddListener(() =>
        {
            if (m_CameraBrain.IsBlending)
                return;

            SetAllCameraPriorityToZero();
            m_ShootCamera.Priority = 1;
            
            m_ShootPanel.SetActive(true);
            m_LookUpBtn.gameObject.SetActive(false);

            SetGameStage(m_CameraBrain.m_DefaultBlend.m_Time,BaseDefenseStage.Shoot);


        });

        SetAllCameraPriorityToZero();
        m_ShootCamera.Priority = 1;


        m_LookUpBtn.gameObject.SetActive(false);
    }

    private void SetAllCameraPriorityToZero(){
        m_ShootCamera.Priority = 0;
        m_SwitchWeaponCamera.Priority = 0;
    }

    public void ShootCameraMoveByCrosshair(Vector2 crosshairPosNormalized){
        m_ShootCamera.transform.position = m_ShootCameraStartPos + new Vector3(
            crosshairPosNormalized.x,
            crosshairPosNormalized.y,
            0);
    }

    public void SwitchWeaponUpdate()
    {
        // can switch weapon when camera done blending 
        if (!m_CameraBrain.IsBlending)
        {
            SwitchWeaponInputHandler();
        }
    }

    private void SwitchWeaponInputHandler()
    {
        // select weapon
        if (Input.GetMouseButtonDown(0) && !m_ShootPanel.activeSelf )
        {

            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            for (int i = 0; i < hits.Length; i++)
            {
                hits[i].collider.TryGetComponent<WeaponToBeSwitch>(out var weaponToBeSwitch);
                if (weaponToBeSwitch != null && weaponToBeSwitch.m_Gun != null)
                {
                    SwitchWeapon(weaponToBeSwitch.m_Gun, weaponToBeSwitch.m_SlotIndex);
                    m_LookUpBtn.onDown.Invoke();
                    return;
                }
            }
        }
    }

    private IEnumerator SetGameStage(float waitTime, BaseDefenseStage gameStage)
    {
        yield return new WaitForSeconds(waitTime);
        BaseDefenseManager.GetInstance().ChangeGameStage(gameStage);
    }

    private void SwitchWeapon(GunScriptable gun, int slotIndex)
    {
        BaseDefenseManager.GetInstance().SwitchSelectedWeapon(gun, slotIndex);
        SetGameStage(m_CameraBrain.m_DefaultBlend.m_Time,BaseDefenseStage.Shoot);
    }
}
