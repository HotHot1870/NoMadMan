using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;
using UnityEngine.UI;
using ExtendedButtons;
using UnityEngine.Rendering;


public enum BGM
{
    None = 0,
    MainMenu,
    Map,
    Battle,
    Defeated
}

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager m_Instance = null;
    private List<AudioSource> m_AllAudioSource = new List<AudioSource>();

    private float m_Volume = 0.75f;
    private float m_AimSensitivity = 0.5f;

    [SerializeField] private List<DialogScriptable> m_AllDialog = new List<DialogScriptable>();
    [SerializeField] private List<GunScriptable> m_AllWeapon = new List<GunScriptable>();
    [SerializeField] private List<MapLocationScriptable> m_AllLocation = new List<MapLocationScriptable>();
    [SerializeField] private List<WeaponUpgradeScriptable> m_AllWeaponUpgrade = new List<WeaponUpgradeScriptable>();
    [SerializeField] private List<EnemyScriptable> m_AllEnemy = new List<EnemyScriptable>();
    
    [Header("Loading")]
    [SerializeField]private Canvas m_LoadingCanvas;
    [SerializeField]private Image m_LoadAmountImage;
    [SerializeField]private Animator m_BgAnimator;
        
    [Header("Click sound")]
    [SerializeField] private AudioSource m_AudioPlayer;
    [SerializeField] private AudioClip m_OnClickStartSound;
    [SerializeField] private AudioClip m_OnClickEndSound;
        
    [Header("BGM")]
    [SerializeField] private AudioClip m_MapBGM;
    [SerializeField] private AudioClip m_MainBGM;
    [SerializeField] private AudioClip m_BattleBGM;
    [SerializeField] private AudioClip m_DefeatedBGM;
    [SerializeField] private AudioSource m_BGMplayer;
    private float m_BGMVolume = 0.75f;
    private List<AudioSource> m_AllBGMAudioSource = new List<AudioSource>();

    [Header("Fog")]
    [SerializeField] private Color m_RedFog;
    [SerializeField] private Color m_WhiteFog;
    private ReflectionProbe baker;
    [Header("AD")]
    [SerializeField] private AdsInitializer m_AdsInitializer;

    
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
        PlayerPrefs.SetFloat("Goo",curGoo+10000);
    }

    [MenuItem("Scene/EndGame")]
    static void ToEndGame()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/EndGame/EndGame.unity");
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

    public void AddOnClickBaseAction(Button2D btn,RectTransform rectTransform = null){
        btn.onDown.RemoveAllListeners();
        btn.onDown.AddListener(()=>{
                OnClickStartSound(rectTransform);
            });

        btn.onUp.RemoveAllListeners();
        btn.onUp.AddListener(()=>{
            OnClickEndSound(rectTransform);
        });
    }

    public void InitAd(){
        m_AdsInitializer.InitializeAds();
    }

    public bool IsAdLoaded(){
        return m_AdsInitializer.m_IsLoadAdSuccess;
    }

    private void OnClickStartSound(RectTransform rectTransform = null){
        m_AudioPlayer.PlayOneShot(m_OnClickStartSound);
        if(rectTransform != null){
            rectTransform.localScale = Vector3.one * 0.9f;
        }
    }

    private void OnClickEndSound(RectTransform rectTransform = null){
        m_AudioPlayer.PlayOneShot(m_OnClickEndSound);
        if(rectTransform != null){
            rectTransform.localScale = Vector3.one * 1f;
        }
    }

    private void PlayBGM(AudioClip clip){
        m_BGMplayer.volume = m_BGMVolume;
        if(clip != null){
            m_BGMplayer.clip = clip;
            m_BGMplayer.Play();
        }else{
            m_BGMplayer.Stop();
        }
    }

    private void Start()
    {
        PlayBGM(m_MainBGM);
        if((float)GetData<float>("AimSensitivity") != 0){
            m_AimSensitivity = (float)GetData<float>("AimSensitivity");
        }else{
            SaveData<float>("AimSensitivity",0.5f);
        }
        if((float)GetData<float>("Volume") != 0){
            m_Volume = (float)GetData<float>("Volume");
        }else{
            SaveData<float>("Volume",0.5f);
        }
        if((float)GetData<float>("BGMVolume") != 0){
            m_BGMVolume = (float)GetData<float>("BGMVolume");
        }else{
            SaveData<float>("BGMVolume",0.5f);
        }
		Application.targetFrameRate = 45;
        
        // unlock pistol
        SaveData<int>("WeaponUnlock"+m_AllWeapon[0].Id.ToString(),1);
        // can use pistol by default , if no gun selected
        if((int)System.Convert.ToSingle(GetData<int>("SelectedWeapon"+0.ToString(), "-1")) < 0)
            SaveData<int>("SelectedWeapon"+0.ToString(),0);
        
        m_BgAnimator.Play("Hidden");
        AddNewAudioSource(m_AudioPlayer);
        AddNewBGMAudioSource(m_BGMplayer);
    }

    public void SetSelectedLocation(MapLocationScriptable location){
        SaveData<int>("SelectedLocation",location.Id);
    }

    public MapLocationScriptable GetSelectedLocation(){
        int locationId = (int)System.Convert.ToSingle(GetData<int>("SelectedLocation","0"));
        return m_AllLocation.Find(x=>x.Id == locationId);
    }

    public void UnlockAllLevel(){
        foreach (var item in m_AllLocation)
        {
            SaveData<int>("Win"+ item.Id, 1);  
        }
    }

    public void LoadSceneWithTransition(string sceneName,Action doneLoadsceneAction = null){
        m_LoadingCanvas.sortingOrder = 1;
        StartCoroutine(LoadAsync(sceneName,doneLoadsceneAction));
    }

    private IEnumerator LoadAsync(string sceneName,Action doneLoadsceneAction){
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

        doneLoadsceneAction?.Invoke();
        
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

        SaveData<int>("WeaponUnlock"+targetGun.Id.ToString(),1);
    }


    public void ChangeGooAmount(float gooChanges){
        var curGoo = (float)GetData<float>("Goo") ;
        SaveData<float>("Goo",curGoo+gooChanges);
    }

    public float GetGooAmount(){
        return (float)GetData<float>("Goo") ;
    }

    public void SetBaseDefenceScene(MapLocationScriptable location){
        ChangeBGM(BGM.Battle);
        LoadSceneWithTransition("BaseDefence", 
            ()=>{
                BaseDefenceManager.GetInstance().StartWave(location);
                
                // fog
                SetFog(location.Level>=2);
            });

    }

    public void SetFog(bool isRed){
        RenderSettings.fogColor = isRed?m_RedFog:m_WhiteFog;
        ChangeSkyBox();
    }
    
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
    }

    private IEnumerator StartWave(MapLocationScriptable location){
        yield return null;

    }

    public void SetMapScene(){
        ChangeBGM(BGM.Map);
        LoadSceneWithTransition("Map");
    }


    public void ChangeBGM(BGM bgm,float fadeOutTime = 1f){
        StartCoroutine(FadeOutAndInBGM(bgm, fadeOutTime));
    }

    private IEnumerator FadeOutAndInBGM(BGM bgm,float fadeOutTime){
        float duration = fadeOutTime;
        float passTime = 0;
        while (passTime <duration)
        {
            passTime+=Time.deltaTime;
            m_BGMplayer.volume = Mathf.Lerp(m_BGMVolume,0,passTime/duration);
            yield return null;
        }

        switch (bgm)
        {
            case BGM.MainMenu:
                PlayBGM(m_MainBGM);
            break;
            case BGM.Map:
                PlayBGM(m_MapBGM);
            break;
            case BGM.Battle:
                PlayBGM(m_BattleBGM);
            break;
            case BGM.Defeated:
                PlayBGM(m_DefeatedBGM);
            break;
            case BGM.None:
                PlayBGM(null);
            break;
            default:
            break;
        }
    }

    public void SetAimSensitivity(float sensitivity)
    {
        m_AimSensitivity = sensitivity;
        SaveData<float>("AimSensitivity",sensitivity);
    }


    public void SetBGMVolume(float volume)
    {
        m_BGMVolume = volume;
        SaveData<float>("BGMVolume",volume);
    }

    public float GetBGMVolume()
    {
        if((float)GetData<float>("BGMVolume","-1") != -1){
            m_BGMVolume = (float)GetData<float>("BGMVolume");
        }else{
            SaveData<float>("BGMVolume",0.5f);
        }
        return m_BGMVolume;
    }

    public void SetSoundVolume(float volume)
    {
        m_Volume = volume;
        SaveData<float>("Volume",volume);
    }
    public float GetVolume()
    {
        if((float)GetData<float>("Volume","-1") != -1){
            m_Volume = (float)GetData<float>("Volume");
        }else{
            SaveData<float>("Volume",0.5f);
        }
        return m_Volume;
    }
    public float GetAimSensitivity()
    {
        if((float)GetData<float>("AimSensitivity","-1") != -1){
            m_AimSensitivity = (float)GetData<float>("AimSensitivity");
        }else{
            SaveData<float>("AimSensitivity",0.6f);
        }
        return m_AimSensitivity;
    }


    public void AddNewAudioSource(AudioSource audioSource)
    {
        if(audioSource == null){
            return;
        }
        m_AllAudioSource.Add(audioSource);
        audioSource.volume = m_Volume;

        UpdateSoundVolume();
    }

    public void UpdateBGMVolume()
    {
        // remove deleted audio Source
        List<int> toBeRemove = new List<int>();
        for (int i = 0; i < m_AllBGMAudioSource.Count; i++)
        {
            if (m_AllBGMAudioSource[i] != null)
            {
                m_AllBGMAudioSource[i].volume = m_BGMVolume;
            }
            else
            {
                toBeRemove.Add(i);
            }
        }
        for (int i = 0; i < toBeRemove.Count; i++)
        {
            m_AllBGMAudioSource.RemoveAt(toBeRemove[i] - i);
        }
    }

    public void AddNewBGMAudioSource(AudioSource audioSource)
    {
        m_AllBGMAudioSource.Add(audioSource);
        audioSource.volume = m_BGMVolume;

        UpdateSoundVolume();
    }

    public void UpdateSoundVolume()
    {
        // remove deleted audio Source
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
