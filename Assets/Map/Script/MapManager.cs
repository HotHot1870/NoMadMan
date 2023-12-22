using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager m_Instance = null;
    [SerializeField] private MapFreeCameraController m_MapFreeCameraController;
    [SerializeField] private MapUIController m_MapUIController;
    [SerializeField] private Transform m_MapLocationParent;
    private MapLocationController m_LocationController = null;
    [SerializeField] private float m_LocationToVehicleMaxDistance = 3f;
    [SerializeField] private Transform m_MapCameraPrent;


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
        // look at location if defence end
        var targetLocation = MainGameManager.GetInstance().GetSelectedLocation();
        if(targetLocation != null){
            m_MapCameraPrent.position = new Vector3(
                targetLocation.Pos.x,
                m_MapCameraPrent.position.y,
                targetLocation.Pos.z
            );
        }
    }

    public void ShowLocationDetail(){
        m_MapUIController.ShowLocationDetail();
    }

    public bool ShouldShowLocationDetail(){
        return m_MapUIController.ShouldShowLocationDetail();
    }


    public void SetLocation(MapLocationController mapLocationController)
    {
        m_LocationController = mapLocationController;
    }

    public MapLocationController GetLocationController(){
        return m_LocationController;
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

    public void ShowEndDefenceDialog(){
        m_MapUIController.OnEndDefenceShowDialog();
    }

    public void SpawnAllLocation()
    {
        foreach (var item in MainGameManager.GetInstance().GetAllLocation())
        {
            var newLocation = Instantiate(item.Prefab, m_MapLocationParent);
            newLocation.transform.position = item.Pos;
            var locationController = newLocation.GetComponent<MapLocationController>();
            locationController.SetScriptable(item);
        }
    }
}
