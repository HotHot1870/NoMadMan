using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class MapEnvironment
{
    public string Stagename;
    public int Level;
    public GameObject Prefeb;
}
public class MapManager : MonoBehaviour
{
    public static MapManager m_Instance = null;
    [SerializeField] private MapFreeCameraController m_MapFreeCameraController;
    [SerializeField] private MapUIController m_MapUIController;
    [SerializeField] private Transform m_MapLocationParent;
    [SerializeField] private Transform m_MapEnvironemntParent;
    private MapLocationController m_LocationController = null;
    [SerializeField] private float m_LocationToVehicleMaxDistance = 3f;
    [SerializeField] private Transform m_MapCameraPrent;
    // TODO : set environment by level
    [SerializeField] private List<MapEnvironment> m_AllEnvironmentPrefab = new List<MapEnvironment>();
    private GameObject m_SpawnedEnvironment = null;


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

    public void SetToOtherLevelBtnStage(MapToOtherLevelBtnStage btnStage){
        m_MapUIController.SetToOtherLevelBtn( btnStage);
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
        // spawn location
        int spawnedlocationCount = m_MapLocationParent.childCount;
        for (int i = 0; i < spawnedlocationCount; i++)
        {
            Destroy(m_MapLocationParent.GetChild(0).gameObject);

        }
        int selectedLevel = MainGameManager.GetInstance().GetSelectedLocation().Level;
        foreach (var item in MainGameManager.GetInstance().GetAllLocation())
        {
            if(selectedLevel != item.Level)
                continue;

            var newLocation = Instantiate(item.Prefab, m_MapLocationParent);
            newLocation.transform.position = item.Pos;
            var locationController = newLocation.GetComponent<MapLocationController>();
            locationController.SetScriptable(item);
        }

        // spawn environment
        if(m_SpawnedEnvironment != null){
            Destroy(m_SpawnedEnvironment);
        }
        m_SpawnedEnvironment = Instantiate(m_AllEnvironmentPrefab.Find(x=>x.Level == selectedLevel).Prefeb,m_MapEnvironemntParent);
    }
}
