using System.Collections;
using System.Collections.Generic;
using ExtendedButtons;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

[System.Serializable]
public class AdSliderAndBtn
{
    public Slider Slider;
    public Button2D StopBtn;
}

public class AdPanel : MonoBehaviour , IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private List<AdSliderAndBtn> m_AllSlider = new List<AdSliderAndBtn>();
    [SerializeField] private Button2D m_SkipBtn;
    [SerializeField] private Button2D m_ClaimBtn;
    [SerializeField] private GameObject m_TotalGainPanel;
    [SerializeField] private AnimationCurve m_Curve;
    [SerializeField] private TMP_Text m_TotalGainText;
    [SerializeField] private RectTransform m_Ring;
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_Hit;
    [SerializeField] private AudioClip m_Miss;
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    private int m_ShowSliderIndex = -1;
    private int m_PlayerHitCount = 0;
    private float m_TimePass = 0;
    private float m_OneLoopTimeNeed = 2f;
    private float m_BaseGain = 0;
    private string m_ADUnitId = null; // This will remain null for unsupported platforms

    public void Init(float baseGain){
        Advertisement.Load(m_ADUnitId, this);
        m_Self.SetActive(true);
        m_Ring.sizeDelta = Vector2.zero;
        m_BaseGain = baseGain;
        m_PlayerHitCount = 0;
        m_ShowSliderIndex = -1;
        m_TimePass = 0;
        m_OneLoopTimeNeed = 2f;
        m_TotalGainPanel.SetActive(false);
        foreach (var item in m_AllSlider)
        {
            item.Slider.gameObject.SetActive(false);
        }
        ShowNextSlider();
    }
    
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
    #if UNITY_EDITOR
        Debug.Log("Ad Loaded: " + adUnitId);
 
    #endif
        if (adUnitId.Equals(m_ADUnitId))
        {
            m_ClaimBtn.onClick.AddListener(ShowAd);
        }
    }

    public void ShowAd()
    {
        m_ClaimBtn.interactable = false;
        if((int)MainGameManager.GetInstance().GetData<int>("AD")==1){
            // Then show the ad:
            Advertisement.Show(m_ADUnitId, this);
        }else{
            MainGameManager.GetInstance().ChangeGooAmount(m_BaseGain);
        }
        MainGameManager.GetInstance().LoadSceneWithTransition("Map",()=>MapManager.GetInstance().ShowEndDefenceDialog());
        MainGameManager.GetInstance().ChangeBGM(BGM.Map);
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(m_ADUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            // Grant a reward.
            MainGameManager.GetInstance().ChangeGooAmount(m_BaseGain);
        }
    }
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }
 
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }
 
    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
 

    private void OnClickSkip(){
        MainGameManager.GetInstance().LoadSceneWithTransition("Map",()=>MapManager.GetInstance().ShowEndDefenceDialog());
        MainGameManager.GetInstance().ChangeBGM(BGM.Map);
    }

    private void ShowNextSlider(){
        m_ShowSliderIndex++;
        if(m_ShowSliderIndex>=m_AllSlider.Count){
            // no more slider to show 
            m_TotalGainPanel.SetActive(true);
            m_TotalGainText.text = "Extra : "+ m_BaseGain*m_PlayerHitCount;
            MainGameManager.GetInstance().ChangeGooAmount(m_BaseGain*m_PlayerHitCount);
            return;
        }
        m_TimePass = 0;
        m_OneLoopTimeNeed = Random.Range(2f,4f-m_PlayerHitCount*0.5f);
        m_AllSlider[m_ShowSliderIndex].Slider.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_IOS
        m_ADUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        m_ADUnitId = _androidAdUnitId;
#endif
        MainGameManager.GetInstance().AddNewAudioSource(m_AudioSource);
        MainGameManager.GetInstance().AddOnClickBaseAction(m_SkipBtn, m_SkipBtn.GetComponent<RectTransform>());
        MainGameManager.GetInstance().AddOnClickBaseAction(m_ClaimBtn,m_ClaimBtn.GetComponent<RectTransform>());
        m_SkipBtn.onClick.AddListener(OnClickSkip);
        for (int i = 0; i < m_AllSlider.Count; i++)
        {
            int index = i ;
            m_AllSlider[index].StopBtn.onClick.AddListener(()=>OnClickStopSlider(m_AllSlider[index].Slider));
        }

        m_Self.SetActive(false);
    }


    private void OnClickStopSlider(Slider slider){
        // slider stop 
        bool isHit = false;
        m_AudioSource.clip = m_Miss;
        if(slider.normalizedValue >= 0.4f && slider.normalizedValue <=0.6f ){
            // hit
            m_PlayerHitCount++;
            m_AudioSource.clip = m_Hit;
            isHit = true;
        }
        m_AudioSource.Play();
        StartCoroutine(RingEffect(isHit, slider.GetComponent<RectTransform>().anchoredPosition));

        ShowNextSlider();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_ShowSliderIndex>=m_AllSlider.Count || m_ShowSliderIndex <0){
            // no more slider to show 
            return;
        }
        m_TimePass += Time.deltaTime;
        m_AllSlider[m_ShowSliderIndex].Slider.normalizedValue = m_Curve.Evaluate( (m_TimePass%m_OneLoopTimeNeed)/m_OneLoopTimeNeed );
    }

    private IEnumerator RingEffect(bool isHit, Vector2 pos){
        float passTime = 0;
        float duration = isHit?0.15f:0.35f;
        m_Ring.GetComponent<Image>().color = isHit?Color.green:Color.red;
        m_Ring.anchoredPosition = pos;
        m_Ring.sizeDelta = Vector2.one * 800f;
        while (passTime<duration)
        {
            passTime += Time.deltaTime;
            m_Ring.sizeDelta = Vector2.Lerp(Vector2.one * 800f , Vector2.zero, passTime/duration);
            yield return null;
        }
    }
}
