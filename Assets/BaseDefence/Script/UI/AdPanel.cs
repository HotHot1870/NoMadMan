using System.Collections;
using System.Collections.Generic;
using ExtendedButtons;
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
    [SerializeField] private List<AdSliderAndBtn> m_AllSlider = new List<AdSliderAndBtn>();
    [SerializeField] private Button2D m_SkipBtn;
    [SerializeField] private GameObject m_TotalGainPanel;
    [SerializeField] private AnimationCurve m_Curve;
    private int m_ShowSliderIndex = -1;
    private int m_PlayerHitCount = 0;
    private float m_TimePass = 0;
    private float m_OneLoopTimeNeed = 2f;

    private void Init(){
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

    private void ShowNextSlider(){
        m_ShowSliderIndex++;
        if(m_ShowSliderIndex>=m_AllSlider.Count){
            // no more slider to show 
            m_TotalGainPanel.SetActive(true);
            return;
        }
        m_TimePass = 0;
        m_OneLoopTimeNeed = Random.Range(2f,4f-m_PlayerHitCount*0.5f);
        m_AllSlider[m_ShowSliderIndex].Slider.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        // TODO : for test , remove later
        Init();



        for (int i = 0; i < m_AllSlider.Count; i++)
        {
            int index = i ;
            m_AllSlider[index].StopBtn.onClick.AddListener(()=>OnClickStopSlider(m_AllSlider[index].Slider));
        }
    }


    private void OnClickStopSlider(Slider slider){
        // slider stop 
        if(slider.normalizedValue >= 0.4f && slider.normalizedValue <=0.6f ){
            // hit
            m_PlayerHitCount++;
            Debug.Log("Hit");
        }

        ShowNextSlider();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_ShowSliderIndex>=m_AllSlider.Count){
            // no more slider to show 
            return;
        }
        m_TimePass += Time.deltaTime;
        m_AllSlider[m_ShowSliderIndex].Slider.normalizedValue = m_Curve.Evaluate( (m_TimePass%m_OneLoopTimeNeed)/m_OneLoopTimeNeed );
    }
}
