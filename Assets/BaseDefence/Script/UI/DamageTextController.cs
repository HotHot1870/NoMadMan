using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageTextController : MonoBehaviour
{
    [SerializeField] private TMP_Text m_Text;
    [SerializeField] private AnimationCurve m_Curve;
    [SerializeField] private float m_ExistTime = 1f;
    [SerializeField] private RectTransform m_Self;
    private float m_TimePass = 0;
    private float m_RandomUp = 100f;
    private float m_RandomLeftRight = 100f;

    // Start is called before the first frame update
    void Start()
    {
        m_RandomUp = Random.Range(50f,150f);
        m_RandomLeftRight = Random.Range(-50f,50f);
        m_Text.alpha = 1;
        m_Self.anchoredPosition += Vector2.up * Random.Range(-50f,50f);
        m_Self.anchoredPosition += Vector2.right * Random.Range(-50f,50f);
    }
    void Update(){
        m_TimePass += Time.deltaTime;
        m_Text.alpha = m_Curve.Evaluate((m_ExistTime-m_TimePass)/m_ExistTime);
        if(m_TimePass >=5f){
            Destroy(this.gameObject);
        }
        m_Self.anchoredPosition += Vector2.up * m_RandomUp * Time.deltaTime;
        m_Self.anchoredPosition += Vector2.right * m_RandomLeftRight * Time.deltaTime;
    }
}
