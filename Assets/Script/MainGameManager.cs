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

    [SerializeField] private List<GunScriptable> m_AllSelectedWeapon = new List<GunScriptable>();

    [SerializeField] private List<GunScriptable> m_AllWeapon = new List<GunScriptable>();
    [SerializeField] private List<MapLocationScriptable> m_AllLocation = new List<MapLocationScriptable>();
    [SerializeField] private List<WeaponUpgradeScriptable> m_AllWeaponUpgrade = new List<WeaponUpgradeScriptable>();
    [SerializeField] private List<EnemyScriptable> m_AllEnemy = new List<EnemyScriptable>();
    
    // TODO : Reset on start defence , put hp in base defence manager , not here
    [SerializeField]private float m_CurrentHp = 1000;
    [SerializeField]private float m_MaxHp = 1000;

    [Header("Loading")]
    [SerializeField]private Canvas m_LoadingCanvas;
    [SerializeField]private Image m_LoadAmountImage;
    [SerializeField]private Animator m_BgAnimator;

    
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
        
        for (int i = 0; i < 4; i++)
        {
            SaveData<int>(m_AllWeapon[i].DisplayName+m_AllWeapon[i].Id.ToString(),1);
        }
        
        m_BgAnimator.Play("Hidden");
    }

    public void UnlockAllLevel(){
        foreach (var item in m_AllLocation)
        {
            SaveData<int>(item.DisplayName + item.Id, 1);  
        }
    }

    public void LoadSceneWithTransition(string sceneName, Action onFinishAction = null){
        m_LoadingCanvas.sortingOrder = 1;
        StartCoroutine(LoadAsync(sceneName,onFinishAction));
    }

    private IEnumerator LoadAsync(string sceneName, Action onFinishAction = null){
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
        onFinishAction?.Invoke();
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
        if(m_AllSelectedWeapon.Count>4){
            m_AllSelectedWeapon = new List<GunScriptable>(m_AllSelectedWeapon.GetRange(0,4));
        }
        return m_AllSelectedWeapon;
    }

    
    public List<WeaponUpgradeScriptable> GetAllWave() {
        return m_AllWeaponUpgrade;
    }

    
    public List<EnemyScriptable> GetAllEnemy(){
        return m_AllEnemy;
    }

    public void SetAllWeapon(List<GunScriptable> allWeapon){
        // TODO : check Ownership
        m_AllWeapon = allWeapon;
        m_AllSelectedWeapon.Clear();

        for (int i = 0; i < m_AllWeapon.Count; i++)
        {
            
            string gunUnlockKey = m_AllWeapon[i].DisplayName+m_AllWeapon[i].Id;

            if((int)System.Convert.ToSingle(GetData<int>(gunUnlockKey)) == 1){
                m_AllSelectedWeapon.Add(m_AllWeapon[i]);
            }
            if(m_AllSelectedWeapon.Count>=4)
                break;
        } 
        if(m_AllSelectedWeapon.Count<4){
            Debug.Log("Not enough owned gun");
        }
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

    public float GetCurHp(){
        return m_CurrentHp;
    }

    public float GetMaxHp(){
        return m_MaxHp;
    }

    public void SetMapScene(){
        LoadSceneWithTransition("Map");
    }

    public void ChangeHp(float changes){
        m_CurrentHp += changes;
        if(m_CurrentHp<0){
            m_CurrentHp = 0;
        }else if(m_CurrentHp>m_MaxHp){
            m_CurrentHp = m_MaxHp;
        }
    }



    public void ChangeSelectedWeapon(int slotIndex, GunScriptable newWeapon)
    {
        if (m_AllSelectedWeapon.Count > slotIndex && slotIndex >= 0)
            m_AllSelectedWeapon[slotIndex] = newWeapon;
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

    
    public object GetData<T>(string key)
    {
        if(typeof(T) == typeof(int)){
            return PlayerPrefs.GetInt(key, 0 );
        }else if(typeof(T) == typeof(float)){
            return PlayerPrefs.GetFloat(key, 0 );
        }
        else if(typeof(T) == typeof(string)){
            return PlayerPrefs.GetString(key, "");
        }
        return 0;
    }

}
