using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;
using UnityEngine.UI;


public class MainGameManager : MonoBehaviour
{
    public static MainGameManager m_Instance = null;
    private List<AudioSource> m_AllAudioSource = new List<AudioSource>();

    [SerializeField] private float m_Volume = 0.75f;
    [SerializeField] private float m_AimSensitivity = 0.5f;

    [SerializeField] private List<DialogScriptable> m_AllDialog = new List<DialogScriptable>();
    [SerializeField] private List<GunScriptable> m_AllWeapon = new List<GunScriptable>();
    [SerializeField] private List<MapLocationScriptable> m_AllLocation = new List<MapLocationScriptable>();
    [SerializeField] private List<WeaponUpgradeScriptable> m_AllWeaponUpgrade = new List<WeaponUpgradeScriptable>();
    [SerializeField] private List<EnemyScriptable> m_AllEnemy = new List<EnemyScriptable>();
    
    [Header("Loading")]
    [SerializeField]private Canvas m_LoadingCanvas;
    [SerializeField]private Image m_LoadAmountImage;
    [SerializeField]private Animator m_BgAnimator;
    [Header("Location")]
    private MapLocationScriptable m_CurlocationData = null;

    
#if UNITY_EDITOR
    [MenuItem("Action/Clear PlayerPref")]
    static void ClearplayerPref()
    {
        PlayerPrefs.DeleteAll();
    }
    [MenuItem("Action/Get Hundred Goo")]
    static void GetGooDebug()
    {
        float curGoo = PlayerPrefs.GetFloat("Goo", 0 );
        PlayerPrefs.SetFloat("Goo",curGoo+1000);
    }
    
    [MenuItem("Scene/MainMenu")]
    static void ToMainMenu()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/MainMenu/MainMenu.unity");
    }

    [MenuItem("Scene/BaseDefence")]
    static void ToBaseDefence()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/BaseDefence/BaseDefence.unity");
    }
    
    [MenuItem("Scene/Map")]
    static void ToMap()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Map/Map.unity");
    }
#endif

    public static MainGameManager GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new GameObject().AddComponent<MainGameManager>();
        }
        return m_Instance;
    }


    private void Start()
    {
		Application.targetFrameRate = 50;
        
        // unlock pistol
        SaveData<int>(m_AllWeapon[0].DisplayName+m_AllWeapon[0].Id.ToString(),1);
        // can use pistol by default , if no gun selected
        if((int)System.Convert.ToSingle(GetData<int>("SelectedWeapon"+0.ToString(), "-1")) < 0)
            SaveData<int>("SelectedWeapon"+0.ToString(),0);
        
        m_BgAnimator.Play("Hidden");
    }

    public void SetSelectedLocation(MapLocationScriptable location){
        m_CurlocationData = location;
    }

    public MapLocationScriptable GetSelectedLocation(){
        return m_CurlocationData;
    }

    public void UnlockAllLevel(){
        foreach (var item in m_AllLocation)
        {
            SaveData<int>(item.DisplayName + item.Id, 1);  
        }
    }

    public void LoadSceneWithTransition(string sceneName,Action doneLoadSenceAction = null){
        m_LoadingCanvas.sortingOrder = 1;
        StartCoroutine(LoadAsync(sceneName,doneLoadSenceAction));
    }

    private IEnumerator LoadAsync(string sceneName,Action doneLoadSenceAction){
        float timePass = 0;
        m_BgAnimator.Play("Open");
        while (timePass<=0.75f)
        {
            timePass += Time.deltaTime;
            yield return null;
        }
        
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone )
        {
            m_LoadAmountImage.fillAmount = Mathf.Clamp01(load.progress/0.9f);
            yield return null;
        }
        m_BgAnimator.Play("Close");
        
        
        yield return null;

        doneLoadSenceAction?.Invoke();
        
    }

    public List<GunScriptable> GetAllWeapon()
    {
        return m_AllWeapon;
    }

    public List<MapLocationScriptable> GetAllLocation(){
        return m_AllLocation;
    }

    public List<GunScriptable> GetAllSelectedWeapon()
    {
        List<GunScriptable> ans = new List<GunScriptable>();
        for (int i = 0; i < 4; i++)
        {
            int gunId = (int)GetData<int>("SelectedWeapon"+i.ToString(),"-1");  
            GunScriptable gunScriptable = m_AllWeapon.Find(x=>x.Id==gunId);
            ans.Add(gunScriptable);
        }
        return ans;
    }

    
    public List<WeaponUpgradeScriptable> GetAllWave() {
        return m_AllWeaponUpgrade;
    }

    
    public List<EnemyScriptable> GetAllEnemy(){
        return m_AllEnemy;
    }
    
    public DialogScriptable GetDialog(int id){
        return m_AllDialog.Find(x=>x.Id == id);
 
    }

    public void SetAllDialog(List<DialogScriptable> allDialog){
        m_AllDialog = allDialog;
 
    }

    public void SetAllWeapon(List<GunScriptable> allWeapon){
        m_AllWeapon = allWeapon;
 
    }

    public void SetAllLocation(List<MapLocationScriptable> allLocation){
        m_AllLocation = allLocation;
    }

    public void SetAllWeaponUpgrade(List<WeaponUpgradeScriptable> allWeaponUpgrade) {
        m_AllWeaponUpgrade = allWeaponUpgrade;
    }

    public void SetAllEnemy(List<EnemyScriptable> allEnemy){
        m_AllEnemy = allEnemy;
    }


    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }



    public void UnlockGun(int id){
        var targetGun = m_AllWeapon.Find(x=>x.Id==id);

        SaveData<int>(targetGun.DisplayName+targetGun.Id.ToString(),1);
    }


    public void ChangeGooAmount(float gooChanges){
        var curGoo = (float)GetData<float>("Goo") ;
        SaveData<float>("Goo",curGoo+gooChanges);
    }

    public float GetGooAmount(){
        return (float)GetData<float>("Goo") ;
    }

    public void SetBaseDefenceScene(MapLocationScriptable location){
        LoadSceneWithTransition("BaseDefence", 
            ()=>BaseDefenceManager.GetInstance().StartWave(location));
    }

    private IEnumerator StartWave(MapLocationScriptable location){
        yield return null;

    }

    public void SetMapScene(){
        LoadSceneWithTransition("Map");
    }




    public void SetAimSensitivity(float sensitivity)
    {
        m_AimSensitivity = sensitivity;
    }

    public void SetVolume(float volume)
    {
        m_Volume = volume;
    }
    public float GetVolume()
    {
        return m_Volume;
    }
    public float GetAimSensitivity()
    {
        return m_AimSensitivity;
    }

    public void AddNewAudioSource(AudioSource audioSource)
    {
        m_AllAudioSource.Add(audioSource);
        audioSource.volume = m_Volume;

        UpdateVolume();
    }

    public void UpdateVolume()
    {
        List<int> toBeRemove = new List<int>();
        for (int i = 0; i < m_AllAudioSource.Count; i++)
        {
            if (m_AllAudioSource[i] != null)
            {
                m_AllAudioSource[i].volume = m_Volume;
            }
            else
            {
                toBeRemove.Add(i);
            }
        }
        for (int i = 0; i < toBeRemove.Count; i++)
        {
            m_AllAudioSource.RemoveAt(toBeRemove[i] - i);
        }
    }



    public void SaveData<T>(string key, T value)
    {
        if(typeof(T) == typeof(int)){
            PlayerPrefs.SetInt(key, Convert.ToInt32(value) );
        }else if(typeof(T) == typeof(float)){
            PlayerPrefs.SetFloat(key, (float)Convert.ToDouble(value) );
        }
        else if(typeof(T) == typeof(string)){
            PlayerPrefs.SetString(key,value.ToString());
        }
    }

    
    public object GetData<T>(string key, string defaultValue = null)
    {
        if(typeof(T) == typeof(int)){
            int baseIntValue = defaultValue != null? int.Parse(defaultValue) : 0;
            return PlayerPrefs.GetInt(key, baseIntValue );
        }else if(typeof(T) == typeof(float)){
            float baseFloatValue = defaultValue != null? float.Parse(defaultValue) : 0;
            return PlayerPrefs.GetFloat(key, baseFloatValue );
        }
        else if(typeof(T) == typeof(string)){
            return PlayerPrefs.GetString(key, "");
        }
        return "";
    }

}
