using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;


[System.Serializable]
public class MapEnvironment
{
    public string Stagename;
    public int Level;
    public GameObject Prefeb;
    public Material SkyBox;
}
public class MapManager : MonoBehaviour
{
    public static MapManager m_Instance = null;
    [SerializeField] private MapFreeCameraController m_MapFreeCameraController;
    [SerializeField] private MapUIController m_MapUIController;
    [SerializeField] private LocationPanelController m_LocationPanelController;
    [SerializeField] private Transform m_MapLocationParent;
    [SerializeField] private Transform m_MapEnvironemntParent;
    private MapLocationController m_LocationController = null;
    [SerializeField] private Transform m_MapCameraPrent;
    [SerializeField] private List<MapEnvironment> m_AllEnvironmentPrefab = new List<MapEnvironment>();
    private GameObject m_SpawnedEnvironment = null;
    private ReflectionProbe baker;
    [SerializeField] private AudioSource m_AudioPlayer;
    [SerializeField] private AudioClip m_OnSelectSound;
    [SerializeField] private Color m_RedFog;
    [SerializeField] private Color m_WhiteFog;


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
        MainGameManager.GetInstance().AddNewAudioSource(m_AudioPlayer);
        SpawnAllLocation();
        CameraLookAtSelectedLocation();
    }

    public void CameraLookAtSelectedLocation(){
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
        m_AudioPlayer.PlayOneShot(m_OnSelectSound);
        m_LocationPanelController.Init();
    }

    public bool ShouldShowLocationDetail(){
        return m_LocationPanelController.ShouldShowLocationDetail();
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
            Destroy(m_MapLocationParent.GetChild(i).gameObject);

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
        
        var targetEnvironment = m_AllEnvironmentPrefab.Find(x=>x.Level == selectedLevel);
        if(targetEnvironment == null){
            Debug.LogError($"Level {selectedLevel} Map environment not found ");
            return;
        }
        // Red fog in level 2 and 3 (last 2 level)
        RenderSettings.fogStartDistance = selectedLevel <2?35f:60f;
        RenderSettings.fogEndDistance = selectedLevel <2?100f:125f;
        RenderSettings.skybox= targetEnvironment.SkyBox ;
        
        MainGameManager.GetInstance().SetFog(selectedLevel>=2);

        m_SpawnedEnvironment = Instantiate(targetEnvironment.Prefeb,m_MapEnvironemntParent);
        // change sky box
    }
    /*
    private void ChangeSkyBox() {
        if(baker == null)
            baker = gameObject.AddComponent<ReflectionProbe>();
            
        RenderSettings.skybox = RenderSettings.skybox;
        DynamicGI.UpdateEnvironment();
        baker.cullingMask = 0;
        baker.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
        baker.mode = ReflectionProbeMode.Realtime;
        baker.timeSlicingMode = ReflectionProbeTimeSlicingMode.NoTimeSlicing;

        RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
        StartCoroutine(UpdateEnvironment());
    }

    IEnumerator UpdateEnvironment() {
        DynamicGI.UpdateEnvironment();
        baker.RenderProbe();
        yield return new WaitForEndOfFrame();
        RenderSettings.customReflectionTexture= baker.texture;
    }*/
}
