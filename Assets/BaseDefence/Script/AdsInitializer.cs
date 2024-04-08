using UnityEngine;
using UnityEngine.Advertisements;
 
public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string _androidGameId = "5589303";
    [SerializeField] string _iOSGameId = "5589302";
    [SerializeField] bool _testMode = true;
    private string _gameId;
    public bool m_IsLoadAdSuccess = true;
 
    void Awake()
    {
        InitializeAds();
    }
 
    public void InitializeAds()
    {
    #if UNITY_IOS
            _gameId = _iOSGameId;
    #elif UNITY_ANDROID
            _gameId = _androidGameId;
    #elif UNITY_EDITOR
            _gameId = _androidGameId; //Only for testing the functionality in the Editor
    #endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

 
    public void OnInitializationComplete()
    {
    #if UNITY_EDITOR
        Debug.Log("Unity Ads initialization complete.");
    #endif
    }
 
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        m_IsLoadAdSuccess = false;
    #if UNITY_EDITOR
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    #endif
    }
}