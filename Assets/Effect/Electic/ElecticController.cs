using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ElecticController : MonoBehaviour
{
    [SerializeField] private Transform m_StartPos;
    [SerializeField] private Transform m_PosTwo;
    [SerializeField] private Transform m_PosThree;
    [SerializeField] private Transform m_EndPos;

    [SerializeField] private Vector2 m_StartCurve;
    [SerializeField] private Vector2 m_EndCurve;
    [SerializeField] private VisualEffect m_VFX;

    [Header("Play One Shot")]
    [SerializeField][Range(0f,1f)] private float m_Normalized=0.3f;
    [SerializeField][Range(0f,0.25f)] private float m_Thiccness=0.15f;
    [SerializeField]private AnimationCurve m_ThiccnessCurve;

    private void Start() {
        
    }

    private void Update() {
        m_VFX.SetFloat("Thiccness", m_ThiccnessCurve.Evaluate(m_Normalized)*m_Thiccness);
        m_VFX.SetFloat("Alpha", m_ThiccnessCurve.Evaluate(m_Normalized));

        m_StartPos.right = m_EndPos.position - m_StartPos.position;
        m_EndPos.right = m_StartPos.position - m_EndPos.position;

        float distance = Vector3.Distance(m_EndPos.position,m_StartPos.position);

        m_PosTwo.localPosition = m_StartCurve * distance/2;
        m_PosThree.localPosition = m_EndCurve * distance/2;

    }
}
