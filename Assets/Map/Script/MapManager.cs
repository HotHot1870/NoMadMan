using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager m_Instance = null;
    [SerializeField] private MapFreeCameraController m_MapFreeCameraController;
    [SerializeField] private MapUIController m_MapUIController;
    [SerializeField] private VehicleController m_VehicleController;
    [SerializeField] private Transform m_MapLocationParent;
    private Dictionary<MapLocationScriptable,MapLocationController> m_AllLocation = new Dictionary<MapLocationScriptable,MapLocationController>();
    private MapLocationController m_NearestLocationController = null;
    [SerializeField] private float m_LocationToVehicleMaxDistance = 3f;


    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        SpawnAllLocation();
    }

    public VehicleController GetVehicleController(){
        return m_VehicleController;
    }

    public void SetNearestLocation(Vector3 VehiclePos)
    {
        m_NearestLocationController = null;
        float ansDistance = m_LocationToVehicleMaxDistance;
        foreach (var item in m_AllLocation)
        {
            float distance = Vector3.Distance(item.Key.Pos, VehiclePos);
            if ( distance < ansDistance)
            {
                m_NearestLocationController = item.Value;
            }
        }

    }

    public MapLocationController GetNearestLocationController(){
        return m_NearestLocationController;
    }

    public static MapManager GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new GameObject().AddComponent<MapManager>();
        }
        return m_Instance;
    }

    public MapFreeCameraController GetMapFreeCameraController()
    {
        return m_MapFreeCameraController;
    }

    public MapUIController GetMapUIController()
    {
        return m_MapUIController;
    }

    public void SpawnAllLocation()
    {
        foreach (var item in MainGameManager.GetInstance().GetAllLocation())
        {
            var newLocation = Instantiate(item.Prefab, m_MapLocationParent);
            newLocation.transform.position = item.Pos;
            var locationController = newLocation.GetComponent<MapLocationController>();
            m_AllLocation.Add(item,locationController);
            locationController.SetScriptable(item);
        }
    }
}
