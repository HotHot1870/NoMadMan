using System.Collections;
using System.Collections.Generic;
using ExtendedButtons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AdSliderAndBtn
{
    public Slider Slider;
    public Button2D StopBtn;
}

public class AdPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private List<AdSliderAndBtn> m_AllSlider = new List<AdSliderAndBtn>();
    [SerializeField] private Button2D m_SkipBtn;
    [SerializeField] private GameObject m_TotalGainPanel;
    [SerializeField] private AnimationCurve m_Curve;
    [SerializeField] private TMP_Text m_TotalGainText;
    [SerializeField] private RectTransform m_Ring;
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_Hit;
    [SerializeField] private AudioClip m_Miss;
    private int m_ShowSliderIndex = -1;
    private int m_PlayerHitCount = 0;
    private float m_TimePass = 0;
    private float m_OneLoopTimeNeed = 2f;
    private float m_BaseGain = 0;

    public void Init(float baseGain){
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
        MainGameManager.GetInstance().AddNewAudioSource(m_AudioSource);
        MainGameManager.GetInstance().AddOnClickBaseAction(m_SkipBtn, m_SkipBtn.GetComponent<RectTransform>());
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
